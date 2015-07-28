namespace AdvancedLauncher.SDK.Model.Config {

    public interface IGameModel {

        string Type {
            get;
            set;
        }

        string GamePath {
            get;
            set;
        }

        string DefLauncherPath {
            get;
            set;
        }
    }
}