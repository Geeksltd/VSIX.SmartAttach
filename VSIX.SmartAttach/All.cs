using System.Collections.Generic;
using GeeksAddin.Attacher;

namespace GeeksAddin
{
    public static class All
    {
        static List<Gadget> _Gadgets;
        public static List<Gadget> Gadgets
        {
            get
            {
                if (_Gadgets == null)
                {
                    _Gadgets = new List<Gadget>();
                    _Gadgets.Add(new AttacherGadget());
                }
                return _Gadgets;
            }
        }
    }
}
