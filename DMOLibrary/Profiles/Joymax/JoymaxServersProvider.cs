using AdvancedLauncher.SDK.Model.Entity;

namespace DMOLibrary.Profiles.Joymax {

    public class JoymaxServersProvider : DatabaseServersProvider {

        public JoymaxServersProvider()
            : base(Server.ServerType.GDMO) {
        }
    }
}