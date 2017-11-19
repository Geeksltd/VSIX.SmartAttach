using GeeksAddin;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Geeks.VSIX.SmartAttach.Attacher
{
    extern alias MSharp;

    public class ProcHolder
    {
        public static readonly string WebServer_W3WP_ProcessName = "w3wp";
        static readonly string[] WebServerProcessNames = new[] { WebServer_W3WP_ProcessName, "iisexpress.exe" };
        static readonly string[] ExcludedProcessNames = new[] { "ServiceHub".ToLower(), "Microsoft".ToLower(), "iisexpresstray", "devenv" };
        static readonly string[] ExcludedProcessNames_WithCommpandLine = new[] { "C:\\program files (x86)\\".ToLower() };

        public EnvDTE80.Process2 Process { get; private set; }
        public string AppPool { get; private set; }
        public DateTime? StartTime { get; private set; } = null;

        /// <summary>
        /// Creates a new ProcHolder instance.
        /// </summary>
        public ProcHolder(EnvDTE80.Process2 prc)
        {
            try
            {
                if (prc == null) return;
                var name = prc.Name;

                if (ExcludedProcessNames.Any(x => prc.Name.ToLower().StartsWith(x))) return;

                try
                {
                    var tp = System.Diagnostics.Process.GetProcessById(prc.ProcessID);

                    if (ExcludedProcessNames.Any(x => tp.ProcessName.ToLower().StartsWith(x))) return;

                    if (WebServerProcessNames.Any(n => tp.ProcessName.IndexOf(n) >= 0) == false && IsDotNetProcess(tp) == false) return;

                    StartTime = tp.StartTime;
                }
                catch
                {

                }

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

                            if (ExcludedProcessNames_WithCommpandLine.Any(x => cmdLineStr.ToLower().StartsWith(x)))
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
            return string.Format("{1} ({0}) [started {2}] [ {3} ]", Process.ProcessID, AppPool, MSharp::System.MSharpExtensions.ToTimeDifferenceString(StartTime.Value), StartTime.Value.ToLongTimeString()).Replace("Geeks@", "");
        }
        public static bool IsDotNetProcess(Process process)
        {
            try
            {
                var modules = process.Modules.Cast<ProcessModule>().Where(
                    m => m.ModuleName.StartsWith("mscor", StringComparison.InvariantCultureIgnoreCase));

                return modules.Any();
            }
            catch (Exception e)
            {
                return false;
            }
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