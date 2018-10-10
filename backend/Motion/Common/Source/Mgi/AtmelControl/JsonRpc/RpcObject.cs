// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RpcObject.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RpcObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl.JsonRpc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Base class for all objects defined in <see cref="http://www.jsonrpc.org/specification"/>.
    /// </summary>
    public abstract class RpcObject
    {
        private readonly string version;

        /// <summary>
        /// Initializes a new instance of the <see cref="RpcObject"/> class.
        /// </summary>
        internal RpcObject()
        {
            this.version = "2.0";
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Gets or sets the JSON-RPC version string.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.NamingRules",
            "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "JSON-RPC naming")]
        public string jsonrpc
        {
            get
            {
                return this.version;
            }

            set
            {
                if (value != this.version)
                {
                    throw new ArgumentException(
                        string.Format("Wrong jsonrpc, expected '{0}' but got '{1}'", this.version, value), "value");
                }
            }
        }

        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Converts the given (JSON) object to the requested object.
        /// </summary>
        /// <param name="obj">
        /// The object. It can be either a normal object or a <see cref="JObject"/>.
        /// </param>
        /// <typeparam name="T">
        /// The type to convert to.
        /// </typeparam>
        /// <returns>
        /// An object of type <see cref="T"/>. This might be the same
        /// as the object passed as an argument or it might be created
        /// by <see cref="JToken.ToObject{T}()"/>.
        /// </returns>
        internal static T Convert<T>(object obj)
        {
            if (obj == null)
            {
                return default(T);
            }

            if (obj is T)
            {
                return (T)obj;
            }

            var jobject = obj as JToken;
            if (jobject != null)
            {
                return jobject.ToObject<T>();
            }

            return (T)System.Convert.ChangeType(obj, typeof(T));
        }
    }
}