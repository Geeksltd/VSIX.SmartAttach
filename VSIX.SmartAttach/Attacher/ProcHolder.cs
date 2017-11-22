using GeeksAddin;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections.Concurrent;

namespace Geeks.VSIX.SmartAttach.Attacher
{
    extern alias MSharp;

    public class ProcHolder
    {
        public EnvDTE80.Process2 Process { get; private set; }
        public string AppPool { get; private set; }
        public DateTime? StartTime { get; private set; } = null;

        static ExcludedProcessesManager _excludedProcessesManager = new ExcludedProcessesManager();
        public static ExcludedProcessesManager ExcludedProcessesManager
        {
            get
            {
                return _excludedProcessesManager;
            }
        }


        static ConcurrentDictionary<int, ProcHolder> alreadyCheckedProcesses = new ConcurrentDictionary<int, ProcHolder>();
        public static ProcHolder CheckAndAddProcHolder(EnvDTE80.Process2 process)
        {
            ProcHolder returnProcessHolder = null;

            try
            {
                if (process == null) return null;


                var startTime = ExcludedProcessesManager.CheckAndReturnStartTime(process);

                if (startTime == null) return null;

                if (alreadyCheckedProcesses.TryGetValue(process.ProcessID, out returnProcessHolder))
                {
                    if (returnProcessHolder.Process.Name == process.Name)
                    {
                        returnProcessHolder.StartTime = startTime;
                        return returnProcessHolder;
                    }
                }

                returnProcessHolder = new ProcHolder(process);
                if (returnProcessHolder.Process != null)
                {
                    alreadyCheckedProcesses.TryAdd(process.ProcessID, returnProcessHolder);
                    returnProcessHolder.StartTime = startTime;
                    return returnProcessHolder;
                }
                return null;
            }
            catch (Exception)
            {
            }
            return returnProcessHolder;
        }

        ProcHolder(EnvDTE80.Process2 prc)
        {
            try
            {
                Process = prc;

                using (var mos = new ManagementObjectSearcher(
                    @"\\{0}\root\cimv2".FormatWith(prc.TransportQualifier.Replace("Geeks@", "")),
                    "SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + prc.ProcessID))
                {
                    foreach (var mo in mos.Get())
                    {
                        var commandLine = mo["CommandLine"];
                        if (commandLine != null)
                        {
                            var cmdLineStr = commandLine.ToString();

                            if (ExcludedProcessesManager.CheckCommandLine(cmdLineStr.ToLower()))
                            {
                                Process = null;
                                return;
                            }

                            var appPool = GetAppPool(commandLine.ToString());

                            if (appPool != null) AppPool += appPool;
                        }
                    }
                }

                if (AppPool == null) AppPool = Path.GetFileName(Process.Name);
            }
            catch (Exception)
            {
            }
        }

        public override string ToString()
        {
            if (Process == null) return "NULL";

            if (StartTime.HasValue == false) return string.Format("{1} ({0})", Process.ProcessID, AppPool, Process.TransportQualifier).Replace("Geeks@", "");
            return string.Format("{1} ({0}) [{2}] [ {3} ]", Process.ProcessID, AppPool, MSharp::System.MSharpExtensions.ToTimeDifferenceString(StartTime.Value, 1), StartTime.Value.ToLongTimeString()).Replace("Geeks@", "");
        }
        string GetAppPool(string fullCommandLine)
        {
            // for IIS
            var match = Regex.Match(fullCommandLine, @"-ap ""(?<pool>[^""]*)""");
            if (match.Success)
                return match.Groups["pool"].Value.Trim();

            var args = Utils.SplitCommandLine(fullCommandLine);

            // for IIS express
            var siteArg = args.FirstOrDefault(a => a.StartsWith("/site:"));
            var configArg = args.FirstOrDefault(a => a.StartsWith("/config:"));

            if (siteArg != null && configArg != null)
            {
                var site = siteArg.TrimStart("/site:").TrimMatchingQuotes();
                var configFile = configArg.TrimStart("/config:").TrimMatchingQuotes();
                try
                {
                    // find physical path to this site from configuration file
                    var xconfig = XDocument.Load(configFile);
                    var siteNode = xconfig.XPathSelectElement("//site[@name='" + site + "']");
                    var physicalPath = siteNode.Element("application").Element("virtualDirectory").Attribute("physicalPath").Value;

                    return site;
                }
                catch
                {
                    // just ignore
                }
            }

            return null;
        }
    }
}