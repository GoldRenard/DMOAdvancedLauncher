using AdvancedLauncher.SDK.Model.Entity;

namespace DMOLibrary.Profiles.Korea {

    public class KoreaIMBCServersProvider : KoreaServersProvider {

        public KoreaIMBCServersProvider()
            : base(Server.ServerType.KDMO_IMBC) {
        }
    }
}