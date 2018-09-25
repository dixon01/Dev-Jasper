namespace Luminator.PeripheralProtocol.Core
{
    using System.Threading;

    using Luminator.PeripheralProtocol.Core.Interfaces;

    /// <summary>The write model with ack.</summary>
    internal class WriteModelWithAck
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="WriteModelWithAck"/> class.</summary>
        /// <param name="peripheralMessage">The peripheral message.</param>
        public WriteModelWithAck(IPeripheralBaseMessage peripheralMessage)
        {
            this.PeripheralMessage = peripheralMessage;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the ack recieved event.</summary>
        public ManualResetEvent AckRecievedEvent { get; } = new ManualResetEvent(false);

        /// <summary>Gets the peripheral message.</summary>
        public IPeripheralBaseMessage PeripheralMessage { get; }

        #endregion
    }
}