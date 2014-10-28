using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AdvancedLauncher.Environment.Commands {

    public class ExecCommand : Command {
        private static readonly log4net.ILog LOGGER = log4net.LogManager.GetLogger(typeof(ExecCommand));

        public ExecCommand()
            : base("exec", "Execute program") {
        }

        public override void DoCommand(string[] args) {
            if (args.Length < 2) {
                LOGGER.InfoFormat("Usage: {0} <executable> [arguments]", args[0]);
                return;
            }
            List<String> argList = new List<string>(args);
            string executable = argList[1];
            argList.RemoveRange(0, 2);

            try {
                if (argList.Count > 0) {
                    Process.Start(executable, ParseArguments(argList));
                } else {
                    Process.Start(executable);
                }
            } catch (Exception ex) {
                LOGGER.Error(ex);
            }
        }

        private string ParseArguments(List<String> args) {
            StringBuilder builder = new StringBuilder();

            foreach (String arg in args) {
                builder.Append(arg + " ");
            }
            return builder.ToString().Trim();
        }
    }
}