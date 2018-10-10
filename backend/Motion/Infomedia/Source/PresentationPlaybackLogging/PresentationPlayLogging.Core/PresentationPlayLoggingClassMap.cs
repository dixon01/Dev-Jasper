// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationLoggingClassMap.cs" company="Luminator LTG">
//   Copyright © 2011-2017 LuminatorLTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PresentationPlayLogging.Core
{
    using CsvHelper.Configuration;

    /// <summary>The presentation logging class map.</summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PresentationPlayLoggingClassMap<T> : ClassMap<T>
        where T : class
    {
    }
}