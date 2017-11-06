using EnvDTE80;
using GeeksAddin;

namespace Geeks.VSIX.SmartAttach.Attacher
{
    internal class AttacherGadget : Gadget
    {
        public AttacherGadget()
        {
            Name = "Attacher";
            Title = "Attacher";
            ShortKey = "CTRL+ALT+P";
        }

        public override void Run(DTE2 app)
        {
            var frm = new FormAttacher(app);
            frm.ShowDialog();
        }
    }
}
