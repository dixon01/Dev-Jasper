// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionGroupBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Base class for all option groups. To be able to serialize the ApplicationState all implementations of this class
//   must be added as known type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Model.Options
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Base class for all option groups. To be able to serialize the ApplicationOptions all implementations of
    /// this class must be added as known type.
    /// </summary>
    [DataContract]
    [KnownType(typeof(LanguageOptionGroup))]
    public abstract class OptionGroupBase
    {
    }
}
