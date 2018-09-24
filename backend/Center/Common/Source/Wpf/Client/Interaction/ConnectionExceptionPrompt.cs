// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionExceptionPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The connection exception prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Interaction
{
    using System;
    using System.ServiceModel;

    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// The connection exception prompt.
    /// </summary>
    public class ConnectionExceptionPrompt : PromptNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionExceptionPrompt"/> class.
        /// </summary>
        /// <param name="exception">
        /// The exception to be shown.
        /// </param>
        /// <param name="message">
        /// The optional message shown before the exception.
        /// </param>
        /// <param name="title">
        /// The title of the prompt.
        /// </param>
        public ConnectionExceptionPrompt(Exception exception, string message = null, string title = null)
        {
            this.Message = message ?? string.Empty;
            this.Title = title ?? Strings.ConnectionException_DefaultTitle;
            this.Exception = exception;

            var fault = exception.InnerException as FaultException;
            if (fault != null)
            {
                this.Message += Environment.NewLine + fault.Message;
            }
        }

        /// <summary>
        /// Gets the message shown before the exception.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}