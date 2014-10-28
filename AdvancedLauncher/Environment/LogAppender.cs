using System;
using log4net.Appender;
using log4net.Core;
using AdvancedLauncher.Windows;

namespace AdvancedLauncher.Environment {
    public class LogAppender : AppenderSkeleton {

        protected override void Append(LoggingEvent loggingEvent) {
            if (Logger.IsInstanceInitialized) {
                Logger.Instance.AddEntry(loggingEvent);
            }
        }
    }
}
