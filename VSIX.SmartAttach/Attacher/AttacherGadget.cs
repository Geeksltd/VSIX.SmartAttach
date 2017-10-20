using EnvDTE80;

namespace GeeksAddin.Attacher
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
