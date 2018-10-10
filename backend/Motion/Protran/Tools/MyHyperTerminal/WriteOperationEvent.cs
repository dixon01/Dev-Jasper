using System;
using System.Collections.Generic;
using System.Text;

namespace MyHyperTerminal
{
    public class WriteOperationEvent
    {
        #region VARIABLES
        private bool isAscii;

        private string text;
        #endregion VARIABLES

        #region PROPERTIES
        public bool IsAscii
        {
            get { return this.isAscii; }
        }

        public string Text
        {
            get { return this.text; }
        }
        #endregion PROPERTIES

        public WriteOperationEvent(string text, bool isAscii)
        {
            this.text = text;
            this.isAscii = isAscii;
        }
    }
}
