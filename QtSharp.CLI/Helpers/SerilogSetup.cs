using System;
using System.IO;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace QtSharp.CLI.Helpers
{
    /// <summary> The setup of Serilog for logging. </summary>
    public class SerilogSetup
    {
        /// <summary> Full pathname of the log file. </summary>
        public static string LogFilePath = @"logs\qtsharp1.txt";

        /// <summary> Setup logging without a Dttm. </summary>
        public static Serilog.Core.Logger GetLogger_Nodttm()
        {
            var outputTemplate = "{Message:lj}{NewLine}{Exception}";
            var theme = AnsiConsoleTheme.Code;

            // Setup Serilog Configuration
            var serilogcfg = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: outputTemplate, theme: theme);

            return serilogcfg.CreateLogger();
        }

        /// <summary> Setup logging with a Dttm. </summary>
        public static Serilog.Core.Logger GetLogger_dttm()
        { 
            var outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            var theme = AnsiConsoleTheme.Code;

            // Setup Serilog Configuration
            var serilogcfg = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(outputTemplate: outputTemplate, theme: theme);

            // Optionally add in file logging
            SetupFileLogging(serilogcfg);

            return serilogcfg.CreateLogger();
        }

        /// <summary> Sets up logging for file output. </summary>
        /// <param name="serilogcfg"> The serilog configuration. </param>
        private static void SetupFileLogging(LoggerConfiguration serilogcfg) {
            // Note logging is limited to 1Gb by default - https://github.com/serilog/serilog-sinks-file

            // Get full path
            string fullpath = Path.Combine(Directory.GetCurrentDirectory(), LogFilePath);
            // Create the logging output directory
            string logdir = Path.GetDirectoryName(fullpath);
            if (Directory.Exists(logdir) == false) Directory.CreateDirectory(logdir ?? throw new InvalidOperationException());
            // Setup Serilog
            serilogcfg.WriteTo.File(fullpath);
        }
    }
}
