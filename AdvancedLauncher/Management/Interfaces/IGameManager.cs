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