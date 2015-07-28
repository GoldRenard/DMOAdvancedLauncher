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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AdvancedLauncher.Management.Execution;
using AdvancedLauncher.Management.Interfaces;
using AdvancedLauncher.Model.Config;
using AdvancedLauncher.SDK.Management.Execution;
using Ninject;

namespace AdvancedLauncher.Management {

    public class LauncherManager : ILauncherManager {
        private readonly Dictionary<String, ILauncher> CollectionByMnemonic = new Dictionary<string, ILauncher>();

        private readonly Dictionary<Type, ILauncher> CollectionByType = new Dictionary<Type, ILauncher>();

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        public void Initialize() {
            foreach (ILauncher launcher in App.Kernel.GetAll<ILauncher>()) {
                CollectionByMnemonic.Add(launcher.Mnemonic, launcher);
                CollectionByType.Add(launcher.GetType(), launcher);
            }
        }

        public ILauncher CurrentLauncher {
            get {
                return GetProfileLauncher(ProfileManager.CurrentProfile);
            }
        }

        public ILauncher GetProfileLauncher(Profile profile) {
            ILauncher launcher = findByMnemonic(profile.LaunchMode);
            if (launcher == null) {
                launcher = Default;
            } else if (!launcher.IsSupported) {
                launcher = Default;
            }
            return launcher;
        }

        public ILauncher Default {
            get {
                var os = System.Environment.OSVersion;

                // first os all, for windows 10 we should use NTLEA as default event if AppLocale doesnt exists
                NTLeaLauncher ntLeaLauncher = findByType<NTLeaLauncher>(typeof(NTLeaLauncher));
                if (ntLeaLauncher != null) {
                    if (os.Platform == PlatformID.Win32NT && os.Version.Major >= 10 && ntLeaLauncher.IsSupported) {
                        return ntLeaLauncher;
                    }
                }

                // second, use AppLocale launcher if it supported
                AppLocaleLauncher alLauncher = findByType<AppLocaleLauncher>(typeof(AppLocaleLauncher));
                if (alLauncher != null) {
                    if (alLauncher.IsSupported) {
                        return alLauncher;
                    }
                }

                // and now if we havent AppLocale, try to use NTLea
                if (ntLeaLauncher != null) {
                    if (ntLeaLauncher.IsSupported) {
                        return ntLeaLauncher;
                    }
                }
                return findByType<DirectLauncher>(typeof(DirectLauncher));
            }
        }

        public ILauncher findByMnemonic(String name) {
            if (name == null) {
                return null;
            }
            ILauncher result;
            if (CollectionByMnemonic.TryGetValue(name, out result)) {
                return result;
            }
            return null;
        }

        public T findByType<T>(Type type) where T : ILauncher {
            ILauncher result = null;
            if (type == null) {
                return (T)result;
            }
            CollectionByType.TryGetValue(type, out result);
            return (T)result;
        }

        public IEnumerator GetEnumerator() {
            return CollectionByMnemonic.Values.OrderBy(x => x.Name).GetEnumerator();
        }

        IEnumerator<ILauncher> IEnumerable<ILauncher>.GetEnumerator() {
            return CollectionByMnemonic.Values.OrderBy(x => x.Name).GetEnumerator();
        }
    }
}