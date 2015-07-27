using AdvancedLauncher.Model.Config;
using DMOLibrary.Events;

namespace AdvancedLauncher.Management.Interfaces {

    public interface ILoginManager : IManager {

        void Login();

        void Login(Profile profile);

        event LoginCompleteEventHandler LoginCompleted;
    }
}