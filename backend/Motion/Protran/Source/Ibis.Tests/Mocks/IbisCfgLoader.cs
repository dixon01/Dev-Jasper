// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisCfgLoader.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Mocks
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Loads the configuration file for the IBIS protocol.
    /// </summary>
    public class IbisCfgLoader
    {
        private readonly IbisConfig ibisConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisCfgLoader"/> class.
        /// </summary>
        /// <param name="is7Bit">
        /// Flag that tells if the config container has the behaviour for 7 Bit.
        /// </param>
        public IbisCfgLoader(bool is7Bit)
        {
            this.ibisConfig = new IbisConfig();

            var chainDefault = new Chain { Id = "Default", Transformations = new List<TransformationConfig>() };

            var chainTime = new Chain { Id = "Time", Transformations = new List<TransformationConfig>() };
            var regexTime = new RegexMapping();
            var mappingTime = new Mapping { From = "^(\\d\\d)(\\d\\d)$", To = "$1:$2" };
            regexTime.Mappings = new List<Mapping> { mappingTime };
            chainTime.Transformations.Add(regexTime);

            var chainConnDest = new Chain { Id = "ConnectionDest", Transformations = new List<TransformationConfig>() };
            var regexConnDest = new RegexMapping();
            var mappingConnDest = new Mapping { From = "[ \\t]+$", To = string.Empty };
            regexConnDest.Mappings = new List<Mapping> { mappingConnDest };
            chainConnDest.Transformations.Add(regexConnDest);

            var chainDate = new Chain { Id = "Time", Transformations = new List<TransformationConfig>() };
            var regexDate = new RegexMapping();
            var mappingDate = new Mapping { From = "^(\\d\\d)(\\d\\d)(\\d\\d)$", To = "$1.$2.20$3" };
            regexDate.Mappings = new List<Mapping> { mappingDate };
            chainDate.Transformations.Add(regexDate);

            var chainStops = new Chain { Id = "Stops", Transformations = new List<TransformationConfig>() };
            var regexStops = new RegexDivider { Regex = "\u000A" };
            chainStops.Transformations.Add(regexStops);

            var chainStops021A = new Chain { Id = "Stops021a", Transformations = new List<TransformationConfig>() };
            var regexStops021A = new RegexDivider { Regex = "[\u0003-\u0006]" };
            chainStops021A.Transformations.Add(regexStops021A);

            var chainLine = new Chain { Id = "Line", Transformations = new List<TransformationConfig>() };
            var regexLine = new RegexMapping { Mappings = new List<Mapping>() };
            var mappingLine = new Mapping { From = "^0+", To = string.Empty };
            regexLine.Mappings.Add(mappingLine);
            chainLine.Transformations.Add(regexLine);

            var chainNumber = new Chain { Id = "Number", Transformations = new List<TransformationConfig>() };
            var integerNumber = new Integer();
            chainNumber.Transformations.Add(integerNumber);

            this.ibisConfig.Transformations.Add(chainDefault);
            this.ibisConfig.Transformations.Add(chainTime);
            this.ibisConfig.Transformations.Add(chainStops);
            this.ibisConfig.Transformations.Add(chainStops021A);
            this.ibisConfig.Transformations.Add(chainLine);
            this.ibisConfig.Transformations.Add(chainNumber);
            this.ibisConfig.Transformations.Add(chainConnDest);

            this.ibisConfig.Behaviour.ByteType = is7Bit ? ByteType.Ascii7 : ByteType.UnicodeBigEndian;
        }

        /// <summary>
        /// Loads the configuration file for the IBIS protocol.
        /// </summary>
        /// <returns>
        /// The container of all the information required for the IBIS protocol.6
        /// </returns>
        public IbisConfig Load()
        {
            return this.ibisConfig;
        }
    }
}
