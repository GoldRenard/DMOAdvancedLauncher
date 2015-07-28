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

using System.IO;
using AdvancedLauncher.Model.Config;
using DMOLibrary.DMOFileSystem;

namespace AdvancedLauncher.Management.Interfaces {

    public interface IGameManager : IManager {

        bool CheckGame(GameModel model);

        bool CheckLauncher(GameModel model);

        bool CheckUpdateAccess(GameModel model);

        string GetImportPath(GameModel model);

        string GetLocalVersionFile(GameModel model);

        string GetPFPath(GameModel model);

        string GetHFPath(GameModel model);

        string GetGameEXE(GameModel model);

        string GetLauncherEXE(GameModel model);

        string GetLauncherPath(GameModel model);

        string GetGamePath(GameModel model);

        IGameConfiguration GetConfiguration(GameModel model);

        void UpdateRegistryPaths(GameModel model);

        DMOFileSystem GetFileSystem(GameModel model);

        bool OpenFileSystem(GameModel model, FileAccess fAccess);
    }
}