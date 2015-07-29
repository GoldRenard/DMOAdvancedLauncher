using AdvancedLauncher.SDK.Model.Entity;

namespace DMOLibrary.Profiles.Korea {

    public class KoreaServersProvider : DatabaseServersProvider {

        public KoreaServersProvider()
            : base(Server.ServerType.KDMO) {
        }

        public KoreaServersProvider(Server.ServerType serverType)
            : base(serverType) {
        }
    }
}