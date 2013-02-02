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

namespace DMOLibrary.DMOFileSystem
{
    public struct DMOFileEntry
    {
        public uint fileId;
        public uint fileOffset;
        public uint fileSize_cur;
        public uint fileSize_full;
    }

    public class DMOFileSystem
    {
        int ArchiveHeader = 0;
        string HF_File, PF_File;
        public List<DMOFileEntry> ArchiveEntries = new List<DMOFileEntry>();
        System.Windows.Threading.Dispatcher owner_dispatcher;

        #region EVENTS
        public delegate void WriteDirectoryStatusChange(object sender, int file_num, int file_count);
        public event WriteDirectoryStatusChange WriteStatusChanged;
        protected virtual void OnFileWrited(int file_num, int file_count)
        {
            if (WriteStatusChanged != null)
            {
                if (owner_dispatcher != null && !owner_dispatcher.CheckAccess())
                {
                    owner_dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new WriteDirectoryStatusChange((sender, num, cnt) =>
                    {
                        WriteStatusChanged(sender, num, cnt);
                    }), this, file_num, file_count);
                }
                else
                    WriteStatusChanged(this, file_num, file_count);
            }
        }
        #endregion

        static string ERROR_CANT_WRITE_FILEMAP = "Can't write filemap. \"{0}\".";
        static string ERROR_CANT_WRITE_FILE = "Can't write file. \"{0}\".";

        public DMOFileSystem(int ArchiveHeader, string HF_File, string PF_File)
        {
            this.ArchiveHeader = ArchiveHeader;
            this.HF_File = HF_File;
            this.PF_File = PF_File;
            if (File.Exists(HF_File))
                ReadMapFile();
        }

        public DMOFileSystem(System.Windows.Threading.Dispatcher owner_dispatcher, int ArchiveHeader, string HF_File, string PF_File)
        {
            this.owner_dispatcher = owner_dispatcher;
            this.ArchiveHeader = ArchiveHeader;
            this.HF_File = HF_File;
            this.PF_File = PF_File;
            if (File.Exists(HF_File))
                ReadMapFile();
        }

        #region Read Section
        public void ReadMapFile()
        {
            BinaryReader binr = null;
            try { binr = new BinaryReader(File.OpenRead(HF_File), Encoding.Default); }
            catch (Exception ex) { throw ex; }

            if (binr.ReadUInt32() != ArchiveHeader)
                throw new FileFormatException();

            uint entryCount = binr.ReadUInt32();
            DMOFileEntry entry;

            for (uint e = 0; e < entryCount; e++)
            {
                entry = new DMOFileEntry();
                if (binr.ReadUInt32() != 1)
                    throw new FileFormatException();

                entry.fileSize_cur = binr.ReadUInt32();
                entry.fileSize_full = binr.ReadUInt32();
                entry.fileId = binr.ReadUInt32();
                entry.fileOffset = binr.ReadUInt32();
                ArchiveEntries.Add(entry);

                if (binr.ReadUInt32() != 0)
                    throw new FileFormatException();
            }
            binr.Close();
        }

        public Stream ReadFile(string file)
        {
            int entry_index = GetEntryIndex(FileHash(file));
            return ReadFile(entry_index);
        }

        public Stream ReadFile(uint ID)
        {
            int entry_index = GetEntryIndex(ID);
            return ReadFile(entry_index);
        }

        public Stream ReadFile(int entry_index)
        {
            if (entry_index < 0)
                return null;

            string map_name = Path.GetFileNameWithoutExtension(PF_File);
            MemoryMappedFile mmf;

            try { mmf = MemoryMappedFile.OpenExisting(map_name); } //check if exists
            catch {
                try { mmf = MemoryMappedFile.CreateFromFile(PF_File, FileMode.Open, map_name, 0, MemoryMappedFileAccess.Read); }
                catch { return null; }
            } // or not - open*/

            MemoryMappedViewStream outStream;
            try { outStream = mmf.CreateViewStream(ArchiveEntries[entry_index].fileOffset, ArchiveEntries[entry_index].fileSize_cur, MemoryMappedFileAccess.Read); }
            catch { return null; }

            /*FileStream fs = File.OpenRead(PF_File);
            fs.Seek(ArchiveEntries[entry_index].fileOffset, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(fs);

            int b_cnt = (int)ArchiveEntries[entry_index].fileSize_cur;

            byte[] bs = br.ReadBytes(b_cnt);

            MemoryStream ms = new MemoryStream(bs);
            fs.Close();
            br.Close();*/

            return outStream;
        }
        #endregion

        #region Write Section
        public bool WriteMapFile()
        {
            BinaryWriter b = null;
            try
            {
                b = new BinaryWriter(File.Open(HF_File + ".tmp", FileMode.Create));
                b.Write(ArchiveHeader);
                b.Write((uint)ArchiveEntries.Count);
                foreach (DMOFileEntry entry in ArchiveEntries)
                {
                    b.Write(1);
                    b.Write(entry.fileSize_cur);
                    b.Write(entry.fileSize_full);
                    b.Write(entry.fileId);
                    b.Write(entry.fileOffset);
                    b.Write(0);
                }
                b.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format(ERROR_CANT_WRITE_FILEMAP, ex.Message), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
            finally
            {
                if (b != null)
                    b.Close();
            }

            try
            {
                File.Delete(HF_File);
                File.Move(HF_File + ".tmp", HF_File);
            }
            catch { return false; }
            return true;
        }

        public bool WriteFile(string source_file, string destination)
        {
            if (!File.Exists(source_file))
                return false;
            FileStream source_stream = new FileStream(source_file, FileMode.Open, FileAccess.Read);
            return WriteStream(source_stream, destination);
        }

        public bool WriteStream(Stream source_stream, string destination)
        {
            uint entryId = FileHash(destination);
            long pf_file_length = 0;
            if (File.Exists(PF_File))
                pf_file_length = (new FileInfo(PF_File)).Length;
            int entry_index = GetEntryIndex(entryId);

            DMOFileEntry entry;

            bool isAppend = false;
            if (entry_index >= 0)
            {
                entry = ArchiveEntries[entry_index];
                entry.fileSize_cur = (uint)source_stream.Length;
                isAppend = source_stream.Length > ArchiveEntries[entry_index].fileSize_full;
                if (isAppend)
                {
                    entry.fileSize_full = (uint)source_stream.Length;
                    entry.fileOffset = (uint)pf_file_length;
                }
                ArchiveEntries[entry_index] = entry;
            }
            else
            {
                isAppend = true;
                entry = new DMOFileEntry();
                entry.fileId = entryId;
                entry.fileOffset = (uint)pf_file_length;
                entry.fileSize_cur = (uint)source_stream.Length;
                entry.fileSize_full = (uint)source_stream.Length;
                ArchiveEntries.Add(entry);
            }

            if (WriteMapFile())
            {
                FileStream target = null;
                try
                {
                    target = new FileStream(PF_File, isAppend ? FileMode.Append : FileMode.Open, FileAccess.Write);
                    if (!isAppend)
                        target.Seek((long)entry.fileOffset, SeekOrigin.Begin);
                    source_stream.CopyTo(target);
                    target.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(string.Format(ERROR_CANT_WRITE_FILE, ex.Message), "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    source_stream.Close();
                    if (target != null)
                        target.Close();
                    return false;
                }
                source_stream.Close();
                return true;
            }
            else
            {
                source_stream.Close();
                return false;
            }
        }

        public bool WriteDirectory(string path, bool DeleteOnComplete)
        {
            if (!Directory.Exists(path))
                return false;
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            string import_file;
            int f_num = 1;
            foreach (string file in files)
            {
                import_file = file.Replace(path, string.Empty);
                if (import_file[0] == '\\')
                    import_file = import_file.Substring(1);
                if (!WriteFile(file, import_file))
                    return false;
                OnFileWrited(f_num, files.Length);
                f_num++;
            }

            if (DeleteOnComplete)
            {
                try { Directory.Delete(path, true); }
                catch { }
            }
            return true;
        }
        #endregion

        #region Tools
        public static uint FileHash(string file_path)
        {
            uint result = 5381;
            int HASH_TRANS_SIZE = 0x400, char_index = 0, len;
            byte char_code;

            len = file_path.Length;
            if (len >= HASH_TRANS_SIZE)
                return 0;
            file_path = file_path.ToLower();

            if (len > 0)
            {
                while (char_index < len)
                {
                    char_code = (byte)file_path[char_index];
                    if (char_code != 46 && char_code != 92)
                        result = char_code + 33 * result;
                    ++char_index;
                }
            }

            return result;
        }

        private int GetEntryIndex(uint file_id)
        {
            int entry_index = -1;
            entry_index = ArchiveEntries.FindIndex(delegate(DMOFileEntry bk) { return bk.fileId == file_id; });
            return entry_index;
        }
        #endregion
    }
}
