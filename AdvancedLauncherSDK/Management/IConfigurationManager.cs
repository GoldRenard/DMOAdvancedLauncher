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

using System.Collections;
using System.Collections.Generic;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Config;

namespace AdvancedLauncher.SDK.Management {

    public interface IConfigurationManager : IManager, IEnumerable, IEnumerable<IConfiguration> {

        bool CheckGame(IGameModel model);

        bool CheckLauncher(IGameModel model);

        bool CheckUpdateAccess(IGameModel model);

        string GetImportPath(IGameModel model);

        string GetLocalVersionFile(IGameModel model);

        string GetPFPath(IGameModel model);

        string GetHFPath(IGameModel model);

        string GetGameEXE(IGameModel model);

        string GetLauncherEXE(IGameModel model);

        string GetLauncherPath(IGameModel model);

        string GetGamePath(IGameModel model);

        IConfiguration GetConfiguration(IGameModel model);

        IConfiguration GetConfiguration(string gameType);

        bool RegisterConfiguration(IConfiguration configuration);

        bool UnRegisterConfiguration(string name);

        void UpdateRegistryPaths(IGameModel model);
    }
}