// ======================================================================
// DMOLibrary
// Copyright (C) 2013 Ilya Egorov (goldrenard@gmail.com)

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
using System.Text;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace DMOLibrary.DMOFileSystem {
    public class DMOFileEntry {
        public uint fileSize_cur;
        public uint fileSize_full;
        public uint fileId;
        public long fileOffset;
    }

    public class DMOFileSystem {
        int ArchiveHeader = 0;
        string HF_File, PF_File;
        public List<DMOFileEntry> ArchiveEntries = new List<DMOFileEntry>();

        bool IsOpened = false;

        BinaryWriter MapWriter;
        FileStream ArchiveStream;
        FileAccess fAccess;

        System.Windows.Threading.Dispatcher owner_dispatcher;

        #region EVENTS
        public delegate void WriteDirectoryStatusChange(object sender, int file_num, int file_count);
        public event WriteDirectoryStatusChange WriteStatusChanged;
        protected virtual void OnFileWrited(int file_num, int file_count) {
            if (WriteStatusChanged != null) {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess()) {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new WriteDirectoryStatusChange((sender, num, cnt) => {
                        WriteStatusChanged(sender, num, cnt);
                    }), this, file_num, file_count);
                } else
                    WriteStatusChanged(this, file_num, file_count);
            }
        }
        #endregion

        static string ERROR_CANT_WRITE_FILEMAP = "Can't write filemap. \"{0}\".";
        static string ERROR_CANT_WRITE_FILE = "Can't write file. \"{0}\".";

        public DMOFileSystem() {

        }

        public DMOFileSystem(System.Windows.Threading.Dispatcher owner_dispatcher) {
            this.owner_dispatcher = owner_dispatcher;
        }


        public bool Open(FileAccess fAccess, int ArchiveHeader, string HF_File, string PF_File) {
            if (IsOpened)
                return false;

            if (!File.Exists(HF_File) || !File.Exists(PF_File))
                throw new FileNotFoundException();
            if (IsFileLocked(HF_File) || IsFileLocked(PF_File))
                throw new UnauthorizedAccessException();

            this.fAccess = fAccess;
            this.ArchiveHeader = ArchiveHeader;
            this.HF_File = HF_File;
            this.PF_File = PF_File;
            this.ArchiveEntries.Clear();

            BinaryReader binr = null;
            try { binr = new BinaryReader(File.OpenRead(HF_File), Encoding.Default); } catch { if (binr != null) binr.Close(); throw; }

            if (binr.ReadUInt32() != ArchiveHeader)
                throw new FileFormatException();

            uint entryCount = binr.ReadUInt32();
            DMOFileEntry entry;

            for (uint e = 0; e < entryCount; e++) {
                entry = new DMOFileEntry();
                if (binr.ReadUInt32() != 1)
                    throw new FileFormatException();

                entry.fileSize_cur = binr.ReadUInt32();
                entry.fileSize_full = binr.ReadUInt32();
                entry.fileId = binr.ReadUInt32();
                entry.fileOffset = binr.ReadInt64();

                ArchiveEntries.Add(entry);
            }
            binr.Close();
            binr.Dispose();

            if (fAccess == FileAccess.ReadWrite || fAccess == FileAccess.Write) {
                try {
                    MapWriter = new BinaryWriter(File.Open(HF_File, FileMode.OpenOrCreate, fAccess, FileShare.None));
                    ArchiveStream = File.Open(PF_File, FileMode.OpenOrCreate, fAccess, FileShare.None);
                } catch { throw; }
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

        public Stream ReadFile(string file) {
            return ReadFile(GetEntryIndex(FileHash(file)));
        }

        public Stream ReadFile(uint ID) {
            return ReadFile(GetEntryIndex(ID));
        }

        public Stream ReadFile(int entry_index) {
            if (!IsOpened && !(fAccess == FileAccess.Read || fAccess == FileAccess.ReadWrite))
                return null;

            if (entry_index < 0)
                return null;

            string map_name = Path.GetFileNameWithoutExtension(PF_File);
            MemoryMappedFile mmf;

            try { mmf = MemoryMappedFile.OpenExisting(map_name); } //check if exists
            catch {
                try { mmf = MemoryMappedFile.CreateFromFile(PF_File, FileMode.Open, map_name, 0, MemoryMappedFileAccess.Read); } catch { return null; }
            } // or not - open*/

            MemoryMappedViewStream outStream;
            try { outStream = mmf.CreateViewStream(ArchiveEntries[entry_index].fileOffset, ArchiveEntries[entry_index].fileSize_cur, MemoryMappedFileAccess.Read); } catch { return null; }

            return outStream;
        }
        #endregion

        #region Write Section
        public bool WriteMapFile() {
            if (!IsOpened && !(fAccess == FileAccess.ReadWrite || fAccess == FileAccess.Write))
                return false;

            ArchiveEntries.Sort((s1, s2) => s1.fileId.CompareTo(s2.fileId));

            try {
                MapWriter.BaseStream.SetLength(0);
                MapWriter.Seek(0, SeekOrigin.Begin);

                MapWriter.Write(ArchiveHeader);
                MapWriter.Write((uint)ArchiveEntries.Count);
                foreach (DMOFileEntry entry in ArchiveEntries) {
                    MapWriter.Write(1);
                    MapWriter.Write(entry.fileSize_cur);
                    MapWriter.Write(entry.fileSize_full);
                    MapWriter.Write(entry.fileId);
                    MapWriter.Write(entry.fileOffset);
                }
            } catch (Exception ex) {
                System.Windows.MessageBox.Show(string.Format(ERROR_CANT_WRITE_FILEMAP, ex.Message), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        public bool WriteFile(string source_file, string destination) {
            if (!File.Exists(source_file))
                return false;
            FileStream source_stream = new FileStream(source_file, FileMode.Open, FileAccess.Read);
            return WriteStream(source_stream, destination);
        }

        public bool WriteStream(Stream source_stream, string destination) {
            return WriteStream(source_stream, FileHash(destination));
        }

        public bool WriteStream(Stream source_stream, uint entryId) {
            List<DMOFileEntry> ArchiveEntries_New = ArchiveEntries;
            if (!_WriteStream(source_stream, entryId))
                return false;
            if (!WriteMapFile())
                return false;
            return true;
        }

        private bool _WriteStream(Stream SourceStream, uint entryId) {
            if (!IsOpened && !(fAccess == FileAccess.ReadWrite || fAccess == FileAccess.Write))
                return false;

            int entry_index = GetEntryIndex(entryId);
            DMOFileEntry entry;

            if (entry_index > 0) {
                ArchiveEntries[entry_index].fileSize_cur = (uint)SourceStream.Length;
                if (SourceStream.Length > ArchiveEntries[entry_index].fileSize_full) {
                    ArchiveEntries[entry_index].fileOffset = ArchiveStream.Length;
                    ArchiveEntries[entry_index].fileSize_full = (uint)SourceStream.Length;
                }
                entry = ArchiveEntries[entry_index];
            } else {
                entry = new DMOFileEntry();
                entry.fileId = entryId;
                entry.fileSize_cur = (uint)SourceStream.Length;
                entry.fileSize_full = (uint)SourceStream.Length;
                entry.fileOffset = ArchiveStream.Length;
                ArchiveEntries.Add(entry);
            }
            try {
                ArchiveStream.Seek(entry.fileOffset, SeekOrigin.Begin);
                SourceStream.CopyTo(ArchiveStream);
            } catch (Exception ex) {
                System.Windows.MessageBox.Show(string.Format(ERROR_CANT_WRITE_FILE, ex.Message), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        public bool WriteDirectory(string path, bool DeleteOnComplete) {
            if (!IsOpened && !(fAccess == FileAccess.ReadWrite || fAccess == FileAccess.Write))
                return false;
            if (!Directory.Exists(path))
                return false;
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            string import_file;
            int f_num = 1;

            foreach (string file in files) {
                import_file = file.Replace(path, string.Empty);
                if (import_file[0] == '\\')
                    import_file = import_file.Substring(1);

                FileStream fStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                if (!_WriteStream(fStream, FileHash(import_file)))
                    return false;
                fStream.Close();
                fStream.Dispose();

                OnFileWrited(f_num, files.Length);
                f_num++;
            }

            if (!WriteMapFile())
                return false;

            if (DeleteOnComplete) {
                try { Directory.Delete(path, true); } catch { }
            }
            return true;
        }
        #endregion

        #region Tools
        public static uint FileHash(string file_path) {
            uint result = 5381;
            int HASH_TRANS_SIZE = 0x400, char_index = 0, len;
            byte char_code;

            len = file_path.Length;
            if (len >= HASH_TRANS_SIZE)
                return 0;
            file_path = file_path.ToLower();

            if (len > 0) {
                while (char_index < len) {
                    char_code = (byte)file_path[char_index];
                    if (char_code != 46 && char_code != 92)
                        result = char_code + 33 * result;
                    ++char_index;
                }
            }

            return result;
        }

        /// <summary> Checks access to file </summary>
        /// <param name="file">Full path to file</param>
        /// <returns> <see langword="True"/> if file is locked </returns>
        public static bool IsFileLocked(string file) {
            FileStream stream = null;

            try { stream = File.Open(file, FileMode.Open, FileAccess.ReadWrite, FileShare.None); } catch (IOException) { return true; } finally {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

        private uint ConvertToUint32(long l) {
            while (l > UInt32.MaxValue)
                l -= UInt32.MaxValue;
            return Convert.ToUInt32(l);
        }

        private int GetEntryIndex(uint file_id) {
            int entry_index = -1;
            entry_index = ArchiveEntries.FindIndex(delegate(DMOFileEntry bk) { return bk.fileId == file_id; });
            return entry_index;
        }
        #endregion
    }
}
