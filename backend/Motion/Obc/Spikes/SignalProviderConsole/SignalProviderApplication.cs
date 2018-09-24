// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalProviderApplication.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
// The program get a console input and sends a Medi message.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SignalProviderConsole
{
    using System;
    using System.Xml.Schema;

    using Gorba.Common.SystemManagement.Host;

    /// <summary>
    /// The signal provider application.
    /// </summary>
    public class SignalProviderApplication : ApplicationBase
    {
        /// <summary>
        /// The management name used in Medi and System Manager.
        /// </summary>
        public static readonly string ManagementName = "SignalProvider";

        private readonly TestSignalMaster testSignalMaster = new TestSignalMaster();

        /// <summary>
        /// Implementation of the <see cref="ApplicationBase.Run"/> method.
        /// The application's main loop is here.
        /// </summary>
        protected override void DoRun()
        {
            var continueLoop = true;
            Console.Write("For help type 'help' or 'h'");

            while (continueLoop)
            {
                var str = Console.ReadLine();

                try
                {
                    switch (str.ToLower())
                    {
                        case "q":
                            continueLoop = false;
                            break;
                        case "quit":
                            continueLoop = false;
                            break;
                        case "h":
                            this.testSignalMaster.PrintHelp();
                            break;
                        case "help":
                            this.testSignalMaster.PrintHelp();
                            break;
                        case "evsetservice":
                            this.testSignalMaster.GetSetServiceParameters();
                            this.testSignalMaster.SendEvSetServiceMessage();
                            break;
                        case "evservicestarted":
                            this.testSignalMaster.GetServiceStartedParameters();
                            this.testSignalMaster.SendEvServiceStartedMessage();
                            break;
                        case "evserviceended":
                            this.testSignalMaster.SendEvServiceEndedMessage();
                            break;
                        case "evdeviationstarted":
                            this.testSignalMaster.SendEvDeviationStartedMessage();
                            break;
                        case "evdeviationended":
                            this.testSignalMaster.SendEvDeviationEndedMessage();
                            break;
                        case "evdeviationdetected":
                            this.testSignalMaster.SendEvDeviationDetectedMessage();
                            break;
                        default:
                            Console.WriteLine("I don't do anything");
                            break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// The do stop. Does nothing.
        /// </summary>
        protected override void DoStop()
        {
        }
    }
}