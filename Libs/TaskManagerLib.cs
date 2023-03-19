using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace SMSCentral.Libs
{
    public static class TaskManagerLib
    {
        public enum ProcessNameMatchType
        {
            Exact,
            Contains,
            StartsWith,
            EndsWith
        }

        public static bool KillProcessUsingSystemProcess(string processName, ProcessNameMatchType processNameMatchType)
        {
            bool result = true;

            try
            {
                List<System.Diagnostics.Process> processes = new List<System.Diagnostics.Process>();
                //Search for processes using match criteria
                switch (processNameMatchType)
                {
                    case ProcessNameMatchType.Exact:
                        processes = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.ToUpper() == processName.ToUpper()).ToList();
                        break;
                    case ProcessNameMatchType.Contains:
                        processes = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.ToUpper().Contains(processName.ToUpper())).ToList();
                        break;
                    case ProcessNameMatchType.StartsWith:
                        processes = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.ToUpper().StartsWith(processName.ToUpper())).ToList();
                        break;
                    case ProcessNameMatchType.EndsWith:
                        processes = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.ToUpper().EndsWith(processName.ToUpper())).ToList();
                        break;
                }

                //Kill'em all | Kill them with fire - old way - doesn't work all the time
                processes.ForEach(o => o.Kill());
            }
            catch (Exception ex)
            {
                result = false;
                var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
                EventLogFileLib.Write(EventLogFileLib.Levels.ERROR, currentMethod.DeclaringType + "." + currentMethod.Name, ex.ToString());
            }

            return result;
        }

        public static bool KillProcessUsingTaskKill(string processName, ProcessNameMatchType processNameMatchType)
        {
            bool result = true;

            try
            {
                //Process.Start("taskkill", string.Format("/F /IM {0}", processName));

                ProcessStartInfo ProcessInfo = new ProcessStartInfo("taskkill", string.Format("/F /IM {0}", processName));
                ProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;
                ProcessInfo.CreateNoWindow = true;
                Process.Start(ProcessInfo);

                System.Threading.Thread.Sleep(250);
            }
            catch (Exception ex)
            {
                result = false;
                var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
                EventLogFileLib.Write(EventLogFileLib.Levels.ERROR, currentMethod.DeclaringType + "." + currentMethod.Name, ex.ToString());
            }

            return result;
        }

        public static bool KillProcess(string processName, ProcessNameMatchType processNameMatchType)
        {
            return KillProcessUsingTaskKill(processName, processNameMatchType);
        }

        public static bool KillProcessesUsingSystemProcess(List<string> processNames, ProcessNameMatchType processNameMatchType)
        {
            bool result = true;

            foreach (var processName in processNames)
            {
                result &= KillProcessUsingSystemProcess(processName, processNameMatchType);
            }

            return result;
        }

        public static bool KillProcessesUsingTaskKill(List<string> processNames, ProcessNameMatchType processNameMatchType)
        {
            bool result = true;

            foreach (var processName in processNames)
            {
                result &= KillProcessUsingTaskKill(processName, processNameMatchType);
            }

            return result;
        }

        public static bool KillProcesses(List<string> processNames, ProcessNameMatchType processNameMatchType)
        {
            return KillProcessesUsingTaskKill(processNames, processNameMatchType);
        }

        public static bool StartProcess(string processPathToExe)
        {
            bool result = true;

            try
            {
                var startInfo = new ProcessStartInfo();

                //Get starting app path
                startInfo.WorkingDirectory = Path.GetDirectoryName(processPathToExe);
                startInfo.FileName = processPathToExe;
                Process.Start(startInfo);

                //System.Diagnostics.Process.Start(processPathToExe);
            }
            catch (Exception ex)
            {
                result = false;
                var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
                EventLogFileLib.Write(EventLogFileLib.Levels.ERROR, currentMethod.DeclaringType + "." + currentMethod.Name, ex.ToString());
            }

            return result;
        }

        public static bool StartProcesses(List<string> processPathsToExe)
        {
            bool result = true;

            foreach (var processPathToExe in processPathsToExe)
            {
                result &= StartProcess(processPathToExe);
            }

            return result;
        }

        public static int GetRunningProcessCount(string processName, ProcessNameMatchType processNameMatchType)
        {
            int result = 0;

            try
            {
                //Search for processes using match criteria
                switch (processNameMatchType)
                {
                    case ProcessNameMatchType.Exact:
                        result = System.Diagnostics.Process.GetProcesses().Count(o => o.ProcessName.ToUpper() == processName.ToUpper());
                        break;
                    case ProcessNameMatchType.Contains:
                        result = System.Diagnostics.Process.GetProcesses().Count(o => o.ProcessName.ToUpper().Contains(processName.ToUpper()));
                        break;
                    case ProcessNameMatchType.StartsWith:
                        result = System.Diagnostics.Process.GetProcesses().Count(o => o.ProcessName.ToUpper().StartsWith(processName.ToUpper()));
                        break;
                    case ProcessNameMatchType.EndsWith:
                        result = System.Diagnostics.Process.GetProcesses().Count(o => o.ProcessName.ToUpper().EndsWith(processName.ToUpper()));
                        break;
                }
            }
            catch (Exception ex)
            {
                var currentMethod = System.Reflection.MethodBase.GetCurrentMethod();
                EventLogFileLib.Write(EventLogFileLib.Levels.ERROR, currentMethod.DeclaringType + "." + currentMethod.Name, ex.ToString());
            }

            return result;
        }

        public static int GetRunningProcessesCount(List<string> processNames, ProcessNameMatchType processNameMatchType)
        {
            int result = 0;

            foreach (var processName in processNames)
            {
                result += GetRunningProcessCount(processName, processNameMatchType);
            }

            return result;
        }
    }
}
