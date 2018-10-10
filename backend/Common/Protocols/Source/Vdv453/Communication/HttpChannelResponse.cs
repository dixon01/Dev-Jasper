// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpChannelResponse.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpChannelResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Communication
{
    using System.Diagnostics.Contracts;
    using System.Net;

    /// <summary>
    /// Contains the meaningful properties of a response handled by the <see cref="HttpChannel"/>.
    /// </summary>
    public class HttpChannelResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpChannelResponse"/> class.
        /// </summary>
        public HttpChannelResponse()
        {
            this.IsFaulted = true;
        }

        /// <summary>
        /// Gets a value indicating whether the response is faulted (<c>true</c> by default).
        /// A response is &quot;faulted&quot; when it was not possible to correctly handle/parse it.
        /// </summary>
        /// <value>
        /// <c>true</c> if the response is faulted; otherwise, <c>false</c>.
        /// </value>
        public bool IsFaulted { get; private set; }

        /// <summary>
        /// Gets the status code.
        /// </summary>
        public HttpStatusCode? StatusCode { get; private set; }

        /// <summary>
        /// Gets or sets the status description.
        /// </summary>
        /// <value>
        /// The status description.
        /// </value>
        public string StatusDescription { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; set; }

        /// <summary>
        /// Sets the status. It also resets the <see cref="IsFaulted"/> flag.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="statusDescription">The status description.</param>
        public void SetStatus(HttpStatusCode httpStatusCode, string statusDescription)
        {
            Contract.Ensures(this.StatusCode.HasValue, "The status code must be set before leaving the method");
            this.IsFaulted = false;
            this.StatusCode = httpStatusCode;
            this.StatusDescription = statusDescription;
        }

        [ContractInvariantMethod]
        private void ObjectInvariants()
        {
            Contract.Invariant(this.IsFaulted || this.StatusCode.HasValue);
        }
    }
}