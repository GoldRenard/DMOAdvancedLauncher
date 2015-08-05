namespace AdvancedLauncher.SDK.Model.Events.Proxy {

    public class BaseEventProxy : EventProxy<BaseEventArgs> {

        private event BaseEventHandler EventHandler;

        public BaseEventProxy(BaseEventHandler action) {
            EventHandler += action;
        }

        public override void Handler(object sender, BaseEventArgs e) {
            if (EventHandler != null) {
                EventHandler(this, e);
            }
        }
    }
}