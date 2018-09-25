// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentReadOnlyDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The document read only data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Project
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// The document read only data view model.
    /// </summary>
    public class DocumentReadOnlyDataViewModel : DataViewModelBase
    {
        private TenantReadableModel referenceTenant;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentReadOnlyDataViewModel"/> class.
        /// </summary>
        /// <param name="readableModel">
        /// The readable model.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if parameter readableModel is null.
        /// </exception>
        public DocumentReadOnlyDataViewModel(DocumentReadableModel readableModel)
        {
            if (readableModel == null)
            {
                throw new ArgumentNullException("readableModel");
            }

            this.Versions = new ObservableCollection<DocumentVersionReadOnlyDataViewModel>();
            this.ReadableModel = readableModel;
            this.ReadableModel.PropertyChanged += this.ReadableModelOnPropertyChanged;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public int Id
        {
            get
            {
                return this.ReadableModel.Id;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.ReadableModel.Name;
            }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.ReadableModel.Description;
            }
        }

        /// <summary>
        /// Gets the tenant.
        /// </summary>
        public TenantReadableModel Tenant
        {
            get
            {
                if (this.referenceTenant == null && this.ReadableModel.Tenant != null)
                {
                    this.referenceTenant = this.ReadableModel.Tenant;
                }

                return this.referenceTenant;
            }
        }

        /// <summary>
        /// Gets the versions.
        /// </summary>
        public ObservableCollection<DocumentVersionReadOnlyDataViewModel> Versions { get; private set; }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        public override string DisplayText
        {
            get
            {
                return Convert.ToString(this.Name);
            }
        }

        /// <summary>
        /// Gets the readable model.
        /// </summary>
        public DocumentReadableModel ReadableModel { get; private set; }

        /// <summary>
        /// The get id string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetIdString()
        {
            return Convert.ToString(this.Id);
        }

        /// <summary>
        /// Checks if this and the object are equal.
        /// </summary>
        /// <param name="obj">
        /// The object.
        /// </param>
        /// <returns>
        /// <c>true</c> if the objects are equal; <c>false</c> otherwise.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as DocumentReadOnlyDataViewModel;
            return other != null && this.Id.Equals(other.Id);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        private void ReadableModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
            if (e.PropertyName == "Name")
            {
                this.RaisePropertyChanged(() => this.DisplayText);
            }
        }
    }
}
