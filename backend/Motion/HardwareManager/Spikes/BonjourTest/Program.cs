using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BonjourTest
{
    using System.Windows.Forms;

    using DNSServiceBrowser_NET;

    using SimpleChat.NET;

    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.Run(new Form1());
            Application.Run(new Form2());
            //Application.Run(new SimpleChat());
        }
    }
}
