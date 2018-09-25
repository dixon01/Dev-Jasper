using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luminator.Multicast.Core
{
    public class MulticastCommandEventArgs : EventArgs
    {
        public MulticastCommands CommandToExecute { get; }

        public MulticastCommandEventArgs(MulticastCommands commandToExecute)
        {
            this.CommandToExecute = commandToExecute;
        }
    }
}
