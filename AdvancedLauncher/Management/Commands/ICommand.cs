using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedLauncher.Management.Commands {
    public interface ICommand {
        bool DoCommand(string[] args);

        string GetDescription();

        string GetName();
    }
}
