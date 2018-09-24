// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralDataReceivedEventArgs.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    public class PeripheralAudioSwitchDataReceivedEventArgs : PeripheralDataReceivedEventArgs
    {
        public PeripheralAudioSwitchDataReceivedEventArgs(byte[] data)
            : base(data)
        {            
        }

        public PeripheralAudioSwitchDataReceivedEventArgs(object message)
            : base(message)
        {
        }

        public PeripheralAudioSwitchMessageType MessageType
        {
            get
            {
                return this.AudioSwitchMessage.Header.MessageType;
            }
        }
        
        public PeripheralBaseMessageType<PeripheralAudioSwitchMessageType> AudioSwitchMessage
        {
            get
            {
                return base.Message as PeripheralBaseMessageType<PeripheralAudioSwitchMessageType>;
            }
            set
            {
                base.Message = value;
            }
        }
    }

    public class PeripheralDimmerDataReceivedEventArgs : PeripheralDataReceivedEventArgs
    {
        public PeripheralDimmerDataReceivedEventArgs(byte[] data)
            : base(data)
        {
        }

        public PeripheralDimmerDataReceivedEventArgs(object message)
            : base(message)
        {
        }

    }

    /// <summary>The peripheral data received event args.</summary>
    public class PeripheralDataReceivedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralDataReceivedEventArgs"/> class.</summary>
        /// <param name="data">The data.</param>
        public PeripheralDataReceivedEventArgs(byte[] data)
        {
            this.Data = data;
            this.IsValid = false;
            this.Message = null;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralDataReceivedEventArgs"/> class.</summary>
        /// <param name="message">The message.</param>
        public PeripheralDataReceivedEventArgs(object message)
        {
            this.Message = message;
            this.IsValid = true;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the data.</summary>
        public byte[] Data { get; set; }

        /// <summary>Gets or sets a value indicating whether is valid.</summary>
        public bool IsValid { get; set; }

        /// <summary>Gets or sets the message.</summary>
        public virtual object Message { get; set; }

 
        [Obsolete("To be deprecated")]
        public IPeripheralBaseMessageType<TMessageType> GetPeripheralMessage<TMessageType>()
        {
            var baseMessage = this.Message as IPeripheralBaseMessageType<TMessageType>;
            return baseMessage;
        }

        public bool IsMessage<TMessageType>(TMessageType messageType)
        {
            var baseMessage = this.Message as IPeripheralBaseMessageType<TMessageType>;
            return baseMessage != null && baseMessage.Header.IsMessageTye(messageType);
        }
        #endregion
    }

    /// <summary>The peripheral invalid message.</summary>
    public class PeripheralInvalidMessage : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PeripheralInvalidMessage"/> class.</summary>
        /// <param name="message">The message.</param>
        public PeripheralInvalidMessage(string message = "")
        {
            this.Message = message;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the message.</summary>
        public string Message { get; set; }

        #endregion
    }
}