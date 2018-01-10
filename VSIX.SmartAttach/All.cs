using System.Collections.Generic;
using Geeks.VSIX.SmartAttach.Attacher;
using GeeksAddin;

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