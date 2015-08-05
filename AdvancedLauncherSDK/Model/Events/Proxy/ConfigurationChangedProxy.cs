namespace AdvancedLauncher.SDK.Model.Events.Proxy {

    public class ConfigurationChangedProxy : EventProxy<ConfigurationChangedEventArgs> {

        private event ConfigurationChangedEventHandler EventHandler;

        public ConfigurationChangedProxy(ConfigurationChangedEventHandler action) {
            EventHandler += action;
        }

        public override void Handler(object sender, ConfigurationChangedEventArgs e) {
            if (EventHandler != null) {
                EventHandler(this, e);
            }
        }
    }
}