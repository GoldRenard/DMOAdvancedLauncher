using AdvancedLauncher.Service;

namespace AdvancedLauncher.Environment.Commands {

    public class ExitCommand : Command {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(ExitCommand));

        public ExitCommand()
            : base("exit", "Schedules the application shutdown") {
        }

        public override void DoCommand(string[] args) {
            TaskManager.CloseApp();
            LOGGER.InfoFormat("Shutdown scheduled...");
        }
    }
}