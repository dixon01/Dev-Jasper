// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The LanguageDataViewModel.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Dictionary
{
    using System.Runtime.Serialization;

    using Gorba.Center.Media.Core.Models.Layout.Dictionary;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// The LanguageDataViewModel.
    /// </summary>
    [DataContract(Name = "Language")]
    public class LanguageDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageDataViewModel" /> class.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        public LanguageDataViewModel(Language language)
        {
            this.Index = language.Index;
            this.Name = language.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageDataViewModel"/> class.
        /// </summary>
        /// <param name="language">
        /// The language.
        /// </param>
        public LanguageDataViewModel(LanguageDataModel language)
        {
            this.Index = language.Index;
            this.Name = language.Name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageDataViewModel" /> class.
        /// </summary>
        public LanguageDataViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the language index.
        /// </summary>
        [DataMember(Name = "Index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the name of the language
        /// </summary>
        [DataMember(Name = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// Converts the view model to its data model representation.
        /// </summary>
        /// <returns>
        /// The data model representation of the view model.
        /// </returns>
        public LanguageDataModel ToDataModel()
        {
            var model = new LanguageDataModel();
            this.ConvertToDataModel(model);
            return model;
        }

        /// <summary>
        /// Overrides the base conversion to DataModel adding specific conversion.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        protected void ConvertToDataModel(object dataModel)
        {
            var model = (LanguageDataModel)dataModel;
            if (model == null)
            {
                return;
            }

            model.Index = this.Index;
            model.Name = this.Name;
        }
    }
}