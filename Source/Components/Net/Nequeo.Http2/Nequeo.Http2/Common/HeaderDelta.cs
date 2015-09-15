/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nequeo.Collections;
using Nequeo.Extension;
using Nequeo.IO.Compression;

using Nequeo.Net.Http2.Protocol;

namespace Nequeo.Net.Http2
{
    /// <summary>
    /// Header delta compression algorithm.
    /// http://tools.ietf.org/html/draft-ietf-httpbis-header-compression-07
    /// </summary>
    internal class HeaderCompression : IDisposable
    {
        /// <summary>
        /// Header delta compression algorithm.
        /// </summary>
        public HeaderCompression()
        {
            //The initial value is 4,096 bytes.
            _maxHeaderByteSize = 4096;
            _isSettingHeaderTableSizeReceived = false;

            //The header table is initially empty.
            _remoteHeadersTable = new HeadersList();
            _localHeadersTable = new HeadersList();

            _remoteRefSet = new HeadersList();
            _localRefSet = new HeadersList();

            _huffmanProc = new HuffmanStream();

            InitCompressor();
            InitDecompressor();
        }

        private HeadersList _remoteHeadersTable;
        private HeadersList _remoteRefSet;
        private HeadersList _localHeadersTable;
        private HeadersList _localRefSet;

        private HuffmanStream _huffmanProc;
        private int _currentOffset;
        private MemoryStream _serializerStream;
        private int _maxHeaderByteSize;

        // This new maximum size MUST be lower than or equal to the value 
        // of the setting SETTINGS_HEADER_TABLE_SIZE
        private int _settingsHeaderTableSize;
        private bool _isSettingHeaderTableSizeReceived;

        /* 07 -> 4.1.2
        String Length:  The number of octets used to encode the string
        literal, encoded as an integer with 7-bit prefix. */
        private const byte stringPrefix = 7;

        /// <summary>
        /// Evict header table entry.
        /// </summary>
        /// <param name="headersTable"></param>
        /// <param name="refTable"></param>
        private void EvictHeaderTableEntries(HeadersList headersTable, HeadersList refTable)
        {
            /* 07 -> 3.3.2
            Whenever the maximum size for the header table is made smaller,
            entries are evicted from the end of the header table until the size
            of the header table is less than or equal to the maximum size. */
            while (headersTable.StoredHeadersSize >= _maxHeaderByteSize && headersTable.Count > 0)
            {
                var header = headersTable[headersTable.Count - 1];
                headersTable.RemoveAt(headersTable.Count - 1);

                /* 07 -> 3.3.2
                Whenever an entry is evicted from the header table, any reference to
                that entry contained by the reference set is removed. */
                if (refTable.Contains(header))
                    refTable.Remove(header);
            }
        }

        /// <summary>
        /// Notify settings changes.
        /// </summary>
        /// <param name="newMaxVal">The new maximum value.</param>
        public void NotifySettingsChanges(int newMaxVal)
        {
            if (newMaxVal <= 0)
                throw new Exception("invalid max header table size in settings");

            _isSettingHeaderTableSizeReceived = true;
            _settingsHeaderTableSize = newMaxVal;

            _maxHeaderByteSize = newMaxVal;

            EvictHeaderTableEntries(_remoteHeadersTable, _remoteRefSet);
            EvictHeaderTableEntries(_localHeadersTable, _localRefSet);
        }

        /// <summary>
        /// Change Max Header Table Size when receiving appropriate Encoding Context Update
        /// </summary>
        /// <param name="newMaxVal">The new maximun value.</param>
        private void ChangeMaxHeaderTableSize(int newMaxVal)
        {
            if (newMaxVal <= 0)
                throw new Exception("invalid max header table size");

            _maxHeaderByteSize = newMaxVal;

            EvictHeaderTableEntries(_remoteHeadersTable, _remoteRefSet);
            EvictHeaderTableEntries(_localHeadersTable, _localRefSet);
        }

        /// <summary>
        /// Initilise compressor.
        /// </summary>
        private void InitCompressor()
        {
            _serializerStream = new MemoryStream();
        }

        /// <summary>
        /// Initialise decompressor.
        /// </summary>
        private void InitDecompressor()
        {
            _currentOffset = 0;
        }

        /// <summary>
        /// Insert to headers table.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <param name="refSet">The refernced header list.</param>
        /// <param name="headersTable">The header list.</param>
        private void InsertToHeadersTable(KeyValuePair<string, string> header, HeadersList refSet,
            HeadersList headersTable)
        {
            /* 07 -> 3.3.1
            The size of an entry is the sum of its name's length in octets (as
            defined in Section 4.1.2), of its value's length in octets
            (Section 4.1.2) and of 32 octets. */
            int headerLen = header.Key.Length + header.Value.Length + 32;

            /* 07 -> 3.3.2 
            Whenever a new entry is to be added to the table, any name referenced
            by the representation of this new entry is cached, and then entries
            are evicted from the end of the header table until the size of the
            header table is less than or equal to (maximum size - new entry
            size), or until the table is empty. 
            
            If the size of the new entry is less than or equal to the maximum
            size, that entry is added to the table.  It is not an error to
            attempt to add an entry that is larger than the maximum size. */

            while (headersTable.StoredHeadersSize + headerLen >= _maxHeaderByteSize && headersTable.Count > 0)
            {
                headersTable.RemoveAt(headersTable.Count - 1);

                /* 07 -> 3.3.2
                Whenever an entry is evicted from the header table, any reference to
                that entry contained by the reference set is removed. */

                if (refSet.Contains(header))
                    refSet.Remove(header);
            }

            /* 07 -> 3.2.1
            We should always insert into 
            begin of the headers table. */
            headersTable.Insert(0, header);
        }

        #region Compression
        /// <summary>
        /// Encode string.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="useHuffman">Use huffman.</param>
        /// <returns>The encoded string.</returns>
        private byte[] EncodeString(string item, bool useHuffman)
        {
            byte[] itemBts;
            int len;

            const byte prefix = 7;

            byte[] lenBts;

            if (!useHuffman)
            {
                itemBts = Encoding.UTF8.GetBytes(item);
                len = item.Length;
                lenBts = len.ToUVarInt(prefix);
            }
            else
            {
                itemBts = Encoding.UTF8.GetBytes(item);
                itemBts = _huffmanProc.Compress(itemBts);

                len = itemBts.Length;
                lenBts = len.ToUVarInt(prefix);

                lenBts[0] |= 0x80;
            }

            byte[] result = new byte[lenBts.Length + itemBts.Length];

            Buffer.BlockCopy(lenBts, 0, result, 0, lenBts.Length);
            Buffer.BlockCopy(itemBts, 0, result, lenBts.Length, itemBts.Length);

            return result;
        }

        /// <summary>
        /// Compress incremental.
        /// </summary>
        /// <param name="header">The header.</param>
        private void CompressIncremental(KeyValuePair<string, string> header)
        {
            const byte prefix = (byte)UVarIntPrefix.Incremental;
            /* 12 -> 8.1.3
            Just as in HTTP/1.x, header field names are strings of ASCII
            characters that are compared in a case-insensitive fashion.  However,
            header field names MUST be converted to lowercase prior to their
            encoding in HTTP/2. */
            int index = _remoteHeadersTable.FindIndex(kv => kv.Key.Equals(header.Key, StringComparison.OrdinalIgnoreCase));
            bool isFound = index != -1;

            /* 07 -> 3.1.4
                    <----------  Index Address Space ---------->
                    <-- Header  Table -->  <-- Static  Table -->
                    +---+-----------+---+  +---+-----------+---+
                    | 1 |    ...    | k |  |k+1|    ...    | n |
                    +---+-----------+---+  +---+-----------+---+
                    ^                   |
                    |                   V
            Insertion Point       Drop Point
            */
            // It's necessary to form result array because partial writeToOutput stream
            // can cause problems because of multithreading
            using (var stream = new MemoryStream(64))
            {
                byte[] indexBinary;
                byte[] nameBinary = new byte[0];
                byte[] valueBinary;

                if (isFound)
                {
                    // Header key was found in the header table. Hence we should encode only value
                    indexBinary = (index + 1).ToUVarInt(prefix);
                    valueBinary = EncodeString(header.Value, true);
                }
                else
                {
                    // Header key was not found in the header table. Hence we should encode name and value
                    indexBinary = 0.ToUVarInt(prefix);
                    nameBinary = EncodeString(header.Key, true);
                    valueBinary = EncodeString(header.Value, true);
                }

                // Set without index type
                indexBinary[0] |= (byte)IndexationType.Incremental;

                stream.Write(indexBinary, 0, indexBinary.Length);
                stream.Write(nameBinary, 0, nameBinary.Length);
                stream.Write(valueBinary, 0, valueBinary.Length);

                WriteToOutput(stream.GetBuffer(), 0, (int)stream.Position);
            }

            InsertToHeadersTable(header, _remoteRefSet, _remoteHeadersTable);
        }

        /// <summary>
        /// Compressed indexed.
        /// </summary>
        /// <param name="header">The header.</param>
        private void CompressIndexed(KeyValuePair<string, string> header)
        {
            /* 07 -> 3.2.1
            An _indexed representation_ corresponding to an entry _not present_
            in the reference set entails the following actions:

            o  If referencing an element of the static table:

                *  The header field corresponding to the referenced entry is
                    emitted.

                *  The referenced static entry is inserted at the beginning of the
                    header table.

                *  A reference to this new header table entry is added to the
                    reference set, except if this new entry didn't fit in the
                    header table. */

            int index = _remoteHeadersTable.FindIndex(kv => kv.Key.Equals(header.Key) && kv.Value.Equals(header.Value));
            bool isFound = index != -1;

            /* 07 -> 3.1.4
                    <----------  Index Address Space ---------->
                    <-- Header  Table -->  <-- Static  Table -->
                    +---+-----------+---+  +---+-----------+---+
                    | 1 |    ...    | k |  |k+1|    ...    | n |
                    +---+-----------+---+  +---+-----------+---+
                    ^                   |
                    |                   V
            Insertion Point       Drop Point
            */
            if (!isFound)
            {
                index = Constants.StaticTable.FindIndex(kv => kv.Key.Equals(header.Key, StringComparison.OrdinalIgnoreCase)
                                                    && kv.Value.Equals(header.Value, StringComparison.OrdinalIgnoreCase));
                isFound = index != -1;

                if (isFound)
                {
                    index += _remoteHeadersTable.Count;
                    /* 07 -> 3.2.1
                    The referenced static entry is inserted at the beginning of the
                    header table. */
                    _remoteHeadersTable.Insert(0, header);
                }
            }

            if (!isFound)
            {
                throw new Exception("cant compress indexed header. Index not found.");
            }

            const byte prefix = (byte)UVarIntPrefix.Indexed;
            var bytes = (index + 1).ToUVarInt(prefix);

            // Set indexed type
            bytes[0] |= (byte)IndexationType.Indexed;

            WriteToOutput(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Compress the header list.
        /// </summary>
        /// <param name="headers">The headers.</param>
        /// <returns>The compressed headers.</returns>
        public byte[] Compress(HeadersList headers)
        {
            var toSend = new HeadersList();
            var toDelete = new HeadersList(_remoteRefSet);

            ClearStream(_serializerStream, (int)_serializerStream.Position);

            foreach (var header in headers)
            {
                if (header.Key == null || header.Value == null)
                {
                    throw new InvalidHeaderException(header);
                }
                if (!_remoteRefSet.Contains(header))
                {
                    // Not there, Will send
                    toSend.Add(header);
                }
                else
                {
                    // Already there, don't delete
                    toDelete.Remove(header);
                }
            }

            foreach (var header in toDelete)
            {
                // Anything left in toDelete, should send, so it is deleted from ref set.
                CompressIndexed(header);
                _remoteRefSet.Remove(header); // Update our copy
            }

            foreach (var header in toSend)
            {
                // Send whatever was left in headersCopy
                if (_remoteHeadersTable.Contains(header) || Constants.StaticTable.Contains(header))
                {
                    CompressIndexed(header);
                }
                else
                {
                    CompressIncremental(header);
                }

                _remoteRefSet.Add(header);
            }

            _serializerStream.Flush();
            var result = new byte[_serializerStream.Position];

            var streamBuffer = _serializerStream.GetBuffer();

            Buffer.BlockCopy(streamBuffer, 0, result, 0, (int)_serializerStream.Position);

            return result;
        }

        #endregion

        #region Decompression
        /// <summary>
        /// Decode string.
        /// </summary>
        /// <param name="bytes">The bytes to decode.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>The decoded string.</returns>
        private string DecodeString(byte[] bytes, byte prefix)
        {
            int maxPrefixVal = (1 << prefix) - 1;

            // Get first bit. If true => huffman used
            bool isHuffman = (bytes[_currentOffset] & 0x80) != 0;

            int len;

            // throw away huffman's mask
            bytes[_currentOffset] &= 0x7f;

            if ((bytes[_currentOffset]) < maxPrefixVal)
            {
                len = bytes[_currentOffset++];
            }
            else
            {
                int i = 1;
                while (true)
                {
                    if ((bytes[_currentOffset + i] & 0x80) == 0)
                    {
                        break;
                    }
                    i++;
                }

                var numberBytes = new byte[++i];
                Buffer.BlockCopy(bytes, _currentOffset, numberBytes, 0, i);
                _currentOffset += i;

                len = Int32Extensions.FromUVarInt(numberBytes);
            }

            string result = String.Empty;

            if (isHuffman)
            {
                var compressedBytes = new byte[len];
                Buffer.BlockCopy(bytes, _currentOffset, compressedBytes, 0, len);
                var decodedBytes = _huffmanProc.Decompress(compressedBytes);
                result = Encoding.UTF8.GetString(decodedBytes);

                _currentOffset += len;

                return result;
            }

            result = Encoding.UTF8.GetString(bytes, _currentOffset, len);
            _currentOffset += len;

            return result;
        }

        /// <summary>
        /// Process cookies.
        /// </summary>
        /// <param name="toProcess">The header list containing the cookies.</param>
        private void ProcessCookie(HeadersList toProcess)
        {
            /* 12 -> 8.1.3.4.
            If there are multiple Cookie header fields after
            decompression, these MUST be concatenated into a single octet string
            using the two octet delimiter of 0x3B, 0x20 (the ASCII string "; "). */

            const string delimiter = "; ";
            var cookie = new StringBuilder(String.Empty);

            for (int i = 0; i < toProcess.Count; i++)
            {
                if (!toProcess[i].Key.Equals(CommonHeaders.Cookie))
                    continue;

                cookie.Append(toProcess[i].Value);
                cookie.Append(delimiter);
                toProcess.RemoveAt(i--);
            }

            if (cookie.Length > 0)
            {
                // Add without last delimeter
                toProcess.Add(new KeyValuePair<string, string>(CommonHeaders.Cookie,
                                                               cookie.ToString(cookie.Length - 2, 2)));
            }
        }

        /// <summary>
        /// Process indexed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The indexation.</returns>
        private Tuple<string, string, IndexationType> ProcessIndexed(int index)
        {
            /* 07 -> 4.2
            The index value of 0 is not used. It MUST be treated as a decoding
            error if found in an indexed header field representation. */
            if (index == 0)
                throw new Exception("indexed representation with zero value");

            var header = default(KeyValuePair<string, string>);
            bool isInStatic = index > _localHeadersTable.Count & index <= _localHeadersTable.Count + Constants.StaticTable.Count;
            bool isInHeaders = index <= _localHeadersTable.Count;

            if (isInStatic)
            {
                header = Constants.StaticTable[index - _localHeadersTable.Count - 1];
            }
            else if (isInHeaders)
            {
                header = _localHeadersTable[index - 1];
            }
            else
            {
                throw new IndexOutOfRangeException("no such index nor in static neither in headers tables");
            }

            /* 07 -> 3.2.1
            An _indexed representation_ corresponding to an entry _present_ in
            the reference set entails the following actions:

            o  The entry is removed from the reference set. */
            if (_localRefSet.Contains(header))
            {
                _localRefSet.Remove(header);
                return null;
            }

            /* 07 -> 3.2.1
            An _indexed representation_ corresponding to an entry _not present_
            in the reference set entails the following actions:

            o  If referencing an element of the static table:

                *  The header field corresponding to the referenced entry is
                    emitted.

                *  The referenced static entry is inserted at the beginning of the
                    header table.

                *  A reference to this new header table entry is added to the
                    reference set, except if this new entry didn't fit in the
                    header table.

            o  If referencing an element of the header table:

                *  The header field corresponding to the referenced entry is
                    emitted.

                *  The referenced header table entry is added to the reference
                    set.
            */
            if (isInStatic)
            {
                _localHeadersTable.Insert(0, header);
            }

            _localRefSet.Add(header);

            return new Tuple<string, string, IndexationType>(header.Key, header.Value, IndexationType.Indexed);
        }

        /// <summary>
        /// Process without indexing.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="index">The index.</param>
        /// <returns>The list.</returns>
        private Tuple<string, string, IndexationType> ProcessWithoutIndexing(byte[] bytes, int index)
        {
            string name;
            string value;

            if (index == 0)
            {
                name = DecodeString(bytes, stringPrefix);
            }
            else
            {
                // Index increased by 1 was sent
                name = index < _localHeadersTable.Count ? _localHeadersTable[index - 1].Key : Constants.StaticTable[index - 1].Key;
            }

            value = DecodeString(bytes, stringPrefix);

            /* 07 -> 3.2.1
            A _literal representation_ that is _not added_ to the header table
            entails the following action:

            o  The header field is emitted. */
            return new Tuple<string, string, IndexationType>(name, value, IndexationType.WithoutIndexation);
        }

        /// <summary>
        /// Process incremental.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="index">The index.</param>
        /// <returns>The list.</returns>
        private Tuple<string, string, IndexationType> ProcessIncremental(byte[] bytes, int index)
        {
            string name;
            string value;

            if (index == 0)
            {
                name = DecodeString(bytes, stringPrefix);
            }
            else
            {
                // Index increased by 1 was sent
                name = index - 1 < _localHeadersTable.Count ?
                                _localHeadersTable[index - 1].Key :
                                Constants.StaticTable[index - _localHeadersTable.Count - 1].Key;
            }

            value = DecodeString(bytes, stringPrefix);

            /* 07 -> 3.2.1
            A _literal representation_ that is _added_ to the header table
            entails the following actions:

            o  The header field is emitted.

            o  The header field is inserted at the beginning of the header table.

            o  A reference to the new entry is added to the reference set (except
                if this new entry didn't fit in the header table). */

            // o  The header field is inserted at the beginning of the header table.
            // This action will be performed when ModifyTable will be called

            var header = new KeyValuePair<string, string>(name, value);

            _localRefSet.Add(header);

            //This action will be performed when ModifyTable will be called
            InsertToHeadersTable(header, _localRefSet, _localHeadersTable);

            return new Tuple<string, string, IndexationType>(name, value, IndexationType.Incremental);
        }

        /// <summary>
        /// Process encoding context update.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="changeTypeFlag">Change type flag.</param>
        /// <returns>The collection.</returns>
        private Tuple<string, string, IndexationType> ProcessEncodingContextUpdate(int command, bool changeTypeFlag)
        {
            if (!changeTypeFlag)
            {
                /* 07 -> 4.4
                This new maximum size MUST be lower than
                or equal to the value of the setting SETTINGS_HEADER_TABLE_SIZE */
                //spec tells that in this case command should be interpreted as new table size
                int newTableSize = command;

                if (_isSettingHeaderTableSizeReceived && (newTableSize <= _settingsHeaderTableSize))
                {
                    ChangeMaxHeaderTableSize(newTableSize);
                }
                else if (!_isSettingHeaderTableSizeReceived)
                {
                    ChangeMaxHeaderTableSize(newTableSize);
                }
                else
                {
                    throw new Exception("incorrect max header table size in Encoding Context Update");
                }
            }
            else if (command == 0)
            {
                _localRefSet.Clear();
            }
            else
            {
                throw new Exception("incorrect format of Encoding Context Update");
            }

            return null;
        }

        /// <summary>
        /// Process never indexed.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="index">The index.</param>
        /// <returns>The list.</returns>
        private Tuple<string, string, IndexationType> ProcessNeverIndexed(byte[] bytes,
            int index)
        {
            /* 07 -> 4.3.3
            The encoding of the representation is the same as for the literal
            header field without indexing representation. */

            string name;
            string value;

            if (index == 0)
            {
                name = DecodeString(bytes, stringPrefix);
            }
            else
            {
                // Index increased by 1 was sent
                name = index < _localHeadersTable.Count ? _localHeadersTable[index - 1].Key : Constants.StaticTable[index - 1].Key;
            }

            value = DecodeString(bytes, stringPrefix);

            return new Tuple<string, string, IndexationType>(name, value, IndexationType.NeverIndexed);
        }

        /// <summary>
        /// Parse hedaer.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The list.</returns>
        private Tuple<string, string, IndexationType> ParseHeader(byte[] bytes)
        {
            var type = GetHeaderType(bytes);

            // If type is EncodingContextUpdate then it contains also flag
            bool changeTypeFlag = false;
            if (type == IndexationType.EncodingContextUpdate)
                changeTypeFlag = GetFlagOfEncodingContextUpdate(bytes);

            int index = GetIndex(bytes, type);

            switch (type)
            {
                case IndexationType.Indexed:
                    return ProcessIndexed(index);
                case IndexationType.Incremental:
                    return ProcessIncremental(bytes, index);
                case IndexationType.EncodingContextUpdate:
                    return ProcessEncodingContextUpdate(index, changeTypeFlag);
                case IndexationType.NeverIndexed:
                    return ProcessNeverIndexed(bytes, index);
                case IndexationType.WithoutIndexation:
                    return ProcessWithoutIndexing(bytes, index);
            }

            throw new Exception("Unknown indexation type");
        }

        /// <summary>
        /// Get index.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="type">The indexation type.</param>
        /// <returns>The index.</returns>
        private int GetIndex(byte[] bytes, IndexationType type)
        {
            byte prefix = 0;
            byte firstByteValue = bytes[_currentOffset];

            switch (type)
            {
                case IndexationType.Incremental:
                    prefix = (byte)UVarIntPrefix.Incremental;
                    break;
                case IndexationType.WithoutIndexation:
                    prefix = (byte)UVarIntPrefix.WithoutIndexing;
                    break;
                case IndexationType.Indexed:
                    prefix = (byte)UVarIntPrefix.Indexed;
                    break;
                case IndexationType.EncodingContextUpdate:
                    prefix = (byte)UVarIntPrefix.EncodingContextUpdate;
                    break;
                case IndexationType.NeverIndexed:
                    prefix = (byte)UVarIntPrefix.NeverIndexed;
                    break;
            }

            int maxPrefixVal = (1 << prefix) - 1;

            if (firstByteValue < maxPrefixVal)
            {
                _currentOffset++;
                return firstByteValue;
            }

            int i = 1;
            while (true)
            {
                if ((bytes[_currentOffset + i] & (byte)IndexationType.Indexed) == 0)
                {
                    break;
                }
                i++;
            }

            var numberBytes = new byte[++i];
            Buffer.BlockCopy(bytes, _currentOffset, numberBytes, 0, i);
            _currentOffset += i;

            return Int32Extensions.FromUVarInt(numberBytes);
        }

        /// <summary>
        /// Get header type.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The indexation type.</returns>
        private IndexationType GetHeaderType(byte[] bytes)
        {
            var typeByte = bytes[_currentOffset];
            IndexationType indexationType;

            if ((typeByte & (byte)IndexationType.Indexed) == (byte)IndexationType.Indexed)
            {
                indexationType = IndexationType.Indexed;
            }
            else if ((typeByte & (byte)IndexationType.Incremental) == (byte)IndexationType.Incremental)
            {
                indexationType = IndexationType.Incremental;
            }
            else if ((typeByte & (byte)IndexationType.EncodingContextUpdate) ==
                (byte)IndexationType.EncodingContextUpdate)
            {
                indexationType = IndexationType.EncodingContextUpdate;
            }
            else if ((typeByte & (byte)IndexationType.NeverIndexed) ==
                     (byte)IndexationType.NeverIndexed)
            {
                indexationType = IndexationType.NeverIndexed;
            }
            /* When we get the type, WithoutIndexation type is assigned when other types are not suitable. 
            Therefore mask is not used since pattern of any representation is suitable to 0x00 mask. */
            else
            {
                indexationType = IndexationType.WithoutIndexation;
            }
            // throw type mask away
            bytes[_currentOffset] = (byte)(bytes[_currentOffset] & (~(byte)indexationType));
            return indexationType;
        }

        /// <summary>
        /// Get flag of encoding context update.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>True ig changed flag type; else false.</returns>
        private bool GetFlagOfEncodingContextUpdate(byte[] bytes)
        {
            const byte flagMask = 0x10; // depends on the pattern length
            var changeTypeFlag = (bytes[_currentOffset] & flagMask) == flagMask;
            bytes[_currentOffset] = (byte)(bytes[_currentOffset] & (~flagMask));
            return changeTypeFlag;
        }

        /// <summary>
        /// Decompress the headers.
        /// </summary>
        /// <param name="serializedHeaders">The serialised headers.</param>
        /// <returns>The header list.</returns>
        public HeadersList Decompress(byte[] serializedHeaders)
        {
            try
            {
                _currentOffset = 0;
                var unindexedHeadersList = new HeadersList();

                while (_currentOffset != serializedHeaders.Length)
                {
                    var entry = ParseHeader(serializedHeaders);

                    // parsed indexed header which was already in the refSet 
                    if (entry == null)
                        continue;

                    var header = new KeyValuePair<string, string>(entry.Item1, entry.Item2);

                    if (entry.Item3 == IndexationType.WithoutIndexation ||
                        entry.Item3 == IndexationType.NeverIndexed)
                    {
                        unindexedHeadersList.Add(header);
                    }
                }

                // Base result on already modified reference set
                var result = new HeadersList(_localRefSet);

                // Add to result Without indexation and Never Indexed
                // They were not added into reference set
                result.AddRange(unindexedHeadersList);

                ProcessCookie(result);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #endregion

        /// <summary>
        /// Write to output.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        private void WriteToOutput(byte[] bytes, int offset, int length)
        {
            _serializerStream.Write(bytes, offset, length);
        }

        /// <summary>
        /// Clear the stream.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="len">The length.</param>
        private static void ClearStream(Stream input, int len)
        {
            var buffer = new byte[len];
            input.Position = 0;
            input.Write(buffer, 0, len);
            input.SetLength(0);
        }

        #region Dispose Object Methods
        /// <summary>
        /// Track whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // Note disposing has been done.
                _disposed = true;

                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_serializerStream != null)
                        _serializerStream.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                _serializerStream = null;
            }
        }

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~HeaderCompression()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
        #endregion
    }
}
