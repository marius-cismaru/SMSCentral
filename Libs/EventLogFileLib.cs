using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace SMSCentral.Libs
{
    public class EventLogFileLib
    {
        public enum Levels
        {
            [Description("Error")]
            ERROR,
            [Description("Warning")]
            WARNING,
            [Description("Info")]
            INFO
        }

        //Logging messages to file
        public static string Write(Levels level, string message, string details)
        {
            string result = string.Empty;
            try
            {
                //Setup file name
                string logFilePath = System.AppDomain.CurrentDomain.BaseDirectory + @"Logs\";
                string logFileName = "Log_" + EnumLib.GetDescriptionFromEnum(level) + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                string dateNow = DateTime.Now.ToString("HH:mm:ss.fff");
                StringBuilder sb = new StringBuilder();
                //sb.AppendLine(string.Concat(Enumerable.Repeat(Environment.NewLine, 1)));
                sb.AppendLine(new string('-', 80));
                sb.AppendLine("| " + dateNow + " | " + EnumLib.GetDescriptionFromEnum(level).ToUpperInvariant() + " | " + message);
                if (!string.IsNullOrEmpty(details.Trim()))
                {
                    sb.AppendLine(Environment.NewLine + details);
                }
                //Create directory
                Directory.CreateDirectory(logFilePath);
                //Append error to file
                File.AppendAllText(logFilePath + logFileName, sb.ToString());
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }

        public static string Write(Levels level, string message)
        {
            return Write(level, message, string.Empty);
        }
    }
}
