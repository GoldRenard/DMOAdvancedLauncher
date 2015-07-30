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
using System.IO;
using AdvancedLauncher.SDK.Model.Events;

namespace AdvancedLauncher.SDK.Management {

    public interface IFileSystemManager : IManager, ILoggable, IDisposable {

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