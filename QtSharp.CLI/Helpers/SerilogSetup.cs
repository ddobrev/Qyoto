using System.IO;
using Serilog;

namespace QtSharp.CLI.Helpers
{
    /// <summary> The setup of Serilog for logging. </summary>
    public class SerilogSetup
    {
        /// <summary> Full pathname of the log file. </summary>
        public static string LogFilePath = @"logs\qtsharp1.txt";

        /// <summary> Sets up Logging. </summary>
        public static void SetupLogging()
        {
            // Setup Serilog Configuration
            var serilogcfg = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console();

            // Optionally add in file logging
            //SetupFileLogging(serilogcfg);

            // Create the logger
            Log.Logger = serilogcfg.CreateLogger();
        }

        /// <summary> Sets up logging for file output. </summary>
        /// <param name="serilogcfg"> The serilog configuration. </param>
        private static void SetupFileLogging(LoggerConfiguration serilogcfg) {
            // Note logging is limited to 1Gb by default - https://github.com/serilog/serilog-sinks-file

            // Get full path
            string fullpath = Path.Combine(Directory.GetCurrentDirectory(), LogFilePath);
            // Create the logging output directory
            string logdir = Path.GetDirectoryName(fullpath);
            if (Directory.Exists(logdir) == false) Directory.CreateDirectory(logdir);
            // Setup Serilog
            serilogcfg.WriteTo.File(fullpath);
        }
    }
}
