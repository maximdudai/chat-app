using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chat_log
{
    public class Log
    {
        private readonly string _logFilePath;

        // Constructor
        public Log(string logType)
        {
            string mainDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string logDirectory = Path.Combine(mainDirectory, "Logs");

            // Verify if the log directory exists, if not, create it
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            // Set the log file path based on the type
            _logFilePath = Path.Combine(logDirectory, $"{logType}_log.txt");
        }

        public void AddLog(string message)
        {
            string logdataFormat = $"{DateTime.Now:dd/MM/yyyy HH:mm:ss} - {message}";
            File.AppendAllText(_logFilePath, logdataFormat + Environment.NewLine);
        }
    }
}
