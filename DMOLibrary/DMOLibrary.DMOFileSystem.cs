// ======================================================================
// DMOLibrary
// Copyright (C) 2014 Ilya Egorov (goldrenard@gmail.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Windows.Threading;

namespace DMOLibrary.DMOFileSystem {

    public class DMOFileSystem {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(DMOFileSystem));

        public class DMOFileEntry {
            public uint Id;
            public long Offset;
            public uint SizeCurrent;
            public uint SizeAvailable;

            public override string ToString() {
                return String.Format("DMOFileEntry - [Id={0}, Offset={1}, SizeCurrent={2}, SizeAvailable={3}",
                    Id, Offset, SizeCurrent, SizeAvailable);
            }
        }

        private int ArchiveHeader = 0;
        private string HeaderFile, PackageFile;
        public List<DMOFileEntry> ArchiveEntries = new List<DMOFileEntry>();

        private bool IsOpened = false;

        private BinaryWriter MapWriter;
        private FileStream ArchiveStream;
        private FileAccess Access;

        private Dispatcher OwnerDispatcher;

        #region EVENTS

        public delegate void WriteDirectoryStatusChange(object sender, int fileNum, int fileCount);

        public event WriteDirectoryStatusChange WriteStatusChanged;

        protected virtual void OnFileWrited(int fileNum, int fileCount) {
            LOGGER.DebugFormat("OnFileWrited: fileNum={0}, fileCount={1}", fileNum, fileCount);
            if (WriteStatusChanged != null) {
                if (OwnerDispatcher != null && !OwnerDispatcher.CheckAccess()) {
                    OwnerDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new WriteDirectoryStatusChange((sender, num, cnt) => {
                        WriteStatusChanged(sender, num, cnt);
                    }), this, fileNum, fileCount);
                } else
                    WriteStatusChanged(this, fileNum, fileCount);
            }
        }

        #endregion EVENTS

        private static string ERROR_CANT_WRITE_FILEMAP = "Can't write filemap. \"{0}\".";
        private static string ERROR_CANT_WRITE_FILE = "Can't write file. \"{0}\".";

        public DMOFileSystem() {
        }

        public DMOFileSystem(Dispatcher OwnerDispatcher) {
            this.OwnerDispatcher = OwnerDispatcher;
        }

        public bool Open(FileAccess access, int archiveHeader, string headerFile, string packageFile) {
            LOGGER.DebugFormat("Opening FileSystem: access={0}, archiveHeader={1}, headerFile={2}, packageFile={3}",
                access, archiveHeader, headerFile, packageFile);
            if (IsOpened) {
                return false;
            }

            if (!File.Exists(headerFile) || !File.Exists(packageFile)) {
                LOGGER.Error("FileSystem open failed (FileNotFoundException)");
                throw new FileNotFoundException();
            }
            if (IsFileLocked(headerFile) || IsFileLocked(packageFile)) {
                LOGGER.Error("FileSystem open failed (UnauthorizedAccessException)");
                throw new UnauthorizedAccessException();
            }

            this.Access = access;
            this.ArchiveHeader = archiveHeader;
            this.HeaderFile = headerFile;
            this.PackageFile = packageFile;
            this.ArchiveEntries.Clear();

            BinaryReader binr = null;
            try {
                binr = new BinaryReader(File.OpenRead(headerFile), Encoding.Default);
            } catch {
                if (binr != null) binr.Close(); throw;
            }

            if (binr.ReadUInt32() != archiveHeader) {
                LOGGER.Error("FileSystem open failed (FileFormatException)");
                throw new FileFormatException();
            }

            uint entryCount = binr.ReadUInt32();
            DMOFileEntry entry;

            for (uint e = 0; e < entryCount; e++) {
                entry = new DMOFileEntry();
                if (binr.ReadUInt32() != 1) {
                    LOGGER.Error("FileSystem open failed (FileFormatException)");
                    throw new FileFormatException();
                }

                entry.SizeCurrent = binr.ReadUInt32();
                entry.SizeAvailable = binr.ReadUInt32();
                entry.Id = binr.ReadUInt32();
                entry.Offset = binr.ReadInt64();

                ArchiveEntries.Add(entry);
            }
            binr.Close();
            binr.Dispose();

            if (access == FileAccess.ReadWrite || access == FileAccess.Write) {
                try {
                    MapWriter = new BinaryWriter(File.Open(HeaderFile, FileMode.OpenOrCreate, access, FileShare.None));
                    ArchiveStream = File.Open(PackageFile, FileMode.OpenOrCreate, access, FileShare.None);
                } catch {
                    throw;
                }
            }

            IsOpened = true;
            return true;
        }

        public void Close() {
            if (IsOpened) {
                if (MapWriter != null) {
                    MapWriter.Close();
                    MapWriter.Dispose();
                    MapWriter = null;
                }
                if (ArchiveStream != null) {
                    ArchiveStream.Close();
                    ArchiveStream.Dispose();
                    ArchiveStream = null;
                }
                IsOpened = false;
            }
        }

        #region Read Section

        public Stream ReadFile(string name) {
            LOGGER.DebugFormat("Reading file: name=\"{0}\"", name);
            return ReadFile(GetEntryIndex(FileHash(name)));
        }

        public Stream ReadFile(uint id) {
            LOGGER.DebugFormat("Reading file: id={0}", id);
            return ReadFile(GetEntryIndex(id));
        }

        public Stream ReadFile(int entryIndex) {
            LOGGER.DebugFormat("Reading file: entryIndex={0}", entryIndex);
            if (!IsOpened && !(Access == FileAccess.Read || Access == FileAccess.ReadWrite)) {
                LOGGER.Error("Reading file failed: Archieve not opened or no read access");
                return null;
            }

            if (entryIndex < 0) {
                LOGGER.ErrorFormat("Reading file failed: Wrong entryIndex={0}", entryIndex);
                return null;
            }

            string mapName = Path.GetFileNameWithoutExtension(PackageFile);
            MemoryMappedFile mmf;

            try {
                mmf = MemoryMappedFile.OpenExisting(mapName);
            } //check if exists
            catch {
                try {
                    mmf = MemoryMappedFile.CreateFromFile(PackageFile, FileMode.Open, mapName, 0, MemoryMappedFileAccess.Read);
                } catch {
                    LOGGER.Error("Reading file failed: Unable to create MemoryMappedFile");
                    return null;
                }
            } // or not - open*/

            MemoryMappedViewStream outStream;
            try {
                outStream = mmf.CreateViewStream(ArchiveEntries[entryIndex].Offset, ArchiveEntries[entryIndex].SizeCurrent, MemoryMappedFileAccess.Read);
            } catch {
                LOGGER.Error("Reading file failed: Unable to create MemoryMappedViewStream");
                return null;
            }

            return outStream;
        }

        #endregion Read Section

        #region Write Section

        public bool WriteMapFile() {
            LOGGER.Debug("Writing map file...");
            if (!IsOpened && !(Access == FileAccess.ReadWrite || Access == FileAccess.Write)) {
                LOGGER.Error("Writing map file failed: Archieve not opened or no write access");
                return false;
            }

            ArchiveEntries.Sort((s1, s2) => s1.Id.CompareTo(s2.Id));

            try {
                MapWriter.BaseStream.SetLength(0);
                MapWriter.Seek(0, SeekOrigin.Begin);

                MapWriter.Write(ArchiveHeader);
                MapWriter.Write((uint)ArchiveEntries.Count);
                foreach (DMOFileEntry entry in ArchiveEntries) {
                    MapWriter.Write(1);
                    MapWriter.Write(entry.SizeCurrent);
                    MapWriter.Write(entry.SizeAvailable);
                    MapWriter.Write(entry.Id);
                    MapWriter.Write(entry.Offset);
                }
            } catch (Exception ex) {
                System.Windows.MessageBox.Show(string.Format(ERROR_CANT_WRITE_FILEMAP, ex.Message), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                LOGGER.Error("Writing map file failed.", ex);
                return false;
            }
            return true;
        }

        public bool WriteFile(string sourceFile, string destination) {
            LOGGER.DebugFormat("Writing file: sourceFile=\"{0}\", destination=\"{1}\"", sourceFile, destination);
            if (!File.Exists(sourceFile)) {
                LOGGER.ErrorFormat("Writing file failed: sourceFile=\"{0}\" not found", sourceFile);
                return false;
            }
            FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
            return WriteStream(sourceStream, destination);
        }

        public bool WriteStream(Stream sourceStream, string destination) {
            LOGGER.DebugFormat("Writing stream: destination=\"{0}\"", destination);
            return WriteStream(sourceStream, FileHash(destination));
        }

        public bool WriteStream(Stream sourceStream, uint entryId) {
            if (!_WriteStream(sourceStream, entryId)) {
                return false;
            }
            if (!WriteMapFile()) {
                return false;
            }
            return true;
        }

        private bool _WriteStream(Stream SourceStream, uint entryId) {
            LOGGER.DebugFormat("Writing stream: entryId=\"{0}\"", entryId);
            if (!IsOpened && !(Access == FileAccess.ReadWrite || Access == FileAccess.Write)) {
                LOGGER.Error("Writing stream failed: Archieve not opened or no write access");
                return false;
            }

            int entryIndex = GetEntryIndex(entryId);
            DMOFileEntry entry;

            if (entryIndex > 0) {
                ArchiveEntries[entryIndex].SizeCurrent = (uint)SourceStream.Length;
                if (SourceStream.Length > ArchiveEntries[entryIndex].SizeAvailable) {
                    ArchiveEntries[entryIndex].Offset = ArchiveStream.Length;
                    ArchiveEntries[entryIndex].SizeAvailable = (uint)SourceStream.Length;
                }
                entry = ArchiveEntries[entryIndex];
            } else {
                entry = new DMOFileEntry();
                entry.Id = entryId;
                entry.SizeCurrent = (uint)SourceStream.Length;
                entry.SizeAvailable = (uint)SourceStream.Length;
                entry.Offset = ArchiveStream.Length;
                ArchiveEntries.Add(entry);
            }
            try {
                ArchiveStream.Seek(entry.Offset, SeekOrigin.Begin);
                SourceStream.CopyTo(ArchiveStream);
            } catch (Exception ex) {
                System.Windows.MessageBox.Show(string.Format(ERROR_CANT_WRITE_FILE, ex.Message), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                LOGGER.Error("Writing stream failed!", ex);
                return false;
            }
            return true;
        }

        public bool WriteDirectory(string path, bool deleteOnComplete) {
            LOGGER.DebugFormat("Writing directory: path=\"{0}\", DeleteOnComplete={1}", path, deleteOnComplete);
            if (!IsOpened && !(Access == FileAccess.ReadWrite || Access == FileAccess.Write)) {
                return false;
            }
            if (!Directory.Exists(path)) {
                return false;
            }
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            string importFile;
            int fNum = 1;

            foreach (string file in files) {
                importFile = file.Replace(path, string.Empty);
                if (importFile[0] == '\\') {
                    importFile = importFile.Substring(1);
                }

                FileStream fStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                if (!_WriteStream(fStream, FileHash(importFile))) {
                    return false;
                }
                fStream.Close();
                fStream.Dispose();

                OnFileWrited(fNum, files.Length);
                fNum++;
            }

            if (!WriteMapFile()) {
                return false;
            }

            if (deleteOnComplete) {
                try {
                    Directory.Delete(path, true);
                } catch {
                }
            }
            return true;
        }

        #endregion Write Section

        #region Tools

        public static uint FileHash(string filePath) {
            uint result = 5381;
            int HASH_TRANS_SIZE = 0x400, charIndex = 0, len;
            byte charCode;

            len = filePath.Length;
            if (len >= HASH_TRANS_SIZE) {
                return 0;
            }
            filePath = filePath.ToLower();

            if (len > 0) {
                while (charIndex < len) {
                    charCode = (byte)filePath[charIndex];
                    if (charCode != 46 && charCode != 92) {
                        result = charCode + 33 * result;
                    }
                    ++charIndex;
                }
            }

            return result;
        }

        /// <summary> Checks access to file </summary>
        /// <param name="file">Full path to file</param>
        /// <returns> <see langword="True"/> if file is locked </returns>
        public static bool IsFileLocked(string file) {
            FileStream stream = null;

            try {
                stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            } catch (IOException) {
                return true;
            } finally {
                if (stream != null) {
                    stream.Close();
                }
            }
            return false;
        }

        private uint ConvertToUint32(long l) {
            while (l > UInt32.MaxValue) {
                l -= UInt32.MaxValue;
            }
            return Convert.ToUInt32(l);
        }

        private int GetEntryIndex(uint fileId) {
            int entryIndex = -1;
            entryIndex = ArchiveEntries.FindIndex(delegate(DMOFileEntry bk) {
                return bk.Id == fileId;
            });
            return entryIndex;
        }

        #endregion Tools
    }
}