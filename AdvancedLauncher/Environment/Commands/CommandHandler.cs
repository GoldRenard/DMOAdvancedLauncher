using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedLauncher.Environment.Commands {
    public static class CommandHandler {

        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(CommandHandler));  
        private static Dictionary<String, Command> commands = new Dictionary<String, Command>();

        private const string ENTER_COMMAND = "Please enter the command or \"help\" to view available commands";
        private const string UNKNOWN_COMMAND = "Unknown command \"{0}\"";

        private static List<String> recentCommands = new List<string>();

        static CommandHandler() {
            commands.Add("help", new HelpCommand());
        }

        public static void Send(string input) {
            if (string.IsNullOrEmpty(input)) {
                LOGGER.Info(ENTER_COMMAND);
                return;
            }
            recentCommands.Add(input);
            string[] args = input.Split(' ');
            if (!commands.ContainsKey(args[0])) {
                LOGGER.Info(String.Format(UNKNOWN_COMMAND, args[0]));
                return;
            }
            Command command;
            commands.TryGetValue(args[0], out command);
            if (command == null) {
                LOGGER.Info(String.Format(UNKNOWN_COMMAND, args[0]));
                return;
            }
            command.DoCommand(args);
        }

        public static void RegisterCommand(String name, Command command) {
            commands.Add(name, command);
        }

        public static void UnRegisterCommand(string name) {
            commands.Remove(name);
        }

        public static Dictionary<String, Command> GetCommands() {
            return commands;
        }

        public static List<String> GetRecent() {
            return recentCommands;
        }
    }
}
