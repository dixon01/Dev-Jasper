// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The document data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Project
{
    using System;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// The document data view model.
    /// </summary>
    public class DocumentDataViewModel : DataViewModelBase
    {
        private TenantReadableModel tenant;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDataViewModel"/> class.
        /// </summary>
        /// <param name="readOnlyDataViewModel">
        /// The read only data view model.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if parameter readOnlyDataViewModel is null.
        /// </exception>
        public DocumentDataViewModel(DocumentReadOnlyDataViewModel readOnlyDataViewModel)
        {
            if (readOnlyDataViewModel == null)
            {
                throw new ArgumentNullException("readOnlyDataViewModel");
            }

            this.Initialize(readOnlyDataViewModel.ReadableModel.ToChangeTrackingModel());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDataViewModel"/> class.
        /// </summary>
        /// <param name="writableModel">
        /// The writable model.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if parameter writableModel is null.
        /// </exception>
        public DocumentDataViewModel(DocumentWritableModel writableModel)
        {
            if (writableModel == null)
            {
                throw new ArgumentNullException("writableModel");
            }

            this.Initialize(writableModel);
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public int Id
        {
            get
            {
                return this.Model.Id;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Model.Name;
            }

            set
            {
                if (object.Equals(this.Model.Name, value))
                {
                    return;
                }

                this.Model.Name = value;
                this.RaisePropertyChanged(() => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description
        {
            get
            {
                return this.Model.Description;
            }

            set
            {
                if (object.Equals(this.Model.Description, value))
                {
                    return;
                }

                this.Model.Description = value;
                this.RaisePropertyChanged(() => this.Description);
            }
        }

        /// <summary>
        /// Gets or sets the tenant.
        /// </summary>
        public TenantReadableModel Tenant
        {
            get
            {
                return this.tenant;
            }

            set
            {
                this.SetProperty(ref this.tenant, value, () => this.Tenant);
            }
        }

        /// <summary>
        /// Gets the writable model.
        /// </summary>
        public DocumentWritableModel Model { get; private set; }

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
            var other = obj as DocumentDataViewModel;
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

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Model.Dispose();
            }
        }

        private void Initialize(DocumentWritableModel writableModel)
        {
            this.Model = writableModel;

            if (writableModel.Tenant != null)
            {
                this.Tenant = writableModel.Tenant;
            }
        }
    }
}
