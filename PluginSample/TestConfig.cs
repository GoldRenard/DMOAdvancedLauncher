using AdvancedLauncher.Providers.GameKing;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Web;

namespace PluginSample {

    public class TestConfig : AbstractConfiguration {
        private readonly ILogManager LogManager;
        private readonly IDatabaseManager DatabaseManager;

        public TestConfig(IDatabaseManager DatabaseManager, ILogManager LogManager) {
            this.LogManager = LogManager;
            this.DatabaseManager = DatabaseManager;
        }

        #region Common

        public override string Name {
            get {
                return "Joymax Plugin";
            }
        }

        public override string GameType {
            get {
                return "GDMOP";
            }
        }

        public override string LauncherExecutable {
            get {
                return "DMLauncher.exe";
            }
        }

        public override string LauncherPathRegKey {
            get {
                return "Software\\Joymax\\DMO";
            }
        }

        public override string LauncherPathRegVal {
            get {
                return "Path";
            }
        }

        public override string GameExecutable {
            get {
                return "GDMO.exe";
            }
        }

        public override string GamePathRegKey {
            get {
                return "Software\\Joymax\\DMO";
            }
        }

        public override string GamePathRegVal {
            get {
                return "Path";
            }
        }

        public override bool IsLastSessionAvailable {
            get {
                return false;
            }
        }

        public override string PatchRemoteURL {
            get {
                return "http://patch.dmo.joymax.com/GDMO{0}.zip";
            }
        }

        public override string VersionLocalPath {
            get {
                return @"LauncherLib\vGDMO.ini";
            }
        }

        public override string VersionRemoteURL {
            get {
                return "http://patch.dmo.joymax.com/PatchInfo_GDMO.ini";
            }
        }

        public override string ConvertGameStartArgs(string args) {
            return "true";
        }

        #endregion Common

        #region Providers

        public override IWebProvider CreateWebProvider() {
            return new GameKingWebProvider(DatabaseManager, LogManager);
        }

        public override INewsProvider CreateNewsProvider() {
            return new JoymaxNewsProvider(LogManager);
        }

        protected override IServersProvider CreateServersProvider() {
            return new GameKingServersProvider(DatabaseManager);
        }

        public override bool IsLoginRequired {
            get {
                return false;
            }
        }

        public override bool IsWebAvailable {
            get {
                return true;
            }
        }

        public override bool IsNewsAvailable {
            get {
                return true;
            }
        }

        #endregion Providers
    }
}