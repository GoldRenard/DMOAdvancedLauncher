using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedLauncher.Environment.Commands {

    public class HelpCommand : Command {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(HelpCommand));

        public HelpCommand()
            : base("help", "Shows help message") {
        }

        public override void DoCommand(string[] args) {
            Dictionary<String, Command> commands = CommandHandler.GetCommands();
            StringBuilder builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine("Available commands:");
            foreach (String key in commands.Keys) {
                Command command;
                commands.TryGetValue(key, out command);
                builder.AppendLine(String.Format("\t{0} - {1}", key, command.GetDescription()));
            }
            LOGGER.Info(builder);
        }
    }
}