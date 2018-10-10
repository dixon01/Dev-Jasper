// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageBoxInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageBoxInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The message box information.
    /// </summary>
    public class MessageBoxInfo
    {
        private readonly string caption;
        private readonly string message;
        private readonly MsgType type = MsgType.Info;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxInfo"/> class.
        /// </summary>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public MessageBoxInfo(string caption, string message, MsgType type)
        {
            this.caption = caption;
            this.message = message;
            this.type = type;
        }

        /// <summary>
        /// The closed event.
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// The message type.
        /// </summary>
        public enum MsgType
        {
            /// <summary>
            /// The info.
            /// </summary>
            Info = 0,

            /// <summary>
            /// The warning.
            /// </summary>
            Warning = 1,

            /// <summary>
            /// The error.
            /// </summary>
            Error = 2,
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message
        {
            get
            {
                return this.message;
            }
        }

        /// <summary>
        /// Gets the caption.
        /// </summary>
        public string Caption
        {
            get
            {
                return this.caption;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public MsgType Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Raises the <see cref="Closed"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public void OnClosed(EventArgs e)
        {
            if (this.Closed != null)
            {
                this.Closed(this, e);
            }
        }
    }
}