/*  Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2010 http://www.nequeo.com.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

#region Nequeo Pty Ltd License
/*
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:

    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Security.Permissions;
using System.Web.Hosting;
using System.IO;

using Nequeo.Handler.Global;

namespace Nequeo.Web.Provider
{
    /// <summary>
    /// Text file web application session state provider.
    /// </summary>
    public sealed class TextFileSessionStateProvider : SessionStateStoreProviderBase
    {
        #region Private Fields
        private Dictionary<string, FileStream> _sessions = new Dictionary<string, FileStream>();
        #endregion

        #region Abstract Method Overrides
        /// <summary>
        /// Initialise the new session state.
        /// </summary>
        /// <param name="name">The name of the session state.</param>
        /// <param name="config">The name value collection.</param>
        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "TextFileSessionStateProvider";

            // Add a default "description" attribute to config if the
            // attribute doesn’t exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Text file session state provider");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " + attr);
            }

            // Make sure we can read and write files in the ~/App_Data/Session_Data directory
            FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, SessionState.GetWebAppSessionStatePath());
            permission.Demand();
        }

        /// <summary>
        /// Creates a new System.Web.SessionState.SessionStateStoreData object to be
        /// used for the current request.
        /// </summary>
        /// <param name="context">The System.Web.HttpContext for the current request.</param>
        /// <param name="timeout">The session-state System.Web.SessionState.HttpSessionState.Timeout value
        /// for the new System.Web.SessionState.SessionStateStoreData.</param>
        /// <returns>A new System.Web.SessionState.SessionStateStoreData for the current request.</returns>
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            return new SessionStateStoreData(new SessionStateItemCollection(), SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }

        /// <summary>
        /// Adds a new session-state item to the data store.
        /// </summary>
        /// <param name="context">The System.Web.HttpContext for the current request.</param>
        /// <param name="id">The System.Web.SessionState.HttpSessionState.SessionID for the current request.</param>
        /// <param name="timeout">The session System.Web.SessionState.HttpSessionState.Timeout for the current
        /// request.</param>
        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            // Create a file containing an uninitialized flag and a time-out
            StreamWriter writer = null;

            try
            {
                writer = new StreamWriter(GetSessionFileName(id));
                writer.WriteLine("0");
                writer.WriteLine(timeout.ToString());
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Returns read-only session-state data from the session data store.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="id">The System.Web.HttpContext for the current request.</param>
        /// <param name="locked">The System.Web.SessionState.HttpSessionState.SessionID for the current request.</param>
        /// <param name="lockAge">When this method returns, contains a System.TimeSpan object that is set to
        ///  the amount of time that an item in the session data store has been locked.</param>
        /// <param name="lockId">When this method returns, contains an object that is set to the lock identifier
        /// for the current request. For details on the lock identifier, see "Locking
        /// Session-Store Data" in the System.Web.SessionState.SessionStateStoreProviderBase
        /// class summary.</param>
        /// <param name="actions">When this method returns, contains one of the System.Web.SessionState.SessionStateActions
        /// values, indicating whether the current session is an uninitialized, cookieless
        /// session.</param>
        /// <returns>A System.Web.SessionState.SessionStateStoreData populated with session values
        /// and information from the session data store.</returns>
        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            return GetSession(context, id, out locked, out lockAge, out lockId, out actions, false);
        }

        /// <summary>
        /// Returns read-only session-state data from the session data store.
        /// </summary>
        /// <param name="context">The System.Web.HttpContext for the current request.</param>
        /// <param name="id">The System.Web.SessionState.HttpSessionState.SessionID for the current request.</param>
        /// <param name="locked">When this method returns, contains a Boolean value that is set to 
        /// true if a lock is successfully obtained; otherwise, false.</param>
        /// <param name="lockAge">When this method returns, contains a System.TimeSpan object that is set to
        /// the amount of time that an item in the session data store has been locked.</param>
        /// <param name="lockId">When this method returns, contains an object that is set to the lock identifier
        /// for the current request. For details on the lock identifier, see "Locking
        /// Session-Store Data" in the System.Web.SessionState.SessionStateStoreProviderBase
        /// class summary.</param>
        /// <param name="actions">When this method returns, contains one of the System.Web.SessionState.SessionStateActions
        /// values, indicating whether the current session is an uninitialized, cookieless
        /// session.</param>
        /// <returns>A System.Web.SessionState.SessionStateStoreData populated with session values
        /// and information from the session data store.</returns>
        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
        {
            return GetSession(context, id, out locked, out lockAge, out lockId, out actions, true);
        }

        /// <summary>
        /// Updates the session-item information in the session-state data store with
        /// values from the current request, and clears the lock on the data.
        /// </summary>
        /// <param name="context">The System.Web.HttpContext for the current request.</param>
        /// <param name="id">The session identifier for the current request.</param>
        /// <param name="item">The System.Web.SessionState.SessionStateStoreData object that 
        /// contains the current session values to be stored.</param>
        /// <param name="lockId">The lock identifier for the current request.</param>
        /// <param name="newItem">true to identify the session item as a new item; 
        /// false to identify the session item as an existing item.</param>
        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            // Serialize the session
            byte[] items, statics;
            SerializeSession(item, out items, out statics);
            string serializedItems = Convert.ToBase64String(items);
            string serializedStatics = Convert.ToBase64String(statics);

            // Get a FileStream representing the session state file
            FileStream stream = null;

            try
            {
                if (newItem)
                    stream = File.Create(GetSessionFileName(id));
                else
                {
                    stream = _sessions[id];
                    stream.SetLength(0);
                    stream.Seek(0, SeekOrigin.Begin);
                }

                // Write session state to the file        
                StreamWriter writer = null;

                try
                {
                    writer = new StreamWriter(stream);
                    writer.WriteLine("1"); // Initialized flag
                    writer.WriteLine(serializedItems);
                    writer.WriteLine(serializedStatics);
                    writer.WriteLine(item.Timeout.ToString());
                }
                finally
                {
                    if (writer != null)
                        writer.Close();
                }
            }
            finally
            {
                if (newItem && stream != null)
                    stream.Close();
            }

            // Unlock the session
            ReleaseItemExclusive(context, id, lockId);
        }

        /// <summary>
        /// Releases a lock on an item in the session data store.
        /// </summary>
        /// <param name="context">The System.Web.HttpContext for the current request.</param>
        /// <param name="id">The session identifier for the current request.</param>
        /// <param name="lockId">The lock identifier for the current request.</param>
        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            // Release the specified session by closing the corresponding
            // FileStream and deleting the lock file
            FileStream stream;

            if (_sessions.TryGetValue(id, out stream))
            {
                _sessions.Remove(id);
                ReleaseLock(context, (string)lockId);
                stream.Close();
            }
        }

        /// <summary>
        /// Updates the expiration date and time of an item in the session data store.
        /// </summary>
        /// <param name="context">The System.Web.HttpContext for the current request.</param>
        /// <param name="id">The session identifier for the current request.</param>
        public override void ResetItemTimeout(HttpContext context, string id)
        {
            // Update the time stamp on the session state file
            string path = GetSessionFileName(id);
            File.SetCreationTime(path, DateTime.Now);
        }

        /// <summary>
        /// Deletes item data from the session data store.
        /// </summary>
        /// <param name="context">The System.Web.HttpContext for the current request.</param>
        /// <param name="id">The session identifier for the current request.</param>
        /// <param name="lockId">The lock identifier for the current request.</param>
        /// <param name="item">The System.Web.SessionState.SessionStateStoreData that 
        /// represents the itemto delete from the data store.</param>
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            // Make sure the session is unlocked
            ReleaseItemExclusive(context, id, lockId);

            // Delete the session state file
            File.Delete(GetSessionFileName(id));
        }

        /// <summary>
        /// Sets a reference to the System.Web.SessionState.SessionStateItemExpireCallback
        /// delegate for the Session_OnEnd event defined in the Global.asax file.
        /// </summary>
        /// <param name="expireCallback">The System.Web.SessionState.SessionStateItemExpireCallback 
        /// delegate for the Session_OnEnd event defined in the Global.asax file.</param>
        /// <returns>true if the session-state store provider supports calling the 
        /// Session_OnEnd event; otherwise, false.</returns>
        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            // This provider doesn't support expiration callbacks,
            // so simply return false here
            return false;
        }

        /// <summary>
        /// Called by the System.Web.SessionState.SessionStateModule 
        /// object for per-request initialization.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public override void InitializeRequest(HttpContext context)
        {
        }

        /// <summary>
        /// Called by the System.Web.SessionState.SessionStateModule 
        /// object at the end of a request.
        /// </summary>
        /// <param name="context">The current http context.</param>
        public override void EndRequest(HttpContext context)
        {
        }

        /// <summary>
        /// When the current web application has ended.
        /// </summary>
        public override void Dispose()
        {
            // Make sure no session state files are left open
            foreach (KeyValuePair<string, FileStream> pair in _sessions)
            {
                pair.Value.Close();
                _sessions.Remove(pair.Key);
            }

            // Delete session files and lock files
            File.Delete(SessionState.GetWebAppSessionStatePath() + "/*_Session.txt");
            File.Delete(SessionState.GetWebAppSessionStatePath() + "/*_Lock.txt");
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the current session state data.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <param name="id">The id of the session state.</param>
        /// <param name="locked">Is the session state file locked.</param>
        /// <param name="lockAge">The current age of the locked file.</param>
        /// <param name="lockId">The locked file id.</param>
        /// <param name="actions">The current session state actions</param>
        /// <param name="exclusive">Should the file be in read write mode else false for read only.</param>
        /// <returns>The session state data.</returns>
        private SessionStateStoreData GetSession(HttpContext context, string id, out bool locked, 
            out TimeSpan lockAge, out object lockId, out SessionStateActions actions, bool exclusive)
        {
            // Assign default values to out parameters
            locked = false;
            lockId = null;
            lockAge = TimeSpan.Zero;
            actions = SessionStateActions.None;

            FileStream stream = null;

            try
            {
                // Attempt to open the session state file
                string path = GetSessionFileName(id);
                FileAccess access = exclusive ? FileAccess.ReadWrite : FileAccess.Read;
                FileShare share = exclusive ? FileShare.None : FileShare.Read;
                stream = File.Open(path, FileMode.Open, access, share);
            }
            catch (FileNotFoundException)
            {
                // Not an error if file doesn't exist
                return null;
            }
            catch (IOException)
            {
                // If we come here, the session is locked because
                // the file couldn't be opened
                locked = true;
                lockId = id;
                lockAge = GetLockAge(context, id);
                return null;
            }

            // Place a lock on the session
            CreateLock(context, id);
            locked = true;
            lockId = id;

            // Save the FileStream reference so it can be used later
            _sessions.Add(id, stream);

            // Find out whether the session is initialized
            StreamReader reader = new StreamReader(stream);
            string flag = reader.ReadLine();
            bool initialized = (flag == "1");

            if (!initialized)
            {
                // Return an empty SessionStateStoreData
                actions = SessionStateActions.InitializeItem;
                int timeout = Convert.ToInt32(reader.ReadLine());
                return new SessionStateStoreData(new SessionStateItemCollection(),
                    SessionStateUtility.GetSessionStaticObjects(context), timeout);
            }
            else
            {
                // Read Items, StaticObjects, and Timeout from the file
                // (NOTE: Don't close the StreamReader, because doing so
                // will close the file)
                byte[] items = Convert.FromBase64String(reader.ReadLine());
                byte[] statics = Convert.FromBase64String(reader.ReadLine());
                int timeout = Convert.ToInt32(reader.ReadLine());

                // Deserialize the session
                return DeserializeSession(items, statics, timeout);
            }
        }

        /// <summary>
        /// Create a lock.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <param name="id">The id of the session state.</param>
        private void CreateLock(HttpContext context, string id)
        {
            // Create a lock file so the lock's age can be determined
            File.Create(GetLockFileName(id)).Close();
        }

        /// <summary>
        /// Release the session state lock.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <param name="id">The id of the session state.</param>
        private void ReleaseLock(HttpContext context, string id)
        {
            // Delete the lock file
            string path = GetLockFileName(id);
            if (File.Exists(path))
                File.Delete(path);
        }

        /// <summary>
        /// Get the current age of the session item.
        /// </summary>
        /// <param name="context">The current http context.</param>
        /// <param name="id">The id of the session state.</param>
        /// <returns>The time span of the session object.</returns>
        private TimeSpan GetLockAge(HttpContext context, string id)
        {
            try
            {
                return DateTime.Now - File.GetCreationTime(GetLockFileName(id));
            }
            catch (FileNotFoundException)
            {
                // This is important, because it's possible that
                // a lock is active but the lock file hasn’t been
                // created yet if another thread owns the lock
                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Get the session file name.
        /// </summary>
        /// <param name="id">The id of the session state.</param>
        /// <returns>The file name of the session data.</returns>
        private string GetSessionFileName(string id)
        {
            return String.Format(SessionState.GetWebAppSessionStatePath() +  "/{0}_Session.txt", id);
        }

        /// <summary>
        /// Get the lock session file name.
        /// </summary>
        /// <param name="id">The id of the session state.</param>
        /// <returns>The file name of the lock session data.</returns>
        private string GetLockFileName(string id)
        {
            return String.Format(SessionState.GetWebAppSessionStatePath() + "/{0}_Lock.txt", id);
        }

        /// <summary>
        /// Serialise the session state data.
        /// </summary>
        /// <param name="store">The session state data to serialise.</param>
        /// <param name="items">The resulting serialised data.</param>
        /// <param name="statics">The resulting serialised statics</param>
        private void SerializeSession(SessionStateStoreData store, out byte[] items, out byte[] statics)
        {
            MemoryStream stream1 = null, stream2 = null;
            BinaryWriter writer1 = null, writer2 = null;

            try
            {
                stream1 = new MemoryStream();
                stream2 = new MemoryStream();
                writer1 = new BinaryWriter(stream1);
                writer2 = new BinaryWriter(stream2);

                ((SessionStateItemCollection)store.Items).Serialize(writer1);
                store.StaticObjects.Serialize(writer2);

                items = stream1.ToArray();
                statics = stream2.ToArray();
            }
            finally
            {
                if (writer2 != null)
                    writer2.Close();
                if (writer1 != null)
                    writer1.Close();
                if (stream2 != null)
                    stream2.Close();
                if (stream1 != null)
                    stream1.Close();
            }
        }

        /// <summary>
        /// Deserialise the session state data.
        /// </summary>
        /// <param name="items">The bytes to deserialise.</param>
        /// <param name="statics">The http static object collection.</param>
        /// <param name="timeout">The timeout for deserialisation.</param>
        /// <returns>The session state object.</returns>
        private SessionStateStoreData DeserializeSession(byte[] items, byte[] statics, int timeout)
        {
            MemoryStream stream1 = null, stream2 = null;
            BinaryReader reader1 = null, reader2 = null;

            try
            {
                stream1 = new MemoryStream(items);
                stream2 = new MemoryStream(statics);
                reader1 = new BinaryReader(stream1);
                reader2 = new BinaryReader(stream2);

                return new SessionStateStoreData(SessionStateItemCollection.Deserialize(reader1),
                    HttpStaticObjectsCollection.Deserialize(reader2), timeout);
            }
            finally
            {
                if (reader2 != null)
                    reader2.Close();
                if (reader1 != null)
                    reader1.Close();
                if (stream2 != null)
                    stream2.Close();
                if (stream1 != null)
                    stream1.Close();
            }
        }
        #endregion
    }
}
