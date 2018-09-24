// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleResponse.cs" company="Luminator Technology Group">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.Motion.Protran.XimpleProtocol
{
    using System;

    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Utils;

    /// <summary>The ximple server response.</summary>
    [Serializable]
    public class XimpleResponse
    {
        /// <summary>Initializes a new instance of the <see cref="XimpleResponse"/> class.</summary>
        public XimpleResponse() : this(XimpleResonseType.Success)
        {
            this.Comment = string.Empty;
        }

        /// <summary>Initializes a new instance of the <see cref="XimpleResponse"/> class.</summary>
        /// <param name="ximpleResponseType">The ximple response type.</param>
        /// <param name="comment">The comment.</param>
        public XimpleResponse(XimpleResonseType ximpleResponseType, string comment = "")
        {
            this.XimpleResponseType = ximpleResponseType;
            this.Comment = comment;
            this.Ximple = new Ximple(Constants.Version2);
        }

        /// <summary>Initializes a new instance of the <see cref="XimpleResponse"/> class.</summary>
        /// <param name="ximple">The ximple.</param>
        public XimpleResponse(Ximple ximple) : this()
        {
            this.Ximple = ximple;
        }

        /// <summary>Gets or sets the ximple response type.</summary>
        public XimpleResonseType XimpleResponseType { get; set; }

        /// <summary>Gets or sets the comment.</summary>
        public string Comment { get; set; }

        /// <summary>Gets or sets the ximple.</summary>
        public Ximple Ximple { get; set; }
    }
}