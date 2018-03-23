using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using CppSharp.Utils;
using CppSharp;
using QtSharp.Logging;

namespace QtSharp
{
    /// <summary> Information about QT. </summary>
    public class QtInfo
    {
        public int MajorVersion;
        public int MinorVersion;
        public string QtPath;
        public string Target;
        public string Docs;
        public string QMake;
        public string Make;
        public string Bins;
        public string Libs;
        public string Headers;
        public IList<string> LibFiles;
        public IEnumerable<string> SystemIncludeDirs;
        public IEnumerable<string> FrameworkDirs;

        /// <summary> Gets the LibLog logger instance. </summary>
        /// <value> LibLog Logger. </value>
        private static ILog Log => _Log ?? (_Log = LogProvider.GetCurrentClassLogger());
        private static ILog _Log;

        /// <summary> Searches for QT on the system. </summary>
        /// <returns> List of found QT installs. </returns>
        public static List<QtInfo> FindQt()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var qts = new List<QtInfo>();

            var qtPath = Path.Combine(home, "Qt");
            if (!Directory.Exists(qtPath))
            {
                return new List<QtInfo>();
            }

            foreach (var path in Directory.EnumerateDirectories(qtPath))
            {
                var dir = Path.GetFileName(path);
                var isNumber = dir.All(c => char.IsDigit(c) || c == '.');
                if (!isNumber)
                    continue;
                var qt = new QtInfo { QtPath = path };
                var match = Regex.Match(dir, @"([0-9]+)\.([0-9]+)");
                if (!match.Success)
                    continue;
                qt.MajorVersion = int.Parse(match.Groups[1].Value);
                qt.MinorVersion = int.Parse(match.Groups[2].Value);
                qts.Add(qt);
            }

            return qts;
        }

        /// <summary> Queries information about a given QT install </summary>
        /// <param name="debug"> True to debug. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public bool Query(bool debug)
        {
            // check for OS X
            if (string.IsNullOrWhiteSpace(QMake))
            {
                QMake = Path.Combine(QtPath, "clang_64/bin/qmake");
            }
            if (string.IsNullOrWhiteSpace(Make))
            {
                Make = "/usr/bin/make";
            }

            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
            path = Path.GetDirectoryName(Make) + Path.PathSeparator + path;
            Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.Process);


            int error;
            string errorMessage;
            Bins = ProcessHelper.Run(QMake, "-query QT_INSTALL_BINS", out error, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Log.Error(errorMessage);
                return false;
            }

            Libs = ProcessHelper.Run(QMake, "-query QT_INSTALL_LIBS", out error, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Log.Error(errorMessage);
                return false;
            }

            DirectoryInfo libsInfo = new DirectoryInfo(Platform.IsWindows ? Bins : Libs);
            if (!libsInfo.Exists)
            {
                Log.Error(
                    "The directory \"{0}\" that qmake returned as the lib directory of the Qt installation, does not exist.",
                    libsInfo.Name);
                return false;
            }
            LibFiles = GetLibFiles(libsInfo, debug);
            Headers = ProcessHelper.Run(QMake, "-query QT_INSTALL_HEADERS", out error, out errorMessage);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Log.Error(errorMessage);
                return false;
            }
            DirectoryInfo headersInfo = new DirectoryInfo(Headers);
            if (!headersInfo.Exists)
            {
                Log.Error(
                    "The directory \"{0}\" that qmake returned as the header direcory of the Qt installation, does not exist.",
                    headersInfo.Name);
                return false;
            }
            Docs = ProcessHelper.Run(QMake, "-query QT_INSTALL_DOCS", out error, out errorMessage);

            string emptyFile = Platform.IsWindows ? "NUL" : "/dev/null";
            string output;
            ProcessHelper.Run("gcc", $"-v -E -x c++ {emptyFile}", out error, out output);
            Target = Regex.Match(output, @"Target:\s*(?<target>[^\r\n]+)").Groups["target"].Value;

            const string includeDirsRegex = @"#include <\.\.\.> search starts here:(?<includes>.+)End of search list";
            string allIncludes = Regex.Match(output, includeDirsRegex, RegexOptions.Singleline).Groups["includes"].Value;
            var includeDirs = allIncludes.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()).ToList();

            const string frameworkDirectory = "(framework directory)";
            SystemIncludeDirs = includeDirs.Where(s => !s.Contains(frameworkDirectory))
                .Select(Path.GetFullPath);

            if (Platform.IsMacOS)
                FrameworkDirs = includeDirs.Where(s => s.Contains(frameworkDirectory))
                    .Select(s => s.Replace(frameworkDirectory, string.Empty).Trim()).Select(Path.GetFullPath);

            return true;
        }

        /// <summary> Gets library files. </summary>
        /// <param name="libsInfo"> Information describing the library directory. </param>
        /// <param name="debug">    True to debug. </param>
        /// <returns> List of the library files. </returns>
        private static IList<string> GetLibFiles(DirectoryInfo libsInfo, bool debug)
        {
            List<string> modules;

            if (Platform.IsMacOS)
            {
                modules = libsInfo.EnumerateDirectories("*.framework").Select(dir => Path.GetFileNameWithoutExtension(dir.Name)).ToList();
            }
            else
            {
                modules = (from file in libsInfo.EnumerateFiles()
                           where Regex.IsMatch(file.Name, @"^Qt\d?\w+\.\w+$")
                           select file.Name).ToList();
            }

            for (var i = modules.Count - 1; i >= 0; i--)
            {
                var module = Path.GetFileNameWithoutExtension(modules[i]);
                if (debug && module != null && !module.EndsWith("d", StringComparison.Ordinal))
                {
                    modules.Remove(module + Path.GetExtension(modules[i]));
                }
                else
                {
                    modules.Remove(module + "d" + Path.GetExtension(modules[i]));
                }
            }
            return modules;
        }
    }
}
