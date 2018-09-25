namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Models
{
    using System.Net;

    using Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces;

    public class AdHocRegistrationResponse : AdHocResponse, IAdHocRegistrationResponse
    {
        /// <summary>Initializes a new instance of the <see cref="AdHocRegistrationResponse"/> class.</summary>
        /// <param name="response">The response.</param>
        /// <param name="status">The status.</param>
        public AdHocRegistrationResponse(string response, HttpStatusCode status)
        {
            this.Response = response;
            this.Status = status;
        }

        /// <summary>Initializes a new instance of the <see cref="AdHocRegistrationResponse"/> class.</summary>
        public AdHocRegistrationResponse()
        {
            this.Response = string.Empty;
            this.Status = HttpStatusCode.OK;
        }
    }
}