using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CppSharp;
using QtSharp.CLI.Helpers;
using QtSharp.CLI.Logging;

namespace QtSharp.CLI
{
    public class Program
    {
        /// <summary> Gets the LibLog logger instance. </summary>
        /// <value> LibLog Logger. </value>
        private static ILog Log => _Log ?? (_Log = LogProvider.GetCurrentClassLogger());
        private static ILog _Log;

        private static int ParseArgs(string[] args, out string qmake, out string make, out bool debug)
        {
            qmake = null;
            make = null;
            debug = false;

            if (args.Length < 2)
            {
               Log.Error("Please enter the paths to qmake and make.");
               return 1;
            }

            qmake = args [0];
            if (!File.Exists(qmake))
            {
                Log.Error("The specified qmake does not exist.");
                return 1;
            }

            make = args [1];
            if (!File.Exists(make))
            {
               Log.Error("The specified make does not exist.");
               return 1;
            }

            debug = args.Length > 2 && (args[2] == "d" || args[2] == "debug");

            return 0;
        }

        public static int Main(string[] args)
        {
            // Setup Logging with SeriLog
            // remove timestamp etc for while we're parsing command line args / input etc
            var logger = SerilogSetup.GetLogger_Nodttm();
            Serilog.Log.Logger = logger;

            // Workaround to redirect Console.WriteLine through Liblog / Serilog
            ConsoleRedirect.Start();

            Stopwatch s = Stopwatch.StartNew();
            var qts = QtInfo.FindQt();
            bool found = qts.Count != 0;
            bool debug = false;
            QtInfo qt;

            if (!found)
            {
                qt = new QtInfo();

                var result = ParseArgs(args, out qt.QMake, out qt.Make, out debug);
                if (result != 0)
                    return result;
            }
            else
            {
                // TODO: Only for OSX for now, generalize for all platforms.
                qt = qts.Last();
            }

            // Re-setup logging with default template including timestamp / log level
            logger.Dispose();
            logger = SerilogSetup.GetLogger_dttm();
            Serilog.Log.Logger = logger;
            _Log = LogProvider.GetCurrentClassLogger();

            Log.Info("QtSharp Starting");
            if (!qt.Query(debug))
                return 1;

            for (int i = qt.LibFiles.Count - 1; i >= 0; i--)
            {
                var libFile = qt.LibFiles[i];
                var libFileName = Path.GetFileNameWithoutExtension(libFile);
                if (Path.GetExtension(libFile) == ".exe" ||
                    // QtQuickTest is a QML module but has 3 main C++ functions and is not auto-ignored
                    libFileName == "QtQuickTest" || libFileName == "Qt5QuickTest")
                {
                    qt.LibFiles.RemoveAt(i);
                }
            }
            var qtSharp = new QtSharp(qt);
            ConsoleDriver.Run(qtSharp);
            var wrappedModules = qtSharp.GetVerifiedWrappedModules();

            if (wrappedModules.Count == 0)
            {
                Log.Error("Generation failed.");
                return 1;
            }

            const string qtSharpZip = "QtSharp.zip";
            if (File.Exists(qtSharpZip)) File.Delete(qtSharpZip);
            
            using (var zip = File.Create(qtSharpZip))
            {
                using (var zipArchive = new ZipArchive(zip, ZipArchiveMode.Create))
                {
                    foreach (var wrappedModule in wrappedModules)
                    {
                        zipArchive.CreateEntryFromFile(wrappedModule.Key, wrappedModule.Key);
                        var documentation = Path.ChangeExtension(wrappedModule.Key, "xml");
                        zipArchive.CreateEntryFromFile(documentation, documentation);
                        zipArchive.CreateEntryFromFile(wrappedModule.Value, Path.GetFileName(wrappedModule.Value));
                    }
                    zipArchive.CreateEntryFromFile("CppSharp.Runtime.dll", "CppSharp.Runtime.dll");
                }
            }
            Log.Info("Done in: " + s.Elapsed);
            Log.Info("QtSharp Finished");
            ConsoleRedirect.Stop();
            return 0;
        }
    }
}
