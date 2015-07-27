using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedLauncher.Management.Commands;

namespace AdvancedLauncher.Management.Interfaces {
    public interface ICommandManager : IManager {
        bool Send(string input);

        void RegisterCommand(ICommand Command);

        bool UnRegisterCommand(string name);

        bool UnRegisterCommand(ICommand command);

        IDictionary<String, ICommand> GetCommands();

        List<string> GetRecent();
    }
}
