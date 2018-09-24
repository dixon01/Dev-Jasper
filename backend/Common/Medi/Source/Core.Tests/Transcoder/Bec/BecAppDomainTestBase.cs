// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecAppDomainTestBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecAppDomainTestBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Tests.Transcoder.Bec
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Policy;

    using Gorba.Common.Medi.Core.Tests.Transcoder.Bec.Utils;
    using Gorba.Common.Medi.Core.Transcoder.Bec;

    /// <summary>
    /// Base class for all BEC tests that require <see cref="AppDomain"/>s for testing.
    /// </summary>
    public abstract class BecAppDomainTestBase
    {
        /// <summary>
        /// Creates a <see cref="ChangingPropertiesTester"/> in a new <see cref="AppDomain"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="AppDomain"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IChangingPropertiesTester"/> that wraps the calls to the
        /// newly created <see cref="AppDomain"/>.
        /// </returns>
        protected IChangingPropertiesTester CreateTester(string name)
        {
            var appBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var appDomain = AppDomain.CreateDomain(name, new Evidence(), appBasePath, ".", true);

            return new ChangingPropertiesTesterWrapper(appDomain);
        }

        /// <summary>
        /// Gets all pairs of possible serializer types.
        /// </summary>
        /// <returns>
        /// An enumeration over all pairs of serializer types.
        /// </returns>
        protected IEnumerable<Tuple<BecCodecConfig.SerializerType, BecCodecConfig.SerializerType>> GetSerializerTypes()
        {
            var types = Enum.GetValues(typeof(BecCodecConfig.SerializerType));
            return from BecCodecConfig.SerializerType type1 in types
                   from BecCodecConfig.SerializerType type2 in types
                   select new Tuple<BecCodecConfig.SerializerType, BecCodecConfig.SerializerType>(type1, type2);
        }
    }
}