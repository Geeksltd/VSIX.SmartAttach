using EnvDTE;
using EnvDTE80;
using Geeks.VSIX.SmartAttach.Base;
using Geeks.VSIX.SmartAttach.Properties;
using GeeksAddin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Geeks.VSIX.SmartAttach.Attacher
{
    public partial class FormAttacher : Form
    {
        static Dictionary<string, object> GlobalAddinCache = new Dictionary<string, object>();

        DTE2 DTE { get; set; }
        public FormAttacher(DTE2 dte)
        {
            InitializeComponent();
            DTE = dte;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            ReloadListOfRemoteMachines();
            ProcessListLoader.RunWorkerAsync();
        }

        void ReloadListOfRemoteMachines()
        {
            lstRemoteMachines.Items.Clear();
            var machinesString = Settings.Default.RemoteMachines;
            if (!machinesString.HasValue())
                return;

            var machines = GetRemoteMachineNames(machinesString);
            foreach (var m in machines)
                lstRemoteMachines.Items.Add(m);
        }

        void RefreshList()
        {
            listBoxProcess.SafeAction(l => l.Items.Clear());

            var solutionName = Utils.GetSolutionName(DTE).ToLower();

            var nominatedForSelection = -1;
            var lengthOfLastNomination = 0;

            var index = 0;
            using (var iis = new IIS())
            {
                foreach (ProcHolder holder in GetWorkerProcesses().OfType<EnvDTE80.Process2>().Select(proc => new ProcHolder(proc)).OrderByDescending(proc => proc.StartTime))
                {
                    if ((checkBoxExcludeMSharp.Checked && holder.AppPool != null && !holder.AppPool.Contains("M#")) || (!checkBoxExcludeMSharp.Checked))
                    {
                        listBoxProcess.SafeAction(l => l.Items.Add(holder));
                    }

                    if (solutionName.HasValue())
                    {
                        var physicalPath = iis.GetPhysicalPath(holder.AppPool);
                        if (physicalPath != null && physicalPath.ToLower().Contains(solutionName))
                        {
                            if (nominatedForSelection == -1)
                            {
                                nominatedForSelection = index;
                                lengthOfLastNomination = holder.AppPool.Length;
                            }
                            else if (holder.AppPool.Length < lengthOfLastNomination)
                            {
                                nominatedForSelection = index;
                                lengthOfLastNomination = holder.AppPool.Length;
                            }
                        }
                    }

                    index++;
                }
            }

            var count = listBoxProcess.SafeGet(() => listBoxProcess.Items.Count);
            if (count == 0)
                btnAttachToAll.SafeAction(b => b.Enabled = false);

            this.SafeAction(f => f.ActiveControl = listBoxProcess);
            if (nominatedForSelection <= count - 1)
                listBoxProcess.SafeAction(l => l.SelectedIndex = nominatedForSelection);

            DTE.StatusBar.Text = "Ready.";
            lblStatus.SafeAction(statusBar, s => s.Text = "");
        }

        static readonly string[] WebServerProcessNames = new[] { "w3wp", "iisexpress.exe" };

        IEnumerable<EnvDTE.Process> GetWorkerProcesses()
        {
            lblStatus.SafeAction(statusBar, s => s.Text = "Loading Local processes...");

            foreach (EnvDTE.Process p in DTE.Debugger.LocalProcesses)
                // if (WebServerProcessNames.Any(n => p.Name.IndexOf(n) >= 0))
                yield return p;

            var machinesString = Settings.Default.RemoteMachines;
            if (machinesString.HasValue())
            {
                var debuggerTwo = DTE.Debugger as EnvDTE80.Debugger2;
                var transport = debuggerTwo.Transports.Item("Default");

                var machines = GetRemoteMachineNames(machinesString);
                foreach (var machine in machines)
                {
                    lblStatus.SafeAction(statusBar, s => s.Text = "Loading {0} processes...".FormatWith(machine));
                    var cacheKey = "W3WP" + machine;

                    Processes processes = null;
                    try
                    {
                        if (GlobalAddinCache.ContainsKey(cacheKey))
                        {
                            processes = GlobalAddinCache[cacheKey] as Processes;
                        }
                        else
                        {
                            processes = debuggerTwo.GetProcesses(transport, machine);
                            GlobalAddinCache[cacheKey] = processes;
                        }
                    }
                    catch (Exception err)
                    {
                        DTE.StatusBar.Text = err.Message;
                        lblStatus.SafeAction(statusBar, s => s.Text = err.Message);
                    }

                    if (processes != null)
                    {
                        foreach (EnvDTE.Process p in processes)
                        {
                            if (WebServerProcessNames.Any(n => p.Name.IndexOf(n) >= 0))
                                yield return p;
                        }
                    }
                }
            }
        }

        static IOrderedEnumerable<string> GetRemoteMachineNames(string machinesString)
        {
            return machinesString.Split('|').Where(m => m.HasValue()).Select(m => m.ToUpper()).Distinct().OrderBy(m => m);
        }

        void btnAttach_Click(object sender, EventArgs e) => AttachToSelected();

        void AttachToSelected()
        {
            DisableAllButtons();
            foreach (ProcHolder holder in listBoxProcess.SelectedItems)
                if (holder.Process != null)
                    holder.Process.Attach();
            Close();
        }

        void KillSelected()
        {
            foreach (ProcHolder holder in listBoxProcess.SelectedItems)
            {
                if (holder.Process != null)
                {
                    try
                    {
                        var prc = System.Diagnostics.Process.GetProcessById(holder.Process.ProcessID);
                        if (prc != null)
                        {
                            prc.Kill();
                        }
                        else
                        {
                            MessageBox.Show("Cannot find process with id " + holder.Process.ProcessID, "Error (on FormAttacher.cs line 191)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception err)
                    {
                        ErrorNotification.EmailError(err);
                    }
                }
            }

            System.Threading.Thread.Sleep(100);
            RefreshList();
        }

        void listBoxProcess_MouseDoubleClick(object sender, MouseEventArgs e) => AttachToSelected();

        void listBoxProcess_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnAttach.Enabled = listBoxProcess.SelectedItem != null;
        }

        void btnAttachToAll_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            foreach (ProcHolder holder in listBoxProcess.Items)
                if (holder.Process != null)
                {
                    holder.Process.Attach();
                }

            Close();
        }

        void btnOriginal_Click(object sender, EventArgs e)
        {
            Close();
            DTE.ExecuteCommand("Debug.AttachtoProcess", "");
        }

        void DisableAllButtons()
        {
            btnAttach.Enabled = false;
            btnAttachToAll.Enabled = false;
        }

        void FormAttacher_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape) Close();
            if (e.KeyData == Keys.F5) RefreshList();
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            ClearCache();
            RefreshList();
        }

        void ClearCache()
        {
            for (int i = GlobalAddinCache.Keys.Count - 1; i >= 0; i--)
            {
                var key = GlobalAddinCache.Keys.ElementAt(i);
                if (key.StartsWith("W3WP"))
                {
                    GlobalAddinCache.Remove(key);
                }
            }
        }

        void DeleteCurrentRemoteMachine()
        {
            if (lstRemoteMachines.SelectedIndex >= 0)
            {
                lstRemoteMachines.Items.RemoveAt(lstRemoteMachines.SelectedIndex);

                Settings.Default.RemoteMachines = "";
                foreach (var m in lstRemoteMachines.Items)
                    Settings.Default.RemoteMachines += m + "|";
                Settings.Default.Save();

                ReloadListOfRemoteMachines();
            }
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtMachineName.Text.HasValue())
            {
                Settings.Default.RemoteMachines += "|" + txtMachineName.Text;
                Settings.Default.Save();
                ReloadListOfRemoteMachines();
            }
        }

        void ProcessListLoader_DoWork(object sender, DoWorkEventArgs e) => RefreshList();

        void FilterItemsWithSearchTerm()
        {
            var searchTerm = txtSearchProcess.Text.ToLower();

            if (string.IsNullOrEmpty(searchTerm)) { RefreshList(); return; }

            for (int i = listBoxProcess.Items.Count - 1; i >= 0; i--)
            {
                var currentItem = listBoxProcess.Items[i].ToString().ToLower();
                if (currentItem.Contains(searchTerm)) continue;
                listBoxProcess.SafeAction(l => l.Items.RemoveAt(i));
            }
        }

        void txtSearchProcess_TextChanged(object sender, EventArgs e) => FilterItemsWithSearchTerm();

        void listBoxProcess_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Delete) return;

            if (tabControl.SelectedTab == tbpgWorkers) KillSelected();
            if (tabControl.SelectedTab == tbpgRemoteMachines) DeleteCurrentRemoteMachine();
        }

        void checkBoxExcludeMSharp_CheckedChanged(object sender, EventArgs e) => RefreshList();
    }
}