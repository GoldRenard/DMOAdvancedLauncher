using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedLauncher.Environment.Commands {
    public abstract class Command {
        public abstract void DoCommand(string[] args);
        public abstract string GetDescription();
    }
}
