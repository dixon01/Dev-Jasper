// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelElementEntityDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelElementEntityDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.T4Templating
{
    /// <summary>
    /// Describes a data view model to be generated.
    /// </summary>
    public class DataViewModelElementEntityDescriptor : DataViewModelEntityDescriptor
    {
        /// <summary>
        /// Gets the name of the view model.
        /// </summary>
        /// <returns>The name of the view model.</returns>
        protected override string GetViewModelName()
        {
            if (this.ParentObject.Name == "Layout" && this.Name == "Base")
            {
                return "LayoutElementDataViewModelBase";
            }

            return base.GetViewModelName();
        }
    }
}