using System.Collections.Concurrent;

namespace AdvancedLauncher.Management.Interfaces {

    public interface ITaskManager : IManager {

        ConcurrentBag<TaskManager.Task> Tasks {
            get;
        }

        bool IsBusy {
            get;
        }

        void CloseApp();
    }
}