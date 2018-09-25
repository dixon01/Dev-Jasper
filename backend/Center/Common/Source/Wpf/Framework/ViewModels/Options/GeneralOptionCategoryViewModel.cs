// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralOptionCategoryViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.Options
{
    using Gorba.Center.Common.Wpf.Framework.Model.Options;

    /// <summary>
    /// The general option category view model.
    /// </summary>
    public class GeneralOptionCategoryViewModel : OptionCategoryViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralOptionCategoryViewModel"/> class.
        /// </summary>
        public GeneralOptionCategoryViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralOptionCategoryViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public GeneralOptionCategoryViewModel(OptionCategoryBase model)
            : base(model)
        {
        }

        /// <summary>
        /// Creates a model from this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="GeneralOptionCategory"/> model.
        /// </returns>
        public override OptionCategoryBase CreateModel()
        {
            var category = new GeneralOptionCategory();
            foreach (var group in this.Groups)
            {
                category.Groups.Add(group.CreateModel());
            }

            return category;
        }
    }
}
