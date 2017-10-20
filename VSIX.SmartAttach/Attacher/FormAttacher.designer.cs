namespace GeeksAddin.Attacher
{
    partial class FormAttacher
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tbpgWorkers = new System.Windows.Forms.TabPage();
            this.txtSearchProcess = new System.Windows.Forms.TextBox();
            this.checkBoxExcludeMSharp = new System.Windows.Forms.CheckBox();
            this.btnOriginal = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnAttachToAll = new System.Windows.Forms.Button();
            this.btnAttach = new System.Windows.Forms.Button();
            this.listBoxProcess = new System.Windows.Forms.ListBox();
            this.tbpgRemoteMachines = new System.Windows.Forms.TabPage();
            this.lstRemoteMachines = new System.Windows.Forms.ListBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtMachineName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ProcessListLoader = new System.ComponentModel.BackgroundWorker();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl.SuspendLayout();
            this.tbpgWorkers.SuspendLayout();
            this.tbpgRemoteMachines.SuspendLayout();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tbpgWorkers);
            this.tabControl.Controls.Add(this.tbpgRemoteMachines);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(606, 363);
            this.tabControl.TabIndex = 4;
            // 
            // tbpgWorkers
            // 
            this.tbpgWorkers.Controls.Add(this.txtSearchProcess);
            this.tbpgWorkers.Controls.Add(this.checkBoxExcludeMSharp);
            this.tbpgWorkers.Controls.Add(this.btnOriginal);
            this.tbpgWorkers.Controls.Add(this.btnRefresh);
            this.tbpgWorkers.Controls.Add(this.btnAttachToAll);
            this.tbpgWorkers.Controls.Add(this.btnAttach);
            this.tbpgWorkers.Controls.Add(this.listBoxProcess);
            this.tbpgWorkers.Location = new System.Drawing.Point(4, 22);
            this.tbpgWorkers.Name = "tbpgWorkers";
            this.tbpgWorkers.Padding = new System.Windows.Forms.Padding(3);
            this.tbpgWorkers.Size = new System.Drawing.Size(598, 337);
            this.tbpgWorkers.TabIndex = 0;
            this.tbpgWorkers.Text = "Worker Procs";
            this.tbpgWorkers.UseVisualStyleBackColor = true;
            // 
            // txtSearchProcess
            // 
            this.txtSearchProcess.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtSearchProcess.Location = new System.Drawing.Point(8, 6);
            this.txtSearchProcess.Name = "txtSearchProcess";
            this.txtSearchProcess.Size = new System.Drawing.Size(496, 20);
            this.txtSearchProcess.TabIndex = 10;
            this.txtSearchProcess.TextChanged += new System.EventHandler(this.txtSearchProcess_TextChanged);
            // 
            // checkBoxExcludeMSharp
            // 
            this.checkBoxExcludeMSharp.AutoSize = true;
            this.checkBoxExcludeMSharp.Checked = true;
            this.checkBoxExcludeMSharp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxExcludeMSharp.Location = new System.Drawing.Point(510, 9);
            this.checkBoxExcludeMSharp.Name = "checkBoxExcludeMSharp";
            this.checkBoxExcludeMSharp.Size = new System.Drawing.Size(83, 17);
            this.checkBoxExcludeMSharp.TabIndex = 9;
            this.checkBoxExcludeMSharp.Text = "Exclude M#";
            this.checkBoxExcludeMSharp.UseVisualStyleBackColor = true;
            this.checkBoxExcludeMSharp.CheckedChanged += new System.EventHandler(this.checkBoxExcludeMSharp_CheckedChanged);
            // 
            // btnOriginal
            // 
            this.btnOriginal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOriginal.Location = new System.Drawing.Point(8, 306);
            this.btnOriginal.Name = "btnOriginal";
            this.btnOriginal.Size = new System.Drawing.Size(180, 23);
            this.btnOriginal.TabIndex = 8;
            this.btnOriginal.Text = "Original Attach to &Process...";
            this.btnOriginal.UseVisualStyleBackColor = true;
            this.btnOriginal.Click += new System.EventHandler(this.btnOriginal_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(520, 306);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(70, 23);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnAttachToAll
            // 
            this.btnAttachToAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAttachToAll.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAttachToAll.Location = new System.Drawing.Point(410, 306);
            this.btnAttachToAll.Name = "btnAttachToAll";
            this.btnAttachToAll.Size = new System.Drawing.Size(104, 23);
            this.btnAttachToAll.TabIndex = 6;
            this.btnAttachToAll.Text = "Attach to &All";
            this.btnAttachToAll.UseVisualStyleBackColor = true;
            this.btnAttachToAll.Click += new System.EventHandler(this.btnAttachToAll_Click);
            // 
            // btnAttach
            // 
            this.btnAttach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAttach.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAttach.Enabled = false;
            this.btnAttach.Location = new System.Drawing.Point(329, 306);
            this.btnAttach.Name = "btnAttach";
            this.btnAttach.Size = new System.Drawing.Size(75, 23);
            this.btnAttach.TabIndex = 5;
            this.btnAttach.Text = "A&ttach";
            this.btnAttach.UseVisualStyleBackColor = true;
            this.btnAttach.Click += new System.EventHandler(this.btnAttach_Click);
            // 
            // listBoxProcess
            // 
            this.listBoxProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxProcess.FormattingEnabled = true;
            this.listBoxProcess.ItemHeight = 31;
            this.listBoxProcess.Location = new System.Drawing.Point(8, 37);
            this.listBoxProcess.Name = "listBoxProcess";
            this.listBoxProcess.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxProcess.Size = new System.Drawing.Size(582, 252);
            this.listBoxProcess.TabIndex = 4;
            this.listBoxProcess.SelectedIndexChanged += new System.EventHandler(this.listBoxProcess_SelectedIndexChanged);
            this.listBoxProcess.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxProcess_KeyDown);
            this.listBoxProcess.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBoxProcess_MouseDoubleClick);
            // 
            // tbpgRemoteMachines
            // 
            this.tbpgRemoteMachines.Controls.Add(this.lstRemoteMachines);
            this.tbpgRemoteMachines.Controls.Add(this.btnAdd);
            this.tbpgRemoteMachines.Controls.Add(this.txtMachineName);
            this.tbpgRemoteMachines.Controls.Add(this.label1);
            this.tbpgRemoteMachines.Location = new System.Drawing.Point(4, 22);
            this.tbpgRemoteMachines.Name = "tbpgRemoteMachines";
            this.tbpgRemoteMachines.Padding = new System.Windows.Forms.Padding(3);
            this.tbpgRemoteMachines.Size = new System.Drawing.Size(598, 337);
            this.tbpgRemoteMachines.TabIndex = 1;
            this.tbpgRemoteMachines.Text = "Remote Machines";
            this.tbpgRemoteMachines.UseVisualStyleBackColor = true;
            // 
            // lstRemoteMachines
            // 
            this.lstRemoteMachines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstRemoteMachines.FormattingEnabled = true;
            this.lstRemoteMachines.Location = new System.Drawing.Point(6, 33);
            this.lstRemoteMachines.Name = "lstRemoteMachines";
            this.lstRemoteMachines.Size = new System.Drawing.Size(590, 225);
            this.lstRemoteMachines.TabIndex = 3;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.Location = new System.Drawing.Point(523, 5);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtMachineName
            // 
            this.txtMachineName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMachineName.Location = new System.Drawing.Point(66, 7);
            this.txtMachineName.Name = "txtMachineName";
            this.txtMachineName.Size = new System.Drawing.Size(448, 20);
            this.txtMachineName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Machine:";
            // 
            // ProcessListLoader
            // 
            this.ProcessListLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ProcessListLoader_DoWork);
            // 
            // statusBar
            // 
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusBar.Location = new System.Drawing.Point(0, 363);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(606, 22);
            this.statusBar.TabIndex = 9;
            this.statusBar.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(42, 17);
            this.lblStatus.Text = "Ready.";
            // 
            // FormAttacher
            // 
            this.AcceptButton = this.btnAttach;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(606, 385);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusBar);
            this.KeyPreview = true;
            this.Name = "FormAttacher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Attach to w3wp processes";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormAttacher_KeyDown);
            this.tabControl.ResumeLayout(false);
            this.tbpgWorkers.ResumeLayout(false);
            this.tbpgWorkers.PerformLayout();
            this.tbpgRemoteMachines.ResumeLayout(false);
            this.tbpgRemoteMachines.PerformLayout();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        System.Windows.Forms.TabControl tabControl;
        System.Windows.Forms.TabPage tbpgWorkers;
        System.Windows.Forms.Button btnOriginal;
        System.Windows.Forms.Button btnRefresh;
        System.Windows.Forms.Button btnAttachToAll;
        System.Windows.Forms.Button btnAttach;
        System.Windows.Forms.ListBox listBoxProcess;
        System.Windows.Forms.TabPage tbpgRemoteMachines;
        System.Windows.Forms.Button btnAdd;
        System.Windows.Forms.TextBox txtMachineName;
        System.Windows.Forms.Label label1;
        System.Windows.Forms.ListBox lstRemoteMachines;
        System.ComponentModel.BackgroundWorker ProcessListLoader;
        System.Windows.Forms.StatusStrip statusBar;
        System.Windows.Forms.ToolStripStatusLabel lblStatus;
        System.Windows.Forms.TextBox txtSearchProcess;
        System.Windows.Forms.CheckBox checkBoxExcludeMSharp;
    }
}