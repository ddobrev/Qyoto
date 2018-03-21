using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CppSharp;

namespace QtSharp.CLI
{
    public class Program
    {
        private static int ParseArgs(string[] args, out string qmake, out string make, out bool debug)
        {
            qmake = null;
            make = null;
            debug = false;

            if (args.Length < 2)
            {
               Console.WriteLine("Please enter the paths to qmake and make.");
               return 1;
            }

            qmake = args [0];
            if (!File.Exists(qmake))
            {
                Console.WriteLine("The specified qmake does not exist.");
                return 1;
            }

            make = args [1];
            if (!File.Exists(make))
            {
               Console.WriteLine("The specified make does not exist.");
               return 1;
            }

            debug = args.Length > 2 && (args[2] == "d" || args[2] == "debug");

            return 0;
        }

        public static int Main(string[] args)
        {
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

            bool log = false;
            ConsoleLogger logredirect = log ? new ConsoleLogger() : null;
            if (logredirect != null)
                logredirect.CreateLogDirectory();

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
                Console.WriteLine("Generation failed.");
                return 1;
            }

            const string qtSharpZip = "QtSharp.zip";
            if (File.Exists(qtSharpZip))
            {
                File.Delete(qtSharpZip);
            }
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
            Console.WriteLine("Done in: " + s.Elapsed);
            return 0;
        }
    }
}
