// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendMessage.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Medi.MediChatTest
{
    using System;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    ///     The send message.
    /// </summary>
    internal class SendMessage
    {
        private const int ConnectWaitTime = 750;

        private readonly StringMessage msg = new StringMessage();

        

        /// <summary>
        ///     The run.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Run()
        {
            // this.messageDispatcher = MessageDispatcher.Instance;
            // this.messageDispatcher.Configure(configurator);
            this.msg.Value = Console.ReadLine();
            if (this.msg.Value == "quit")
            {
                return false;
            }

            this.messageDispatcher.Broadcast(this.msg);
            return true;
        }

        /// <summary>
        /// The establish medi connection.
        /// </summary>
        /// <param name="configFileName">
        /// The config File Name.
        /// </param>
        

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void Disconnect()
        {
            this.messageDispatcher.Unsubscribe<StringMessage>(this.SubscribeEventHandler);
        }

        
    }
}