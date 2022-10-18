using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE80;
using Geeks.VSIX.SmartAttach.Attacher;
using Geeks.VSIX.SmartAttach.Base;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace Geeks.VSIX.SmartAttach
{
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F", PackageAutoLoadFlags.BackgroundLoad)]    // Microsoft.VisualStudio.VSConstants.UICONTEXT_NoSolution
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideService(typeof(IMenuCommandService), IsAsyncQueryable = true)]

    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionsPage), "Geeks productivity tools", "General", 0, 0, true)]
    [Guid(GuidList.GuidGeeksProductivityToolsPkgString)]
    public sealed class SmartAttachPackage : AsyncPackage
    {
        public SmartAttachPackage() { }

        // Strongly reference events so that it's not GC'd
        EnvDTE.DocumentEvents docEvents;
        EnvDTE.SolutionEvents solEvents;
        EnvDTE.Events events;

        public static SmartAttachPackage Instance { get; private set; }

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await base.InitializeAsync(cancellationToken, progress);

            App.Initialize(GetDialogPage(typeof(OptionsPage)) as OptionsPage);


            Instance = this;

            var componentModel = (IComponentModel)await GetServiceAsync(typeof(SComponentModel));

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var menuCommandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            var cmdidWebFileToggleId = new CommandID(GuidList.GuidGeeksProductivityToolsCmdSet, 0x101);
            var cmdidAttacherId = new CommandID(GuidList.GuidGeeksProductivityToolsCmdSet, (int)PkgCmdIDList.CmdidAttacher);

            var otherMenu = menuCommandService.FindCommand(cmdidWebFileToggleId);

            if (otherMenu != null)
            {
            }

            if (null != menuCommandService)
            {
                var menuCommand = new OleMenuCommand(CallAttacher, cmdidAttacherId);
                menuCommand.BeforeQueryStatus += MenuCommand_BeforeQueryStatus;
                menuCommandService.AddCommand(menuCommand);
            }

            SetCommandBindings();

            // Hook up event handlers
            events = App.DTE.Events;
            docEvents = events.DocumentEvents;
            solEvents = events.SolutionEvents;
            docEvents.DocumentSaved += DocumentEvents_DocumentSaved;
            solEvents.Opened += delegate { App.Initialize(GetDialogPage(typeof(OptionsPage)) as OptionsPage); };

        }

        //    SetCommandBindings();

        //    // Hook up event handlers
        //    events = App.DTE.Events;
        //    docEvents = events.DocumentEvents;
        //    solEvents = events.SolutionEvents;
        //    docEvents.DocumentSaved += DocumentEvents_DocumentSaved;
        //    solEvents.Opened += delegate { App.Initialize(GetDialogPage(typeof(OptionsPage)) as OptionsPage); };
        //}

        void MenuCommand_BeforeQueryStatus(object sender, EventArgs e)
        {
            var cmd = sender as OleMenuCommand;
            // var activeDoc = App.DTE.ActiveDocument;

            // if (null != cmd && activeDoc != null)
            // {
            //    var fileName = App.DTE.ActiveDocument.FullName.ToUpper();
            //    cmd.Visible = true;
            // }
        }

        void DocumentEvents_DocumentSaved(EnvDTE.Document document)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (document.Name.EndsWith(".cs") ||
                    document.Name.EndsWith(".css") ||
                    document.Name.EndsWith(".js") ||
                    document.Name.EndsWith(".ts"))
                {
                    //document.DTE.ExecuteCommand("Edit.FormatDocument");
                }

                if (!document.Saved) document.Save();
            }
            catch
            {
            }
        }

        void SetCommandBindings()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var commands = (Commands2)App.DTE.Commands;
            foreach (EnvDTE.Command cmd in commands)
            {
                if (cmd.Name == "File.CloseAllButThis")
                    cmd.Bindings = "Global::CTRL+SHIFT+F4";

                foreach (var gadget in All.Gadgets)
                {
                    if (gadget.CommandName == cmd.Name)
                    {
                        cmd.Bindings = gadget.Binding;
                        break;
                    }
                }
            }
        }

        void CallAttacher(object sender, EventArgs e) => new AttacherGadget().Run(App.DTE);
    }
}