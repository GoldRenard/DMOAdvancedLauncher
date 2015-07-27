using DMOLibrary.Profiles;

namespace AdvancedLauncher.Management.Interfaces {

    public interface IGameConfiguration {

        GameManager.GameType GameType {
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

        string VarsionLocalPath {
            get;
        }

        string VersionRemoteURL {
            get;
        }

        string PatchRemoteURL {
            get;
        }

        DMOProfile Profile {
            get;
        }

        string GetGamePathFromRegistry();

        string GetLauncherPathFromRegistry();
    }
}