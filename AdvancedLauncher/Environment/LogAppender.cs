using AdvancedLauncher.Windows;
using log4net.Appender;
using log4net.Core;

namespace AdvancedLauncher.Environment {

    public class LogAppender : AppenderSkeleton {

        protected override void Append(LoggingEvent loggingEvent) {
            if (Logger.IsInstanceInitialized) {
                lock (this) {
                    Logger.Instance.AddEntry(loggingEvent);
                }
            }
        }
    }
}