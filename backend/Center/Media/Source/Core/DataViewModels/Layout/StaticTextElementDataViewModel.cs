// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticTextElementDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Layout
{
    using Gorba.Center.Media.Core.Models.Layout;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Defines the properties of a static text layout element.
    /// </summary>
    public class StaticTextElementDataViewModel : TextElementDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTextElementDataViewModel"/> class.
        /// </summary>
        /// <param name="parentViewModel">
        /// The parent view model.
        /// </param>
        public StaticTextElementDataViewModel(IMediaShell parentViewModel)
            : base(parentViewModel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTextElementDataViewModel"/> class.
        /// </summary>
        /// <param name="parentViewModel">
        /// The parent view model.
        /// </param>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public StaticTextElementDataViewModel(IMediaShell parentViewModel, StaticTextElementDataModel dataModel)
            : base(parentViewModel, dataModel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTextElementDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        protected StaticTextElementDataViewModel(IMediaShell mediaShell, StaticTextElementDataViewModel dataViewModel)
            : base(mediaShell, dataViewModel)
        {
        }

        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The deep cloned instance.
        /// </returns>
        public override object Clone()
        {
            var result = new StaticTextElementDataViewModel(this.Shell, this);

            if (this.IsDirty)
            {
                result.MakeDirty();
            }

            result.ClonedFrom = this.GetHashCode();

            return result;
        }

        /// <summary>
        /// The to data model.
        /// </summary>
        /// <returns>
        /// The <see cref="StaticTextElementDataModel"/>.
        /// </returns>
        public new StaticTextElementDataModel ToDataModel()
        {
            var model = (StaticTextElementDataModel)this.CreateDataModelObject();
            this.ConvertToDataModel(model);
            return model;
        }

        /// <summary>
        /// Creates an empty <see cref="StaticTextElementDataModel"/>.
        /// </summary>
        /// <returns>
        /// The the empty data model.
        /// </returns>
        protected override object CreateDataModelObject()
        {
            return new StaticTextElementDataModel();
        }
    }
}
