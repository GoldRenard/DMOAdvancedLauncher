using System;
using System.Text;

namespace AdvancedLauncher.Environment.Commands {

    public class EchoCommand : Command {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(EchoCommand));

        public EchoCommand()
            : base("echo", "Echo to console") {
        }

        public override void DoCommand(string[] args) {
            StringBuilder builder = new StringBuilder();
            bool skipEcho = false;
            foreach (String arg in args) {
                if (!skipEcho) {
                    skipEcho = true;
                    continue;
                }
                builder.Append(String.Format("{0} ", arg));
            }
            LOGGER.Info(builder);
        }
    }
}