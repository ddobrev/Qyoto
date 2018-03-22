using System;
using System.IO;
using System.Text;
using QtSharp.CLI.Logging;

// Since CppSharp uses Console.WriteLine, this is a workaround which captures the console output
// and funnels it through LibLog which goes to SeriLog. This is done without interfering with Serilog's Console output

// If CppSharp decides to move to a logging abstraction such as LibLog or some other then this could potentially be removed


namespace QtSharp.CLI.Helpers
{
    /// <summary> Used for Console redirection of logging through LibLog / Serilog. </summary>
    public class ConsoleRedirect : TextWriter {

        /// <summary> Logging level to use for this redirection. </summary>
        public LogLevel LoggingLevel;

        /// <summary> The character encoding in which the output is written. </summary>
        /// <value> The character encoding in which the output is written. </value>
        public override Encoding Encoding => Encoding.Default;

        /// <summary> Gets the LibLog logger instance. </summary>
        /// <value> LibLog Logger. </value>
        private static ILog Log => _Log ?? (_Log = LogProvider.GetCurrentClassLogger());
        private static ILog _Log;

        /// <summary> Constructor. </summary>
        /// <param name="level"> The logging level. </param>
        private ConsoleRedirect(LogLevel level) {
            LoggingLevel = level;
        }

        /// <summary> Writes a line and terminator to LibLog. </summary>
        /// <param name="value"> The string to write. If <paramref name="value" /> is null, only the line
        ///                      terminator is written. </param>
        public override void WriteLine(string value) {
            Log.Log(LoggingLevel, () => value);
        }

        /// <summary> Writes characters to the default non redirected Console Output. </summary>
        /// <param name="value"> The character to write to the text stream. </param>
        public override void Write(char value)
        {
            ConsoleStdOutput.Write(value);
        }


        /// <summary> Original Console Output Stream. </summary>
        /// <value> Original Console Output Stream. </value>
        public static TextWriter ConsoleStdOutput { get; set; }

        /// <summary> Original Console Error Stream. </summary>
        /// <value> Original Console Error Stream. </value>
        public static TextWriter ConsoleErrOutput { get; set; }

        /// <summary> Standard Output Console redirection stream. </summary>
        private static ConsoleRedirect out_redirectstream;
        /// <summary> Standard Error Console redirection stream. </summary>
        private static ConsoleRedirect err_redirectstream;

        /// <summary> Start the redirection to LibLog. </summary>
        public static void Start()
        {
            // Make a note of the original console logging outputs
            ConsoleStdOutput = Console.Out;
            ConsoleErrOutput = Console.Error;

            out_redirectstream = new ConsoleRedirect(LogLevel.Info);
            Console.SetOut(out_redirectstream);
            err_redirectstream = new ConsoleRedirect(LogLevel.Error);
            Console.SetError(err_redirectstream);
        }

        /// <summary> Stops the redirection. </summary>
        public static void Stop()
        {
            Console.SetOut(ConsoleStdOutput);
            Console.SetError(ConsoleErrOutput);
            out_redirectstream?.Close();
            err_redirectstream?.Close();
        }
    }
}
