using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedLauncher.Management.Execution;
using AdvancedLauncher.Model.Config;

namespace AdvancedLauncher.Management.Interfaces {

    public interface ILauncherManager : IManager, IEnumerable, IEnumerable<ILauncher> {

        ILauncher CurrentLauncher {
            get;
        }

        ILauncher Default {
            get;
        }

        ILauncher GetProfileLauncher(Profile profile);

        ILauncher findByMnemonic(String name);

        T findByType<T>(Type type) where T : ILauncher;
    }
}