// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ReflectionWrapper.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels.Eval
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// the reflection wrapper
    /// </summary>
    public class ReflectionWrapper : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly object source;

        private readonly PropertyInfo propertyInfo;

        private string error;

        private string evalPart;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionWrapper"/> class.
        /// </summary>
        /// <param name="source">the source</param>
        /// <param name="propertyInfo">the property info</param>
        public ReflectionWrapper(object source, PropertyInfo propertyInfo)
        {
            this.source = source;

            if (this.source != null)
            {
                this.SourceType = source.GetType();
            }

            this.propertyInfo = propertyInfo;
        }

        /// <summary>
        /// The property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the source type.
        /// </summary>
        public Type SourceType { get; private set; }

        string IDataErrorInfo.Error
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ReflectionWrapper.Value"/>
        /// is an <see cref="EvalDataViewModelBase"/> object.
        /// </summary>
        public bool IsEvalDataViewModelBase
        {
            get
            {
                return this.propertyInfo.PropertyType.Name == "EvalDataViewModelBase";
            }
        }

        /// <summary>
        /// Gets or sets the wrapped value
        /// </summary>
        public object Value
        {
            get
            {
                var result = this.propertyInfo.GetValue(this.source, new object[0]);

                var evalDataViewModelBase = result as EvalDataViewModelBase;
                if (evalDataViewModelBase != null)
                {
                   if (this.evalPart == null)
                    {
                        return evalDataViewModelBase.HumanReadable();
                    }

                    return this.evalPart;
                }

                if (result is IDataValue)
                {
                    result = ((IDataValue)result).ValueObject;
                    if (result is DateTime)
                    {
                        return result;
                    }

                    if (result is TimeSpan)
                    {
                        return result;
                    }
                }

                if (result == null && !string.IsNullOrEmpty(this.evalPart))
                {
                    return this.evalPart;
                }

                return result;
            }

            set
            {
                try
                {
                    var currentValue = this.propertyInfo.GetValue(this.source, new object[0]);
                    if (this.propertyInfo.PropertyType.Name == "EvalDataViewModelBase" && value is string)
                    {
                        if (this.TrySetEvaluation((string)value))
                        {
                            this.evalPart = null;
                        }

                        this.OnPropertyChanged("Value");
                        return;
                    }

                    var dataValue = currentValue as IDataValue;
                    this.SetDataValue(dataValue, value, currentValue);

                    this.error = string.Empty;
                }
                catch (Exception e)
                {
                    this.error = e.Message;
                }
            }
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        internal string PropertyName
        {
            get
            {
                return this.propertyInfo.Name;
            }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                return !string.IsNullOrEmpty(this.error) ? this.error : null;
            }
        }

        private void SetDataValue(IDataValue dataValue, object value, object currentValue)
        {
            if (dataValue != null)
            {
                if (dataValue.ValueObject != value)
                {
                    dataValue.ValueObject = value;
                    this.OnPropertyChanged("Value");
                }
            }
            else
            {
                if (currentValue != value)
                {
                    this.propertyInfo.SetValue(this.source, value, new object[0]);
                    this.OnPropertyChanged("Value");
                }
            }
        }

        private bool TrySetEvaluation(string value)
        {
            var appController = ServiceLocator.Current.GetInstance<IMediaApplicationController>();
            var formulaController = appController.ShellController.FormulaController;
            var formulaValue = value;
            if (!value.StartsWith("="))
            {
                formulaValue = value.Insert(0, "=");
            }

            EvalDataViewModelBase result;

            try
            {
                result = formulaController.ParseFormula(formulaValue);
                var evalconfig = result as EvaluationConfigDataViewModel;
                if (evalconfig != null)
                {
                    result = new EvaluationEvalDataViewModel(appController.ShellController.Shell)
                                         {
                                             Reference = evalconfig
                                         };
                }

                this.error = string.Empty;
                this.evalPart = null;
            }
            catch (Exception e)
            {
                this.error = e.Message;
                this.evalPart = value;
                return false;
            }

            // decrement references count for previous predefined formulas
            var previousValue = this.propertyInfo.GetValue(this.source, new object[0]) as EvalDataViewModelBase;
            if (previousValue != null)
            {
                var predefinedFormulas = previousValue.GetContainedPredefinedFormulas();
                foreach (var predefinedFormula in predefinedFormulas)
                {
                    predefinedFormula.ReferencesCount--;
                }
            }

            this.propertyInfo.SetValue(this.source, result, new object[0]);

            // increment references count for new predefined formulas
            if (result != null)
            {
                var predefinedFormulas = result.GetContainedPredefinedFormulas();
                foreach (var predefinedFormula in predefinedFormulas)
                {
                    predefinedFormula.ReferencesCount++;
                }
            }

            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}