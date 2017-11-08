using GeeksAddin;
using System;
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
        public EnvDTE80.Process2 Process { get; private set; }
        public string AppPool { get; private set; }
        public DateTime? StartTime { get; private set; } = null;

        public override string ToString()
        {
            if (Process == null)
                return "NULL";

            if (StartTime.HasValue == false) return String.Format("{1} ({0})", Process.ProcessID, AppPool, Process.TransportQualifier).Replace("Geeks@", "");
            return String.Format("{1} ({0}) [started {2}] [ {3} ]", Process.ProcessID, AppPool, MSharp::System.MSharpExtensions.ToTimeDifferenceString(StartTime.Value), StartTime.Value.ToLongTimeString()).Replace("Geeks@", "");
        }

        /// <summary>
        /// Creates a new ProcHolder instance.
        /// </summary>
        public ProcHolder(EnvDTE80.Process2 prc)
        {
            if (prc == null)
                return;
            Process = prc;
            try
            {
                using (var mos = new ManagementObjectSearcher(
                    @"\\{0}\root\cimv2".FormatWith(prc.TransportQualifier.Replace("Geeks@", "")),
                    "SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + prc.ProcessID))
                {
                    foreach (var mo in mos.Get())
                    {
                        var commandLine = mo["CommandLine"];
                        if (commandLine != null)
                        {
                            AppPool += GetAppPool(commandLine.ToString());
                        }
                    }
                }
            }
            catch (Exception)
            {
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

            var tp = System.Diagnostics.Process.GetProcessById(Process.ProcessID);
            if (tp != null)
            {
                StartTime = tp.StartTime;
            }

            return Path.GetFileName(Process.Name);
        }
    }
}
