using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Geeks.VSIX.SmartAttach.Attacher
{
    public class ExcludedProcessesManager
    {
        static readonly string ExcludedProcessesFileName;//"c:\\test\\processes.txt";

        public static readonly string WebServer_W3WP_ProcessName = "w3wp";
        static readonly string[] WebServerProcessNames = new[] { WebServer_W3WP_ProcessName, "iisexpress.exe" };

        static readonly string[] ExcludedProcessNames = new[]
        {
            "ServiceHub".ToLower(),
            "Microsoft".ToLower(),
            "iisexpresstray".ToLower(),
            "devenv".ToLower(),
            "IntelliTrace.exe".ToLower(),
            "Msbuild.exe".ToLower(),
            "MSBuildWatcher.exe".ToLower(),
            "MSBuild.exe".ToLower(),
            "IpOverUsbSvc.exe".ToLower(),
            "PerfWatson2.exe".ToLower()
        };

        static readonly string[] ExcludedProcessNames_WithCommpandLine = new[] { "C:\\program files (x86)\\".ToLower() };

        public static bool IsEnabled = true;
        readonly static bool ShouldWrtieToSetting = true;
        readonly static bool ShouldWrtieToFile;

        static Lazy<List<string>> excludedNoneDotNetProcesses =
            new Lazy<List<string>>(() =>
            {
                if (!ShouldWrtieToSetting && !ShouldWrtieToFile) return new List<string>();

                if (ShouldWrtieToSetting)
                {
                    if (VSIX.SmartAttach.Properties.Settings.Default.ExcludedNoneDotNetProcess == null) return new List<string>();

                    return VSIX.SmartAttach.Properties.Settings.Default.ExcludedNoneDotNetProcess.Split(new char[] { '\n' }).ToList();
                }
                else if (ShouldWrtieToFile)
                {
                    var existsFile = File.Exists(ExcludedProcessesFileName);
                    if (existsFile) return File.ReadAllLines(ExcludedProcessesFileName).ToList();
                    File.Create(ExcludedProcessesFileName).Close();
                }

                return new List<string>();
            });
        static List<string> ExcludedNoneDotNetProcesses = excludedNoneDotNetProcesses.Value;

        bool IsDotNetProcess(System.Diagnostics.Process process, string processFullName)
        {
            try
            {
                    if (processFullName.Contains("dotnet.exe")) return true;

                    var modules = process.Modules.Cast<ProcessModule>().Where(
                    m => m.ModuleName.StartsWith("mscor", StringComparison.InvariantCultureIgnoreCase));

                return modules.Any();
            }
            catch (Exception e)
            {
                AddExcludedNoneDotNetProcesses(processFullName);

                return false;
            }
        }

        public DateTime? CheckAndReturnStartTime(EnvDTE80.Process2 prc)
        {
            var processFullName = prc.Name.ToLower();

            if (IsEnabled)
            {
                if (ExcludedNoneDotNetProcesses.Any(x => x.ToLower() == processFullName)) return null;
            }

            if (ExcludedProcessNames.Any(x => prc.Name.ToLower().StartsWith(x))) return null;

            var fileName = Path.GetFileName(prc.Name);
            if (fileName.HasValue())
                if (ExcludedProcessNames.Any(x => fileName.Equals(x, StringComparison.OrdinalIgnoreCase)))
                    return null;

            try
            {
                var tp = Process.GetProcessById(prc.ProcessID);

                if (ExcludedProcessNames.Any(x => tp.ProcessName.ToLower().StartsWith(x))) return null;

                if (WebServerProcessNames.Any(n => processFullName.IndexOf(n) >= 0) == false && IsDotNetProcess(tp, processFullName) == false)
                    return null;

                return tp.StartTime;
            }
            catch
            {
            }

            return null;
        }

        public static bool CheckCommandLine(string cmdLineStr)
        {
            return ExcludedProcessNames_WithCommpandLine.Any(x => cmdLineStr.ToLower().StartsWith(x));
        }

        ConcurrentQueue<string> tempExcludedNoneDotNetProcesses = new ConcurrentQueue<string>();
        void AddExcludedNoneDotNetProcesses(string processFullName)
        {
            if (IsEnabled == false) return;
            if (processFullName == null) return;
            tempExcludedNoneDotNetProcesses.Enqueue(processFullName);
        }
        public void Flush()
        {
            if (IsEnabled == false) return;
            if (tempExcludedNoneDotNetProcesses.IsEmpty) return;

            if (ShouldWrtieToSetting)
            {
                var oldprocesses = VSIX.SmartAttach.Properties.Settings.Default.ExcludedNoneDotNetProcess.Split(new[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
                
                var allProcesses =  string.Join("\n", oldprocesses.Union(tempExcludedNoneDotNetProcesses).Distinct());
                
                VSIX.SmartAttach.Properties.Settings.Default.ExcludedNoneDotNetProcess = allProcesses;
                VSIX.SmartAttach.Properties.Settings.Default.Save();
            }
            else if (ShouldWrtieToFile)
            {
                File.AppendAllLines(ExcludedProcessesFileName, tempExcludedNoneDotNetProcesses);
            }

            foreach (var processFullName in tempExcludedNoneDotNetProcesses)
                ExcludedNoneDotNetProcesses.Add(processFullName);


            tempExcludedNoneDotNetProcesses = new ConcurrentQueue<string>();
        }
    }
}