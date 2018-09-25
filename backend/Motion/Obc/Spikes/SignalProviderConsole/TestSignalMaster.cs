// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSignalMaster.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestSignalMaster type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SignalProviderConsole
{
    using System;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Edi.Core;

    /// <summary>
    /// The test signal master.
    /// </summary>
    public class TestSignalMaster
    {
        /// <summary>
        /// Gets or sets the <see cref="evSetService"/> message.
        /// </summary>
        public evSetService SetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="evServiceStarted"/> message.
        /// </summary>
        public evServiceStarted ServiceStarted { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="evServiceEnded"/> message.
        /// </summary>
        public evServiceEnded ServiceEnded { get; set; }

        /// <summary>
        /// Prints help.
        /// </summary>
        internal void PrintHelp()
        {
            Console.WriteLine("Following commands are case insensitive.");
            Console.WriteLine("to quit type 'q' or 'quit'");
            Console.WriteLine("to send xyz message type 'xyz', e.g. for 'evSetService' type 'evSetService'");
            Console.WriteLine("then the program will ask for message specific parameters.");
            Console.WriteLine("Available messages are: 'evSetService', 'evServiceStarted' ");
            Console.WriteLine("    'evServiceEnded', 'evDeviationStarted', 'evDeviationEnded',");
            Console.WriteLine("    and 'evDeviationDetected'");
            Console.WriteLine("For help type 'h' or 'help'.");
        }

        /// <summary>
        /// The get set service parameters.
        /// </summary>
        internal void GetSetServiceParameters()
        {
            Console.WriteLine("Enter Umlauf number");
            string str = Console.ReadLine();
            int umlauf;
            bool flag = int.TryParse(str, out umlauf);
            if (!flag)
            {
                Console.WriteLine("You entered non numeric string");
            }

            this.SetService = new evSetService() { Umlauf = umlauf };
        }

        /// <summary>
        /// The send <see cref="evSetService"/> message.
        /// </summary>
        internal void SendEvSetServiceMessage()
        {
            if (this.SetService != null)
            {
                MessageDispatcher.Instance.Broadcast(this.SetService);
                Console.WriteLine("Message sent");
            }
        }

        /// <summary>
        /// The get service started parameters.
        /// </summary>
        internal void GetServiceStartedParameters()
        {
            Console.WriteLine("Enter Service number");
            string str = Console.ReadLine();
            int service;
            bool flag = int.TryParse(str, out service);
            if (!flag)
            {
                Console.WriteLine("You entered non-numeric string. I stop sending message");
                return;
            }

            Console.WriteLine("Is this ExtraService?");
            str = Console.ReadLine();
            bool extraService;
            flag = bool.TryParse(str, out extraService);
            if (!flag)
            {
                Console.WriteLine("You entered non-boolean string. I stop sending message");
                return;
            }

            Console.WriteLine("Is this School driving?");
            str = Console.ReadLine();
            bool schoolDriving;
            flag = bool.TryParse(str, out schoolDriving);
            if (!flag)
            {
                Console.WriteLine("You entered non-boolean string. I stop sending message");
                return;
            }

            Console.WriteLine("Is this extension course?");
            str = Console.ReadLine();
            bool extensionCourse;
            flag = bool.TryParse(str, out extensionCourse);
            if (!flag)
            {
                Console.WriteLine("You entered non-boolean string. I stop sending message");
                return;
            }

            this.ServiceStarted = new evServiceStarted()
                                      {
                                          Service = service,
                                          ExtraService = extraService,
                                          School = schoolDriving,
                                          ExtensionCourse = extensionCourse
                                      };
        }

        /// <summary>
        /// The send <see cref="evServiceStarted" /> message.
        /// </summary>
        internal void SendEvServiceStartedMessage()
        {
            MessageDispatcher.Instance.Broadcast(this.ServiceStarted);
            Console.WriteLine("Message sent");
        }

        /// <summary>
        /// The send <seealso cref="evServiceEnded"/> message.
        /// </summary>
        internal void SendEvServiceEndedMessage()
        {
            MessageDispatcher.Instance.Broadcast(new evServiceEnded());
            Console.WriteLine("Message sent");
        }

        /// <summary>
        /// The send <seealso cref="evDeviationStarted"/> message.
        /// </summary>
        internal void SendEvDeviationStartedMessage()
        {
            MessageDispatcher.Instance.Broadcast(new evDeviationStarted());
            Console.WriteLine("Message sent");
        }

        /// <summary>
        /// The send <seealso cref="evDeviationEnded"/> message.
        /// </summary>
        internal void SendEvDeviationEndedMessage()
        {
            MessageDispatcher.Instance.Broadcast(new evDeviationEnded());
            Console.WriteLine("Message sent");
        }

        /// <summary>
        /// The send <see cref="evDeviationDetected"/> message.
        /// </summary>
        internal void SendEvDeviationDetectedMessage()
        {
            MessageDispatcher.Instance.Broadcast(new evDeviationDetected());
            Console.WriteLine("Message sent");
        }
    }
}
