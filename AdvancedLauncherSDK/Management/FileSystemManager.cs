// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2015 Ilya Egorov (goldrenard@gmail.com)

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
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Management {

    public class FileSystemManager : IFileSystemManager {

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
        private List<DMOFileEntry> ArchiveEntries = new List<DMOFileEntry>();

        private bool _IsOpened = false;

        public bool IsOpened {
            get {
                return _IsOpened;
            }
        }

        private BinaryWriter MapWriter;
        private FileStream ArchiveStream;
        private FileAccess Access;

        private Dispatcher OwnerDispatcher;

        private ILogManager LogManager {
            get;
            set;
        }

        #region EVENTS

        public event WriteStatusChangedEventHandler WriteStatusChanged;

        protected virtual void OnFileWrited(int fileNum, int fileCount) {
            if (LogManager != null) {
                LogManager.DebugFormat("OnFileWrited: fileNum={0}, fileCount={1}", fileNum, fileCount);
            }
            WriteDirectoryEventArgs args = new WriteDirectoryEventArgs(fileNum, fileCount);
            if (WriteStatusChanged != null) {
                if (OwnerDispatcher != null && !OwnerDispatcher.CheckAccess()) {
                    OwnerDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new WriteStatusChangedEventHandler((s, e) => {
                        WriteStatusChanged(s, e);
                    }), this, args);
                } else
                    WriteStatusChanged(this, args);
            }
        }

        #endregion EVENTS

        public FileSystemManager() {
        }

        public FileSystemManager(Dispatcher OwnerDispatcher) {
            this.OwnerDispatcher = OwnerDispatcher;
        }

        public void Initialize() {
            // nothing to do here
        }

        public void Initialize(ILogManager logManager) {
            LogManager = logManager;
        }

        public bool Open(FileAccess access, int archiveHeader, string headerFile, string packageFile) {
            if (LogManager != null) {
                LogManager.DebugFormat("Opening FileSystem: access={0}, archiveHeader={1}, headerFile={2}, packageFile={3}",
                    access, archiveHeader, headerFile, packageFile);
            }
            if (_IsOpened) {
                return false;
            }

            if (!File.Exists(headerFile) || !File.Exists(packageFile)) {
                if (LogManager != null) {
                    LogManager.Error("FileSystem open failed (FileNotFoundException)");
                }
                throw new FileNotFoundException();
            }
            if (IsFileLocked(headerFile) || IsFileLocked(packageFile)) {
                if (LogManager != null) {
                    LogManager.Error("FileSystem open failed (UnauthorizedAccessException)");
                }
                throw new UnauthorizedAccessException();
            }

            this.Access = access;
            this.ArchiveHeader = archiveHeader;
            this.HeaderFile = headerFile;
            this.PackageFile = packageFile;
            this.ArchiveEntries.Clear();

            using (BinaryReader reader = new BinaryReader(File.OpenRead(headerFile), Encoding.Default)) {
                if (reader.ReadUInt32() != archiveHeader) {
                    if (LogManager != null) {
                        LogManager.Error("FileSystem open failed (FileFormatException)");
                    }
                    throw new FileFormatException();
                }

                uint entryCount = reader.ReadUInt32();
                DMOFileEntry entry;

                for (uint e = 0; e < entryCount; e++) {
                    entry = new DMOFileEntry();
                    if (reader.ReadUInt32() != 1) {
                        if (LogManager != null) {
                            LogManager.Error("FileSystem open failed (FileFormatException)");
                        }
                        throw new FileFormatException();
                    }

                    entry.SizeCurrent = reader.ReadUInt32();
                    entry.SizeAvailable = reader.ReadUInt32();
                    entry.Id = reader.ReadUInt32();
                    entry.Offset = reader.ReadInt64();

                    ArchiveEntries.Add(entry);
                }
            }

            if (access == FileAccess.ReadWrite || access == FileAccess.Write) {
                try {
                    MapWriter = new BinaryWriter(File.Open(HeaderFile, FileMode.OpenOrCreate, access, FileShare.None));
                    ArchiveStream = File.Open(PackageFile, FileMode.OpenOrCreate, access, FileShare.None);
                } catch {
                    throw;
                }
            }

            _IsOpened = true;
            return true;
        }

        public void Close() {
            this.Dispose();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (_IsOpened && disposing) {
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
                _IsOpened = false;
            }
        }

        #region Read Section

        public Stream ReadFile(string name) {
            if (LogManager != null) {
                LogManager.DebugFormat("Reading file: name=\"{0}\"", name);
            }
            return ReadFile(GetEntryIndex(FileHash(name)));
        }

        public Stream ReadFile(uint id) {
            if (LogManager != null) {
                LogManager.DebugFormat("Reading file: id={0}", id);
            }
            return ReadFile(GetEntryIndex(id));
        }

        public Stream ReadFile(int entryIndex) {
            if (LogManager != null) {
                LogManager.DebugFormat("Reading file: entryIndex={0}", entryIndex);
            }
            if (!_IsOpened && !(Access == FileAccess.Read || Access == FileAccess.ReadWrite)) {
                if (LogManager != null) {
                    LogManager.Error("Reading file failed: Archieve not opened or no read access");
                }
                return null;
            }

            if (entryIndex < 0) {
                if (LogManager != null) {
                    LogManager.ErrorFormat("Reading file failed: Wrong entryIndex={0}", entryIndex);
                }
                return null;
            }

            string mapName = Path.GetFileNameWithoutExtension(PackageFile);
            MemoryMappedFile mmf;

            try {
                mmf = MemoryMappedFile.OpenExisting(mapName);
            } //check if exists
            catch {
                try {
                    mmf = MemoryMappedFile.CreateFromFile(ArchiveStream, mapName, 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, true);
                } catch (Exception e) {
                    if (LogManager != null) {
                        LogManager.Error("Reading file failed: Unable to create MemoryMappedFile", e);
                    }
                    return null;
                }
            } // or not - open*/

            MemoryMappedViewStream outStream;
            try {
                outStream = mmf.CreateViewStream(ArchiveEntries[entryIndex].Offset, ArchiveEntries[entryIndex].SizeCurrent, MemoryMappedFileAccess.Read);
            } catch (Exception e) {
                if (LogManager != null) {
                    LogManager.Error("Reading file failed: Unable to create MemoryMappedViewStream", e);
                }
                return null;
            }

            return outStream;
        }

        #endregion Read Section

        #region Write Section

        private bool WriteMapFile() {
            if (LogManager != null) {
                LogManager.Debug("Writing map file...");
            }
            if (!_IsOpened && !(Access == FileAccess.ReadWrite || Access == FileAccess.Write)) {
                if (LogManager != null) {
                    LogManager.Error("Writing map file failed: Archieve not opened or no write access");
                }
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
                if (LogManager != null) {
                    LogManager.Error("Writing map file failed.", ex);
                }
                throw;
            }
            return true;
        }

        public bool WriteFile(string sourceFile, string destination) {
            if (LogManager != null) {
                LogManager.DebugFormat("Writing file: sourceFile=\"{0}\", destination=\"{1}\"", sourceFile, destination);
            }
            if (!File.Exists(sourceFile)) {
                if (LogManager != null) {
                    LogManager.ErrorFormat("Writing file failed: sourceFile=\"{0}\" not found", sourceFile);
                }
                return false;
            }
            FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
            return WriteStream(sourceStream, destination);
        }

        public bool WriteStream(Stream sourceStream, string destination) {
            if (LogManager != null) {
                LogManager.DebugFormat("Writing stream: destination=\"{0}\"", destination);
            }
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
            if (LogManager != null) {
                LogManager.DebugFormat("Writing stream: entryId=\"{0}\"", entryId);
            }
            if (!_IsOpened && !(Access == FileAccess.ReadWrite || Access == FileAccess.Write)) {
                if (LogManager != null) {
                    LogManager.Error("Writing stream failed: Archieve not opened or no write access");
                }
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
            ArchiveStream.Seek(entry.Offset, SeekOrigin.Begin);
            SourceStream.CopyTo(ArchiveStream);
            return true;
        }

        public bool WriteDirectory(string path, bool deleteOnComplete) {
            if (LogManager != null) {
                LogManager.DebugFormat("Writing directory: path=\"{0}\", DeleteOnComplete={1}", path, deleteOnComplete);
            }
            if (!_IsOpened && !(Access == FileAccess.ReadWrite || Access == FileAccess.Write)) {
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
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                    if (!_WriteStream(fs, FileHash(importFile))) {
                        return false;
                    }
                }
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

        public uint FileHash(string filePath) {
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
            entryIndex = ArchiveEntries.FindIndex(delegate (DMOFileEntry bk) {
                return bk.Id == fileId;
            });
            return entryIndex;
        }

        #endregion Tools
    }
}