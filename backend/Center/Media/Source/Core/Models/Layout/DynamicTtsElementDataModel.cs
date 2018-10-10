// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicTtsElementDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the properties of a dynamic tts layout element.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml.Serialization;

    /// <summary>
    /// Defines the properties of a dynamic tts layout element.
    /// </summary>   
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here."), XmlRoot("DynamicTts")]
    [Serializable] 
    public class DynamicTtsElementDataModel : TextToSpeechElementDataModel
    {
        /// <summary>
        /// Gets or sets the test data displayed in preview.
        /// </summary>
        [XmlAttribute("TestData")]
        public string TestData { get; set; }
    }
}
