using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedLauncher.Model.Config;
using DMOLibrary.DMOFileSystem;
using DMOLibrary.Events;

namespace AdvancedLauncher.Management.Interfaces {
    public interface IGameUpdateManager : IManager {
        DMOFileSystem GetFileSystem(GameModel model);

        VersionPair CheckUpdates(GameModel model);

        bool ImportPackages(GameModel model);

        event EventHandler ImportStarted;

        event EventHandler FileSystemOpenError;

        event WriteStatusChangedEventHandler WriteStatusChanged;
    }
}
