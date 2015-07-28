namespace AdvancedLauncher.Model.Config {

    public class VersionPair {

        public int Local {
            get;
            private set;
        }

        public int Remote {
            get;
            private set;
        }

        public bool IsUpdateRequired {
            get {
                return Remote > Local;
            }
        }

        public VersionPair(int Local, int Remote) {
            this.Local = Local;
            this.Remote = Remote;
        }
    }
}