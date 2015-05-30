/*  Company :       Nequeo Pty Ltd, http://www.nequeo.net.au/
 *  Copyright :     Copyright © Nequeo Pty Ltd 2013 http://www.nequeo.net.au/
 * 
 *  File :          
 *  Purpose :       
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace Nequeo.IO.Compression
{
    /// <summary>
    /// Represents a package of compressed files/data in the zip archive format.
    /// </summary>
    public class Archive
    {
        /// <summary>
        /// Get the relative file path.
        /// </summary>
        /// <param name="filename">The name of the file.</param>
        /// <returns>The relative file path.</returns>
        private string GetRelativePath(string filename)
        {
            // Get the relative info
            string rootPath = Path.GetPathRoot(filename);
            string relativePath = Path.GetDirectoryName(filename).TrimEnd('\\') + "\\";
            return relativePath.Replace(rootPath.TrimEnd('\\') + "\\", "");
        }

        /// <summary>
        /// Add a file to the archive.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to add the file to.</param>
        /// <param name="filename">The name of the file to add.</param>
        public void AddFile(string zipFilename, string filename)
        {
            // Open the archive.
            using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Create))
            {
                // Add the file to the archive.
                archive.CreateEntryFromFile(filename, GetRelativePath(filename) + Path.GetFileName(filename));
            }
        }

        /// <summary>
        /// Add files to the archive.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to add the files to.</param>
        /// <param name="filenames">The name of the files to add.</param>
        public void AddFiles(string zipFilename, string[] filenames)
        {
            // Open the archive.
            using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Create))
            {
                // For each file add to the archive.
                foreach (string file in filenames)
                {
                    // Add the file to the archive.
                    archive.CreateEntryFromFile(file, GetRelativePath(file) + Path.GetFileName(file));
                }
            }
        }

        /// <summary>
        /// Add text to the archive.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to add the text to.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="entryName">The entry name.</param>
        public void AddText(string zipFilename, string text, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Create))
            {
                // Create an empty entry.
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                // Create the write stream of the entry.
                using (StreamWriter writer = new StreamWriter(entry.Open()))
                {
                    // Write the text.
                    writer.Write(text);
                }
            }
        }

        /// <summary>
        /// Add text to the archive.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to add the text to.</param>
        /// <param name="writeAction">The action used to write the text.</param>
        /// <param name="entryName">The entry name.</param>
        public void AddText(string zipFilename, Action<StreamWriter> writeAction, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Create))
            {
                // Create an empty entry.
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                // Create the write stream of the entry.
                using (StreamWriter writer = new StreamWriter(entry.Open()))
                {
                    // Write the text from the action handler.
                    writeAction(writer);
                }
            }
        }

        /// <summary>
        /// Add data to the archive.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to add the data to.</param>
        /// <param name="data">The data to write.</param>
        /// <param name="entryName">The entry name.</param>
        public void AddData(string zipFilename, byte[] data, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Create))
            {
                // Create an empty entry.
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                // Create the write stream of the entry.
                using (BinaryWriter writer = new BinaryWriter(entry.Open()))
                {
                    // Write the text.
                    writer.Write(data);
                }
            }
        }

        /// <summary>
        /// Add data to the archive.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to add the data to.</param>
        /// <param name="writeAction">The action used to write the data.</param>
        /// <param name="entryName">The entry name.</param>
        public void AddData(string zipFilename, Action<BinaryWriter> writeAction, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Create))
            {
                // Create an empty entry.
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                // Create the write stream of the entry.
                using (BinaryWriter writer = new BinaryWriter(entry.Open()))
                {
                    // Write the text from the action handler.
                    writeAction(writer);
                }
            }
        }

        /// <summary>
        /// Add a file to the archive.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to add the file to.</param>
        /// <param name="filename">The name of the file to add.</param>
        public void AddFile(Stream zipArchive, string filename)
        {
            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Create, true))
            {
                // Add the file to the archive.
                archive.CreateEntryFromFile(filename, GetRelativePath(filename) + Path.GetFileName(filename));
            }
        }

        /// <summary>
        /// Add files to the archive.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to add the files to.</param>
        /// <param name="filenames">The name of the files to add.</param>
        public void AddFiles(Stream zipArchive, string[] filenames)
        {
            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Create, true))
            {
                // For each file add to the archive.
                foreach (string file in filenames)
                {
                    // Add the file to the archive.
                    archive.CreateEntryFromFile(file, GetRelativePath(file) + Path.GetFileName(file));
                }
            }
        }

        /// <summary>
        /// Add text to the archive.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to add the text to.</param>
        /// <param name="text">The text to write.</param>
        /// <param name="entryName">The entry name.</param>
        public void AddText(Stream zipArchive, string text, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Create, true))
            {
                // Create an empty entry.
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                // Create the write stream of the entry.
                using (StreamWriter writer = new StreamWriter(entry.Open()))
                {
                    // Write the text.
                    writer.Write(text);
                }
            }
        }

        /// <summary>
        /// Add text to the archive.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to add the text to.</param>
        /// <param name="writeAction">The action used to write the text.</param>
        /// <param name="entryName">The entry name.</param>
        public void AddText(Stream zipArchive, Action<StreamWriter> writeAction, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Create, true))
            {
                // Create an empty entry.
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                // Create the write stream of the entry.
                using (StreamWriter writer = new StreamWriter(entry.Open()))
                {
                    // Write the text from the action handler.
                    writeAction(writer);
                }
            }
        }

        /// <summary>
        /// Add data to the archive.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to add the data to.</param>
        /// <param name="data">The data to write.</param>
        /// <param name="entryName">The entry name.</param>
        public void AddData(Stream zipArchive, byte[] data, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Create, true))
            {
                // Create an empty entry.
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                // Create the write stream of the entry.
                using (BinaryWriter writer = new BinaryWriter(entry.Open()))
                {
                    // Write the text.
                    writer.Write(data);
                }
            }
        }

        /// <summary>
        /// Add data to the archive.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to add the data to.</param>
        /// <param name="writeAction">The action used to write the data.</param>
        /// <param name="entryName">The entry name.</param>
        public void AddData(Stream zipArchive, Action<BinaryWriter> writeAction, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Create, true))
            {
                // Create an empty entry.
                ZipArchiveEntry entry = archive.CreateEntry(entryName);

                // Create the write stream of the entry.
                using (BinaryWriter writer = new BinaryWriter(entry.Open()))
                {
                    // Write the text from the action handler.
                    writeAction(writer);
                }
            }
        }

        /// <summary>
        /// Get the list of entry names within the archive.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to get data from.</param>
        /// <returns>The list of entry names within the archive.</returns>
        public List<string> GetEntryNames(string zipFilename)
        {
            List<string> entries = new List<string>();

            // Open the archive.
            using (ZipArchive archive = ZipFile.OpenRead(zipFilename))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Get the name of the entry.
                    entries.Add(entry.FullName);
                }
            }

            // Return all the entry names.
            return entries;
        }

        /// <summary>
        /// Get the list of entry names within the archive.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to get data from.</param>
        /// <returns>The list of entry names within the archive.</returns>
        public List<string> GetEntryNames(Stream zipArchive)
        {
            List<string> entries = new List<string>();

            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Read, true))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Get the name of the entry.
                    entries.Add(entry.FullName);
                }
            }

            // Return all the entry names.
            return entries;
        }

        /// <summary>
        /// Get the entry.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to get data from.</param>
        /// <param name="entryName">The entry name to search for.</param>
        /// <returns>The archive entry.</returns>
        public ArchiveEntry GetEntry(string zipFilename, string entryName)
        {
            ArchiveEntry archiveEntry = new ArchiveEntry();

            // Open the archive.
            using (ZipArchive archive = ZipFile.OpenRead(zipFilename))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // If the entry has been found.
                    if (entry.FullName.ToLower() == entryName.ToLower())
                    {
                        // Get the entry details.
                        archiveEntry.Name = entry.Name;
                        archiveEntry.Length = entry.Length;
                        archiveEntry.FullName = entry.FullName;
                        archiveEntry.LastWriteTime = entry.LastWriteTime;
                        archiveEntry.CompressionLength = entry.CompressedLength;

                        // Get the entry stream.
                        archiveEntry.Stream = entry.Open();
                        break;
                    }
                }
            }

            // Return the archive stream.
            return archiveEntry;
        }

        /// <summary>
        /// Get the entry.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to get data from.</param>
        /// <param name="entryName">The entry name to search for.</param>
        /// <returns>The archive entry.</returns>
        public ArchiveEntry GetEntry(Stream zipArchive, string entryName)
        {
            ArchiveEntry archiveEntry = new ArchiveEntry();

            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Read, true))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // If the entry has been found.
                    if (entry.FullName.ToLower() == entryName.ToLower())
                    {
                        // Get the entry details.
                        archiveEntry.Name = entry.Name;
                        archiveEntry.Length = entry.Length;
                        archiveEntry.FullName = entry.FullName;
                        archiveEntry.LastWriteTime = entry.LastWriteTime;
                        archiveEntry.CompressionLength = entry.CompressedLength;

                        // Get the entry stream.
                        archiveEntry.Stream = entry.Open();
                        break;
                    }
                }
            }

            // Return the archive stream.
            return archiveEntry;
        }

        /// <summary>
        /// Get the entries.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to get data from.</param>
        /// <param name="entryNames">The entry names to search for.</param>
        /// <returns>The archive entry collection.</returns>
        public List<ArchiveEntry> GetEntries(string zipFilename, string[] entryNames)
        {
            List<ArchiveEntry> archiveEntries = new List<ArchiveEntry>();

            // Open the archive.
            using (ZipArchive archive = ZipFile.OpenRead(zipFilename))
            {
                // For each entry to find.
                foreach (string name in entryNames)
                {
                    // For each entry.
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // If the entry has been found.
                        if (entry.FullName.ToLower() == name.ToLower())
                        {
                            // Create a new archive entry.
                            ArchiveEntry archiveEntry = new ArchiveEntry();

                            // Get the entry details.
                            archiveEntry.Name = entry.Name;
                            archiveEntry.Length = entry.Length;
                            archiveEntry.FullName = entry.FullName;
                            archiveEntry.LastWriteTime = entry.LastWriteTime;
                            archiveEntry.CompressionLength = entry.CompressedLength;

                            // Get the entry stream.
                            archiveEntry.Stream = entry.Open();

                            // Add the entry.
                            archiveEntries.Add(archiveEntry);
                            break;
                        }
                    }
                }
            }

            // Return the archive streams.
            return archiveEntries;
        }

        /// <summary>
        /// Get the entries.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to get data from.</param>
        /// <param name="entryNames">The entry names to search for.</param>
        /// <returns>The archive entry collection.</returns>
        public List<ArchiveEntry> GetEntries(Stream zipArchive, string[] entryNames)
        {
            List<ArchiveEntry> archiveEntries = new List<ArchiveEntry>();

            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Read, true))
            {
                // For each entry to find.
                foreach (string name in entryNames)
                {
                    // For each entry.
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // If the entry has been found.
                        if (entry.FullName.ToLower() == name.ToLower())
                        {
                            // Create a new archive entry.
                            ArchiveEntry archiveEntry = new ArchiveEntry();

                            // Get the entry details.
                            archiveEntry.Name = entry.Name;
                            archiveEntry.Length = entry.Length;
                            archiveEntry.FullName = entry.FullName;
                            archiveEntry.LastWriteTime = entry.LastWriteTime;
                            archiveEntry.CompressionLength = entry.CompressedLength;

                            // Get the entry stream.
                            archiveEntry.Stream = entry.Open();

                            // Add the entry.
                            archiveEntries.Add(archiveEntry);
                            break;
                        }
                    }
                }
            }

            // Return the archive streams.
            return archiveEntries;
        }

        /// <summary>
        /// Get all the entries.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to get data from.</param>
        /// <returns>The archive entry collection.</returns>
        public List<ArchiveEntry> GetEntries(string zipFilename)
        {
            List<ArchiveEntry> archiveEntries = new List<ArchiveEntry>();

            // Open the archive.
            using (ZipArchive archive = ZipFile.OpenRead(zipFilename))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Create a new archive entry.
                    ArchiveEntry archiveEntry = new ArchiveEntry();

                    // Get the entry details.
                    archiveEntry.Name = entry.Name;
                    archiveEntry.Length = entry.Length;
                    archiveEntry.FullName = entry.FullName;
                    archiveEntry.LastWriteTime = entry.LastWriteTime;
                    archiveEntry.CompressionLength = entry.CompressedLength;

                    // Get the entry stream.
                    archiveEntry.Stream = entry.Open();

                    // Add the entry.
                    archiveEntries.Add(archiveEntry);
                }
            }

            // Return the archive streams.
            return archiveEntries;
        }

        /// <summary>
        /// Get all the entries.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to get data from.</param>
        /// <returns>The archive entry collection.</returns>
        public List<ArchiveEntry> GetEntries(Stream zipArchive)
        {
            List<ArchiveEntry> archiveEntries = new List<ArchiveEntry>();

            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Read, true))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Create a new archive entry.
                    ArchiveEntry archiveEntry = new ArchiveEntry();

                    // Get the entry details.
                    archiveEntry.Name = entry.Name;
                    archiveEntry.Length = entry.Length;
                    archiveEntry.FullName = entry.FullName;
                    archiveEntry.LastWriteTime = entry.LastWriteTime;
                    archiveEntry.CompressionLength = entry.CompressedLength;

                    // Get the entry stream.
                    archiveEntry.Stream = entry.Open();

                    // Add the entry.
                    archiveEntries.Add(archiveEntry);
                }
            }

            // Return the archive streams.
            return archiveEntries;
        }

        /// <summary>
        /// Delete the entry.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to delete data from.</param>
        /// <param name="entryName">The entry name to search for.</param>
        public void DeleteEntry(string zipFilename, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Update))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // If the entry has been found.
                    if (entry.FullName.ToLower() == entryName.ToLower())
                    {
                        // Delete the entry from the archive.
                        entry.Delete();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Delete the entry.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to delete data from.</param>
        /// <param name="entryName">The entry name to search for.</param>
        public void DeleteEntry(Stream zipArchive, string entryName)
        {
            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Update, true))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // If the entry has been found.
                    if (entry.FullName.ToLower() == entryName.ToLower())
                    {
                        // Delete the entry from the archive.
                        entry.Delete();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Delete the entries.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to delete data from.</param>
        /// <param name="entryNames">The entry names to search for.</param>
        public void DeleteEntries(string zipFilename, string[] entryNames)
        {
            // Open the archive.
            using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Update))
            {
                // For each entry to find.
                foreach (string name in entryNames)
                {
                    // For each entry.
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // If the entry has been found.
                        if (entry.FullName.ToLower() == name.ToLower())
                        {
                            // Delete the entry from the archive.
                            entry.Delete();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete the entries.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to delete data from.</param>
        /// <param name="entryNames">The entry names to search for.</param>
        public void DeleteEntries(Stream zipArchive, string[] entryNames)
        {
            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Update, true))
            {
                // For each entry to find.
                foreach (string name in entryNames)
                {
                    // For each entry.
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // If the entry has been found.
                        if (entry.FullName.ToLower() == name.ToLower())
                        {
                            // Delete the entry from the archive.
                            entry.Delete();
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete all the entries.
        /// </summary>
        /// <param name="zipFilename">The archived zip filename to delete data from.</param>
        public void DeleteEntries(string zipFilename)
        {
            // Open the archive.
            using (ZipArchive archive = ZipFile.Open(zipFilename, ZipArchiveMode.Update))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Delete the entry from the archive.
                    entry.Delete();
                }
            }
        }

        /// <summary>
        /// Delete all the entries.
        /// </summary>
        /// <param name="zipArchive">The archived zip stream to delete data from.</param>
        public void DeleteEntries(Stream zipArchive)
        {
            // Open the archive.
            using (ZipArchive archive = new ZipArchive(zipArchive, ZipArchiveMode.Update, true))
            {
                // For each entry.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Delete the entry from the archive.
                    entry.Delete();
                }
            }
        }
    }
}
