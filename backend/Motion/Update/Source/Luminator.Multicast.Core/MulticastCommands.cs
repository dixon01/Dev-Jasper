using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminator.Multicast.Core
{
    using System.ComponentModel;

    public enum MulticastCommands
    {
        [Description("None")]
        None,

        [Description("Update TFT Displays")]
        Update,

        [Description("Show TFT Informational Display")]
        ShowDisplay
    }
}
