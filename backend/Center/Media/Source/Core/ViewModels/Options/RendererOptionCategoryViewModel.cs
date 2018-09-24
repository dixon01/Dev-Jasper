// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RendererOptionCategoryViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.Options
{
    using Gorba.Center.Common.Wpf.Framework.Model.Options;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Options;
    using Gorba.Center.Media.Core.Models.Options;

    /// <summary>
    /// The renderer option category view model.
    /// </summary>
    public class RendererOptionCategoryViewModel : OptionCategoryViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RendererOptionCategoryViewModel"/> class.
        /// </summary>
        public RendererOptionCategoryViewModel()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RendererOptionCategoryViewModel"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public RendererOptionCategoryViewModel(OptionCategoryBase model)
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
            var category = new RendererOptionCategory();
            foreach (var group in this.Groups)
            {
                category.Groups.Add(group.CreateModel());
            }

            return category;
        }
    }
}
