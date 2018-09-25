namespace Luminator.PeripheralProtocol.Core
{
    using System.Threading;

    using Luminator.PeripheralProtocol.Core.Interfaces;

    /// <summary>The write model with ack.</summary>
    internal class WriteModelWithAck<TMessageType> 
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="WriteModelWithAck"/> class.</summary>
        /// <param name="peripheralMessage">The peripheral message.</param>
        public WriteModelWithAck(IPeripheralBaseMessageType<TMessageType> peripheralMessage)
        {
            this.PeripheralMessage = peripheralMessage;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the ack received event.</summary>
        public ManualResetEvent AckRecievedEvent { get; } = new ManualResetEvent(false);

        /// <summary>Gets the peripheral message.</summary>
        public IPeripheralBaseMessageType<TMessageType> PeripheralMessage { get; }

        #endregion
    }
}