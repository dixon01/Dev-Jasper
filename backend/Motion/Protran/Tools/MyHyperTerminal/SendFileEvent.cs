using System;
using System.Collections.Generic;
using System.Text;

namespace MyHyperTerminal
{
    public class SendFileEvent
    {
        #region VARIABLES
        private byte[] buffer;
        private int bufferLength;
        #endregion VARIABLES

        #region PROPERTIES
        public byte[] Buffer
        {
            get { return this.buffer; }
        }

        public int BufferLength
        {
            get { return this.bufferLength; }
        }
        #endregion PROPERTIES

        public SendFileEvent( byte[] buffer, int bufferLength )
        {
            this.buffer = buffer;
            this.bufferLength = bufferLength;
        }
    }
}
