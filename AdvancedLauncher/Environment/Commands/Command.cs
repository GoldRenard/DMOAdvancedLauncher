
namespace AdvancedLauncher.Environment.Commands {
    public abstract class Command {
        private string commandName;
        private string commandDescription;

        public Command(string commandName, string commandDescription) {
            this.commandName = commandName;
            this.commandDescription = commandDescription;
        }

        public abstract void DoCommand(string[] args);

        public virtual string GetDescription() {
            return commandDescription;
        }
        public virtual string GetName() {
            return commandName;
        }
    }
}
