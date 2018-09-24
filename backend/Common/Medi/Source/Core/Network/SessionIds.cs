// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionIds.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SessionIds type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Network
{
    /// <summary>
    /// Static helper class for <see cref="ISessionId"/>.
    /// </summary>
    public static class SessionIds
    {
        /// <summary>
        /// Session id that represents the entries.
        /// </summary>
        public static readonly ISessionId Local = new LocalSessionId();

        private class LocalSessionId : ISessionId
        {
            public override bool Equals(object obj)
            {
                return this.Equals(obj as LocalSessionId);
            }

            public bool Equals(ISessionId obj)
            {
                return obj != null && this.GetType() == obj.GetType();
            }

            public override int GetHashCode()
            {
                return 42;
            }

            public override string ToString()
            {
                return "<Local>";
            }
        }
    }
}