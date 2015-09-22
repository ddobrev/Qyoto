using System;
using System.IO;

namespace QtSharp.CLI
{
    public class ConsoleLogger
    {
        /// <summary> Original Console Output Stream. </summary>
        /// <value> Original Console Output Stream. </value>
        public TextWriter ConsoleStdOutput { get; set; }

        /// <summary> Original Console Output Stream. </summary>
        /// <value> Original Console Output Stream. </value>
        public TextWriter ConsoleErrOutput { get; set; }

        /// <summary> Path to the Log File. </summary>
        /// <value> Path to the Log File. </value>
        public string LogFilePath {
            get { return _LogFilePath; }
        }
        protected string _LogFilePath;

        /// <summary> Filestream used for output. </summary>
        protected FileStream _fstream;

        /// <summary> Filestream Writer used for output. </summary>
        protected StreamWriter _fstreamwriter;

        /// <summary> Default constructor. </summary>
        public ConsoleLogger() {
            ConsoleStdOutput = Console.Out;
            ConsoleErrOutput = Console.Error;
            _LogFilePath = null;
        }

        /// <summary> Sets the log file output. </summary>
        /// <param name="filename"> Filename of the log file to use. </param>
        public void SetLogFile(string filename) {
            _LogFilePath = Path.Combine("logs", filename);
        }

        /// <summary> Starts console redirection. </summary>
        public void Start() {
            Stop();
            _fstream = new FileStream(_LogFilePath, FileMode.Create);
            _fstreamwriter = new StreamWriter(_fstream);
            Console.SetOut(_fstreamwriter);
            Console.SetError(_fstreamwriter);
        }

        /// <summary> Stops console redirection. </summary>
        public void Stop() {
            Console.SetOut(ConsoleStdOutput);
            Console.SetError(ConsoleErrOutput);
            _fstreamwriter?.Close();
            _fstream?.Close();
        }

        /// <summary> Creates log directory. </summary>
        public void CreateLogDirectory() {
            if (Directory.Exists("logs") == false) Directory.CreateDirectory("logs");
        }
    }
}
