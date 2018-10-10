// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityValidatorBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityValidatorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities
{
    using System.ComponentModel;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.Services;

    /// <summary>
    /// The base class for entity validation.
    /// </summary>
    public abstract class EntityValidatorBase
    {
        private readonly DataViewModelBase dataViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityValidatorBase"/> class.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <param name="dataController">
        /// The data controller.
        /// </param>
        protected EntityValidatorBase(DataViewModelBase dataViewModel, DataController dataController)
        {
            this.dataViewModel = dataViewModel;
            this.DataController = dataController;
        }

        /// <summary>
        /// Gets the data controller.
        /// </summary>
        public DataController DataController { get; private set; }

        /// <summary>
        /// Starts this validator.
        /// </summary>
        public virtual void Start()
        {
            this.dataViewModel.PropertyChanged += this.DataViewModelOnPropertyChanged;
            this.HandleDataViewModelChange(null);
        }

        /// <summary>
        /// Stops this validator.
        /// </summary>
        public virtual void Stop()
        {
            this.dataViewModel.PropertyChanged -= this.DataViewModelOnPropertyChanged;
        }

        /// <summary>
        /// Subclasses can handle property changes and validation in this method.
        /// </summary>
        /// <param name="propertyName">
        /// The property name. If the property name is null, all properties have to be validated.
        /// </param>
        protected virtual void HandleDataViewModelChange(string propertyName)
        {
        }

        /// <summary>
        /// Validates the given <see cref="XmlDataViewModel"/> object.
        /// This method is usually only called by generated code.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="xmlData">
        /// The xml data.
        /// </param>
        protected virtual void ValidateXml(string propertyName, XmlDataViewModel xmlData)
        {
            this.dataViewModel.ClearErrors(propertyName);
            var exceptions = XmlValidator.Validate(xmlData.Xml, xmlData.Schema);
            foreach (var exception in exceptions)
            {
                this.dataViewModel.AddError(propertyName, exception.Message);
            }
        }

        private void DataViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.HandleDataViewModelChange(e.PropertyName);
        }
    }
}
