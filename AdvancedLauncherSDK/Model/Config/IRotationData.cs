namespace AdvancedLauncher.SDK.Model.Config {

    public interface IRotationData {

        string Guild {
            set;
            get;
        }

        string Tamer {
            set;
            get;
        }

        byte ServerId {
            set;
            get;
        }

        int UpdateInterval {
            set;
            get;
        }
    }
}