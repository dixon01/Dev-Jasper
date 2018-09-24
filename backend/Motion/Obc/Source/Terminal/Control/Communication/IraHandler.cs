// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IraHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IraHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Communication
{
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Edi.Core;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The iqube radio handler.
    /// </summary>
    internal class IraHandler
    {
        private readonly IContext context;

        private IraCallState callState = IraCallState.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="IraHandler"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public IraHandler(IContext context)
        {
            this.context = context;

            MessageDispatcher.Instance.Subscribe<evSpeechConnected>(this.EvSpeechConnectedEvent);
            MessageDispatcher.Instance.Subscribe<evSpeechDisconnected>(this.EvSpeechDisconnectedEvent);
            MessageDispatcher.Instance.Subscribe<evSpeechRequested>(this.EvSpeechRequestedEvent);
        }

        /// <summary>
        /// The IRA call states.
        /// </summary>
        internal enum IraCallState
        {
            /// <summary>
            /// No call.
            /// </summary>
            None = 0,

            /// <summary>
            /// Call building up.
            /// </summary>
            Building = 1,

            /// <summary>
            /// In a call.
            /// </summary>
            InCall = 2,
        }

        /// <summary>
        /// Gets the call state.
        /// </summary>
        /// <returns>
        /// The <see cref="IraCallState"/>.
        /// </returns>
        public IraCallState GetCallState()
        {
            return this.callState;
        }

        /// <summary>
        /// The send disconnect.
        /// </summary>
        /// <returns>
        /// True if disconnected.
        /// </returns>
        public bool SendDisconnect()
        {
            if (this.callState == IraCallState.None)
            {
                return false;
            }

            // In a call or building a call
            MessageDispatcher.Instance.Broadcast(new evSpeechDisconnected(1));
            return true;
        }

        private void EvSpeechRequestedEvent(object sender, MessageEventArgs<evSpeechRequested> e)
        {
            this.callState = IraCallState.Building;
        }

        private void EvSpeechDisconnectedEvent(object sender, MessageEventArgs<evSpeechDisconnected> e)
        {
            // check the IraEnums.ACK for the type...
            this.callState = IraCallState.None;

            switch (e.Message.Response)
            {
                    // [wes] todo: why this constant '7'?
                case 7:
                    this.context.Screen.HideProgressBar();
                    this.context.Screen.ShowMessageBox(
                        new MessageBoxInfo(
                            ml.ml_string(135, "No free Channel"),
                            ml.ml_string(136, "The Radio channel is in use. Try again later"),
                            MessageBoxInfo.MsgType.Info));
                    break;
            }
        }

        private void EvSpeechConnectedEvent(object sender, MessageEventArgs<evSpeechConnected> e)
        {
            this.callState = IraCallState.InCall;
        }
    }
}