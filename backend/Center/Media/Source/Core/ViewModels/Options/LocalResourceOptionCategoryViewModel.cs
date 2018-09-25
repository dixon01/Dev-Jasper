// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalResourceOptionCategoryViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.Options
{
    using Gorba.Center.Common.Wpf.Framework.Model.Options;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Options;
    using Gorba.Center.Media.Core.Models.Options;

    /// <summary>
    /// The local resource option category view model.
    /// </summary>
    public class LocalResourceOptionCategoryViewModel : OptionCategoryViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalResourceOptionCategoryViewModel"/> class.
        /// </summary>
        public LocalResourceOptionCategoryViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalResourceOptionCategoryViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public LocalResourceOptionCategoryViewModel(OptionCategoryBase model)
            : base(model)
        {
        }

        /// <summary>
        /// The create model.
        /// </summary>
        /// <returns>
        /// The <see cref="OptionCategoryBase"/>.
        /// </returns>
        public override OptionCategoryBase CreateModel()
        {
            var category = new LocalResourceOptionCategory();
            foreach (var group in this.Groups)
            {
                category.Groups.Add(group.CreateModel());
            }

            return category;
        }
    }
}
