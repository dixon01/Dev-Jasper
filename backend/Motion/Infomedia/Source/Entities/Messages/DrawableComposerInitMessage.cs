using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gorba.Motion.Infomedia.Entities.Messages
{

    public enum DrawableStatus
    {
        Initialized,
        Rendering,
        Disposing
    }

    [Serializable]
    public class DrawableComposerInitMessage
    {
        public string UnitName { get; set; }

        public int ElementID { get; set; }

        public string ElementFileName { get; set; }

        public DrawableStatus Status { get; set; }
    }
}
