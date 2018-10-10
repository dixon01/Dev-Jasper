// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentVersionReadOnlyDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The document version read only data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Project
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Documents;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Membership;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;

    /// <summary>
    /// The document version read only data view model.
    /// </summary>
    public class DocumentVersionReadOnlyDataViewModel : DataViewModelBase
    {
        private DocumentReadOnlyDataViewModel referenceDocument;

        private UserReadableModel referenceCreatingUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentVersionReadOnlyDataViewModel"/> class.
        /// </summary>
        /// <param name="readableModel">
        /// The readable model.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if parameter readableModel is null.
        /// </exception>
        public DocumentVersionReadOnlyDataViewModel(DocumentVersionReadableModel readableModel)
        {
            if (readableModel == null)
            {
                throw new ArgumentNullException("readableModel");
            }

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
        /// Gets the major.
        /// </summary>
        public int Major
        {
            get
            {
                return this.ReadableModel.Major;
            }
        }

        /// <summary>
        /// Gets the minor.
        /// </summary>
        public int Minor
        {
            get
            {
                return this.ReadableModel.Minor;
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
        /// Gets the document.
        /// </summary>
        public DocumentReadOnlyDataViewModel Document
        {
            get
            {
                if (this.referenceDocument == null && this.ReadableModel.Document != null)
                {
                    this.referenceDocument = new DocumentReadOnlyDataViewModel(this.ReadableModel.Document);
                }

                return this.referenceDocument;
            }
        }

        /// <summary>
        /// Gets the creating user.
        /// </summary>
        public UserReadableModel CreatingUser
        {
            get
            {
                if (this.referenceCreatingUser == null && this.ReadableModel.CreatingUser != null)
                {
                    this.referenceCreatingUser = this.ReadableModel.CreatingUser;
                }

                return this.referenceCreatingUser;
            }
        }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        public override string DisplayText
        {
            get
            {
                return Convert.ToString(this.Description);
            }
        }

        /// <summary>
        /// Gets the readable model.
        /// </summary>
        public DocumentVersionReadableModel ReadableModel { get; private set; }

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
            var other = obj as DocumentVersionReadOnlyDataViewModel;
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
            if (e.PropertyName == "Description")
            {
                this.RaisePropertyChanged(() => this.DisplayText);
            }
        }
    }
}
