// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorDialogContent.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;

    /// <summary>
    /// The error dialog content.
    /// </summary>
    public class ErrorDialogContent : DataViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDialogContent"/> class.
        /// </summary>
        /// <param name="message">
        /// The message shown above the exception.
        /// </param>
        /// <param name="navigateUri">
        /// The Uri where the hyperlink should navigate to.
        /// </param>
        /// <param name="exception">
        /// The exception to show.
        /// </param>
        /// <param name="hyperlinkText">
        /// The text shown as hyper link.
        /// </param>
        public ErrorDialogContent(string message, Uri navigateUri, Exception exception, string hyperlinkText)
        {
            this.Message = message;
            this.NavigateUri = navigateUri;
            this.Exception = exception;
            this.HyperlinkText = hyperlinkText;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the navigate uri.
        /// </summary>
        public Uri NavigateUri { get; private set; }

        /// <summary>
        /// Gets the hyperlink text.
        /// </summary>
        public string HyperlinkText { get; private set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets the navigate command.
        /// </summary>
        public ICommand NavigateCommand
        {
            get
            {
                return new RelayCommand(
                    () =>
                        {
                            if (this.NavigateUri != null)
                            {
                                Process.Start(this.NavigateUri.AbsoluteUri);
                            }
                        });
            }
        }
    }
}
