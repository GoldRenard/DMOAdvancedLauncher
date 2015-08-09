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

        /// <summary>
        /// Fires on status changing.
        /// </summary>
        /// <seealso cref="WriteStatusChangedEventHandler"/>
        event WriteStatusChangedEventHandler WriteStatusChanged;

        /// <summary>
        /// Is game archives was opened
        /// </summary>
        bool IsOpened {
            get;
        }

        /// <summary>
        /// Opens game archives
        /// </summary>
        /// <param name="access">File access mode.</param>
        /// <param name="archiveHeader">Archives header number. Usually it is 16 for common game archive</param>
        /// <param name="headerFile">Header file path</param>
        /// <param name="packageFile">Packages file path</param>
        /// <returns></returns>
        bool Open(FileAccess access, int archiveHeader, string headerFile, string packageFile);

        /// <summary>
        /// Close game archive
        /// </summary>
        void Close();

        /// <summary>
        /// Read file by name
        /// </summary>
        /// <param name="name">File name</param>
        /// <returns>File stream</returns>
        Stream ReadFile(string name);

        /// <summary>
        /// Read file by id
        /// </summary>
        /// <param name="id">File id</param>
        /// <returns>File stream</returns>
        Stream ReadFile(uint id);

        /// <summary>
        /// Writes file to archive.
        /// </summary>
        /// <param name="sourceFile">Source file (physical on the disk)</param>
        /// <param name="destination">Destination file (internal file path inside archive)</param>
        /// <returns><b>True</b> on success</returns>
        bool WriteFile(string sourceFile, string destination);

        /// <summary>
        /// Writes stream to archive.
        /// </summary>
        /// <param name="sourceStream">Source stream</param>
        /// <param name="destination">Destination file (internal file path inside archive)</param>
        /// <returns><b>True</b> on success</returns>
        bool WriteStream(Stream sourceStream, string destination);

        /// <summary>
        /// Writes stream to archive.
        /// </summary>
        /// <param name="sourceStream">Source stream</param>
        /// <param name="entryId">ID of destination file</param>
        /// <returns><b>True</b> on success</returns>
        bool WriteStream(Stream sourceStream, uint entryId);

        /// <summary>
        /// Writes the full directory content into game archive
        /// </summary>
        /// <param name="path">Directory path</param>
        /// <param name="deleteOnComplete">Set <b>True</b> of you want delete this directory awter write.</param>
        /// <returns><b>True</b> on success</returns>
        bool WriteDirectory(string path, bool deleteOnComplete);

        /// <summary>
        /// Calculates file hash (internal file ID).
        /// Is is hash function over the game path (not the file itself).
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Internal file ID</returns>
        uint FileHash(string filePath);
    }
}