using Geeks.VSIX.SmartAttach.Attacher;
using GeeksAddin;
using System.Collections.Generic;

namespace Geeks.VSIX.SmartAttach
{
    public static class All
    {
        static List<Gadget> gadgets;
        public static List<Gadget> Gadgets
        {
            get
            {
                if (gadgets == null)
                {
                    gadgets = new List<Gadget>();
                    gadgets.Add(new AttacherGadget());
                }

                return gadgets;
            }
        }
    }
}