using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gorba.Common.Protocols.Core
{
    public abstract class ProtocolLayer
    {
        private ProtocolLayer m_LowerLayer;
        public ProtocolLayer LowerLayer
        {
            get { return m_LowerLayer; }
            set { m_LowerLayer = value; }
        }

        private ProtocolLayer m_UpperLayer;
        public ProtocolLayer UpperLayer
        {
            get { return m_UpperLayer; }
            set { m_UpperLayer = value; }
        }

        public ProtocolLayer()
        {
            m_LowerLayer = null;
            m_UpperLayer = null;
        }

        public abstract void Transmit(ref ProtocolPacket packet);
        public abstract void HandleReceive(ref ProtocolPacket packet);

    }
}
