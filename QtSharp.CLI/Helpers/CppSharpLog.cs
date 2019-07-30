using System.Collections.Generic;
using System.Linq;
using CppSharp;
using QtSharp.CLI.Logging;

namespace QtSharp.CLI.Helpers
{
    /// <summary> Used to override CppSharp's default Logging output. </summary>
    public class CppSharpLog : IDiagnostics
    {
        /// <summary> Gets the LibLog logger instance. </summary>
        /// <value> LibLog Logger. </value>
        private static ILog Log => _Log ?? (_Log = LogProvider.GetCurrentClassLogger());
        private static ILog _Log;

        /// <summary> Default constructor. </summary>
        public CppSharpLog()
        {
            Indents = new Stack<int>();
            Level = DiagnosticKind.Message;
        }

        /// <summary> The number of indents to add to the message. </summary>
        public Stack<int> Indents;

        /// <summary> Normally a cutoff for the logging level, not used here since SerilOg handles that. </summary>
        public DiagnosticKind Level { get; set; }

        /// <summary> Handle the logging message. </summary>
        /// <param name="info"> The log information. </param>
        public void Emit(DiagnosticInfo info) {
            var currentIndent = Indents.Sum();
            var message = new string(' ', currentIndent) + info.Message;
            var level = DiagnosticKind_To_LibLog(info.Kind);
            Log.Log(level, () => message);
        }

        /// <summary> Increases the indent of the log output. </summary>
        /// <param name="level"> amount to increase the indent. </param>
        public void PushIndent(int level = 4) {
            Indents.Push(level);
        }

        /// <summary> Decrease the indent of the log output. </summary>
        public void PopIndent() {
            Indents.Pop();
        }

        /// <summary> Convert DiagnosticKind loglevel to LibLog loglevel </summary>
        /// <param name="level"> CppSharp DiagnosticKind loglevel </param>
        /// <returns> LibLog LogLevel. </returns>
        private static LogLevel DiagnosticKind_To_LibLog(DiagnosticKind level)
        {
            switch (level)
            {
                case DiagnosticKind.Error:
                    return LogLevel.Error;
                case DiagnosticKind.Warning:
                    return LogLevel.Warn;
                case DiagnosticKind.Message:
                    return LogLevel.Info;
                case DiagnosticKind.Debug:
                    return LogLevel.Debug;
                default:
                    return LogLevel.Info;
            }
        }
    }
}
