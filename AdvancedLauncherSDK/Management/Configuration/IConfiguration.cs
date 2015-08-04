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

using AdvancedLauncher.SDK.Model.Web;

namespace AdvancedLauncher.SDK.Management.Configuration {

    public interface IConfiguration {

        string Name {
            get;
        }

        string ServerName {
            get;
        }

        string GameType {
            get;
        }

        bool IsLastSessionAvailable {
            get;
        }

        string GamePathRegKey {
            get;
        }

        string GamePathRegVal {
            get;
        }

        string LauncherPathRegKey {
            get;
        }

        string LauncherPathRegVal {
            get;
        }

        string GameExecutable {
            get;
        }

        string LauncherExecutable {
            get;
        }

        string VersionLocalPath {
            get;
        }

        string VersionRemoteURL {
            get;
        }

        string PatchRemoteURL {
            get;
        }

        ILoginProvider CreateLoginProvider();

        IWebProvider CreateWebProvider();

        INewsProvider CreateNewsProvider();

        IServersProvider ServersProvider {
            get;
        }

        bool IsWebAvailable {
            get;
        }

        bool IsNewsAvailable {
            get;
        }

        bool IsLoginRequired {
            get;
        }

        string ConvertGameStartArgs(string args);

        string ConvertLauncherStartArgs(string args);
    }
}