
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Management;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using GeeksAddin;

namespace Geeks.VSIX.SmartAttach.Attacher
{
	//extern alias MSharp;

	public class ProcHolder
	{
		public EnvDTE80.Process2 Process { get; private set; }
		public string AppPool { get; private set; }
		public DateTime? StartTime { get; private set; } = null;

		public string ProcessPath { get; private set; }

		static ExcludedProcessesManager excludedProcessesManager = new ExcludedProcessesManager();
		public static ExcludedProcessesManager ExcludedProcessesManager => excludedProcessesManager;

		static ConcurrentDictionary<int, ProcHolder> alreadyCheckedProcesses = new ConcurrentDictionary<int, ProcHolder>();

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

							var dotNetCoreApp = GetDotNetCoreApp(prc, commandLine.ToString());

							if (dotNetCoreApp != null) AppPool += dotNetCoreApp;
						}
					}
				}

				if (AppPool == null) AppPool = Path.GetFileName(Process.Name);
			}
			catch (Exception)
			{
			}
		}

		public static ProcHolder CheckAndAddProcHolder(EnvDTE80.Process2 process)
		{
			ProcHolder returnProcessHolder = null;

			try
			{
				if (process == null) return null;
				if (process.Name.Contains("dotnet.exe")) return null;
				var startTime = ExcludedProcessesManager.CheckAndReturnStartTime(process);
				var ProcessPath = process.Name;

				if (startTime == null) return null;

				if (alreadyCheckedProcesses.TryGetValue(process.ProcessID, out returnProcessHolder))
				{
					if (returnProcessHolder.Process.Name == process.Name)
					{
						returnProcessHolder.StartTime = startTime;
						returnProcessHolder.ProcessPath = ProcessPath;
						return returnProcessHolder;
					}
				}

				returnProcessHolder = new ProcHolder(process);
				if (returnProcessHolder.Process != null)
				{
					alreadyCheckedProcesses.TryAdd(process.ProcessID, returnProcessHolder);
					returnProcessHolder.StartTime = startTime;
					returnProcessHolder.ProcessPath = ProcessPath;
					return returnProcessHolder;
				}

				return null;
			}
			catch (Exception ex)
			{
			}

			return returnProcessHolder;
		}

		public override string ToString()
		{
			if (Process == null) return "NULL";

			try
			{
				if (AppPool.ToLower() != "website.exe")
					return (AppPool.PadRight(30, ' ') + Process.ProcessID.ToString().PadRight(8, ' ') + Process.Name).Replace("Geeks@", "");
				else
				{
					string ProcName = null;
					int EndIndex = Process.Name.ToLower().IndexOf(@"\website\bin\");

					if (EndIndex < 0)
					{
						return (AppPool.PadRight(30, ' ') + Process.ProcessID.ToString().PadRight(8, ' ') + Process.Name).Replace("Geeks@", "");
					}
					int StartIndex = EndIndex - 1;
					while (StartIndex > 0 && Process.Name[StartIndex - 1] != '\\')
					{
						StartIndex--;
					}
					ProcName = Process.Name.Substring(StartIndex, EndIndex - StartIndex);

					return ((string.Format("{0} ({1})", ProcName, Path.GetFileNameWithoutExtension(AppPool))).PadRight(30, ' ') + Process.ProcessID.ToString().PadRight(8, ' ') + Process.Name).Replace("Geeks@", "");
				}
			}
			catch
			{
				return "NULL";
			}

			//string Str2 = null;
			//if (StartTime.HasValue == false)
			//{
			//    return (AppPool.PadRight(20, ' ') + Process.ProcessID.ToString().PadRight(28, ' ') + Process.Name).Replace("Geeks@", "");
			//    //return string.Format("{1} ({0}) ({2}) ({3})", Process.ProcessID, AppPool, Process.TransportQualifier, Process.Name).Replace("Geeks@", "");
			//}
			//string Result = AppPool.PadRight(25, ' ');
			//try
			//{
			//    Str2 = string.Format("{0} {1} {2}", Process.ProcessID, MSharp::System.MSharpExtensions.ToTimeDifferenceString(StartTime.Value, 1)).Remove("Geeks@");
			//}
			//catch
			//{
			//    //Str2= string.Format("{1} ({0}) ({2}) ({3})", Process.ProcessID+ Str2.PadRight(35, ' '), AppPool+ Str2.PadRight(35, ' '), Process.TransportQualifier + Str2.PadRight(35, ' '), Process.Name).Replace("Geeks@", "");
			//    Str2 = (AppPool.PadRight(20, ' ') + Process.ProcessID.ToString().PadRight(28, ' ') + Process.Name).Replace("Geeks@", "");
			//    //Str2 = Str2.PadRight(20, ' ');
			//    //Str2 += Process.ProcessID;
			//    //Str2 = Str2.PadRight(28, ' ');
			//    //Str2 += Process.TransportQualifier + "   ";
			//    //Str2 += Process.Name;
			//    //Str2 = Str2.Replace("Geeks@", "");
			//    return Str2;
			//}
			//Str2 = Str2.PadRight(35, ' ');
			//Result += Str2;
			//Result += Process.Name;
			//return Result;
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

		string GetDotNetCoreApp(EnvDTE80.Process2 prc, string fullCommandLine)
		{
			if (fullCommandLine.Contains("dotnet.exe") == false) return null;
			var match = Regex.Match(fullCommandLine, @"dotnet.exe"".*exec.*""(.*)""");
			if (match.Success && match.Groups.Count >= 1)
			{
				var tp = System.Diagnostics.Process.GetProcessById(prc.ProcessID);

				string title = Path.GetFileName(match.Groups[1].Value.Trim());

				try
				{
					var windowtitle = tp.MainWindowTitle;

					if (!(string.IsNullOrEmpty(windowtitle) || string.IsNullOrWhiteSpace(windowtitle))) title += " >> " + windowtitle;
				}
				catch
				{

				}

				return $"dotnet[\"{title}\"]";
			}
			return null;
		}
	}
}