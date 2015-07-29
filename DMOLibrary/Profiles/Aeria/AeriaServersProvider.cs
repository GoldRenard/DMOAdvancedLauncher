using AdvancedLauncher.SDK.Model.Entity;

namespace DMOLibrary.Profiles.Aeria {

    public class AeriaServersProvider : DatabaseServersProvider {

        public AeriaServersProvider()
            : base(Server.ServerType.ADMO) {
        }
    }
}