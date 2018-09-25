namespace Luminator.Motion.Protran.XimpleProtocol
{
    using System;

    public class BadXimpleEventArgs : EventArgs
    {
        public BadXimpleEventArgs(string xml)
        {
            this.Xml = xml;
        }

        public string Xml { get; private  set; }
    }
}