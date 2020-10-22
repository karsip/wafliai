using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameModels.Singleton
{
    public sealed class Logger : ILogger
    {
        private Logger()
        {

        }
        private static readonly Lazy<Logger> instance = new Lazy<Logger>(() => new Logger());
        public static Logger GetInstance
        {
            get {
                return instance.Value;
            }
        }
        public void LogException(string messagge)
        {
            string fileName = string.Format("{0}_{1}.log", "Exception", DateTime.Now.ToShortDateString());
            // string logFilePath = string.Format(@"{0}\{1}", AppDomain.CurrentDomain.BaseDirectory, fileName);
            string logFilePath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, fileName);
            StringBuilder sb = new StringBuilder();
            string line = new string('-', 25);
            sb.AppendLine(line);
            sb.AppendLine(DateTime.Now.ToString());
            sb.AppendLine(messagge);
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.Write(sb.ToString());
                sw.Flush();
            }
        }
    }
}
