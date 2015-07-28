using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK {
    public interface IFileSystem : IDisposable {

        event WriteStatusChangedEventHandler WriteStatusChanged;

        bool IsOpened {
            get;
        }

        bool Open(FileAccess access, int archiveHeader, string headerFile, string packageFile);

        void Close();

        Stream ReadFile(string name);

        Stream ReadFile(uint id);

        Stream ReadFile(int entryIndex);

        bool WriteFile(string sourceFile, string destination);

        bool WriteStream(Stream sourceStream, string destination);

        bool WriteStream(Stream sourceStream, uint entryId);

        bool WriteDirectory(string path, bool deleteOnComplete);

        uint FileHash(string filePath);
    }
}
