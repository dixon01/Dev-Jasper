// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnimationDataViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The animation data view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.DataViewModels
{
    using System;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Common.Configuration.Infomedia.Common;

    /// <summary>
    /// The animation data view model.
    /// </summary>
    public class AnimationDataViewModel : AnimationDataViewModelBase
    {
        private readonly IMediaShell mediaShell;

        private DataValue<TimeSpan> duration;

        private DataValue<PropertyChangeAnimationType> type;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        public AnimationDataViewModel(IMediaShell mediaShell, AnimationDataViewModel dataViewModel)
        {
            this.mediaShell = mediaShell;
            this.Duration = (DataValue<TimeSpan>)dataViewModel.Duration.Clone();
            this.Duration.PropertyChanged += this.DurationChanged;
            this.Type = (DataValue<PropertyChangeAnimationType>)dataViewModel.Type.Clone();
            this.Type.PropertyChanged += this.TypeChanged;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationDataViewModel"/> class.
        /// </summary>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        public AnimationDataViewModel(IMediaShell mediaShell, PropertyChangeAnimationDataModel dataModel = null)
        {
            this.mediaShell = mediaShell;
            this.Duration = new DataValue<TimeSpan>(new TimeSpan());
            this.Duration.PropertyChanged += this.DurationChanged;
            this.Type = new DataValue<PropertyChangeAnimationType>();
            this.Type.PropertyChanged += this.TypeChanged;
            if (dataModel != null)
            {
                this.Duration.Value = dataModel.Duration;
                this.Type.Value = dataModel.Type;
            }
        }

        /// <summary>
        /// Gets or sets the duration of the animation.
        /// </summary>
        public DataValue<TimeSpan> Duration
        {
            get
            {
                return this.duration;
            }

            set
            {
                this.SetProperty(ref this.duration, value, () => this.Duration);
                this.MakeDirty();
            }
        }

        /// <summary>
        /// Gets or sets the type of the animation.
        /// </summary>
        public DataValue<PropertyChangeAnimationType> Type
        {
            get
            {
                return this.type;
            }

            set
            {
                this.SetProperty(ref this.type, value, () => this.Type);
                this.MakeDirty();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value or the Formula is dirty.
        /// </summary>
        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || (this.Duration != null && this.Duration.IsDirty)
                       || (this.Type != null && this.Type.IsDirty);
            }
        }

        /// <summary>
        /// Exports the data view model.
        /// </summary>
        /// <param name="exportParameters">
        /// The export Parameters.
        /// These are used to do automatic conversions if project is not compatible with the selected update group.
        /// </param>
        /// <returns>
        /// The exported entity.
        /// </returns>
        public PropertyChangeAnimation Export(object exportParameters = null)
        {
            var anim = new PropertyChangeAnimation();
            this.DoExport(anim, exportParameters);
            return anim;
        }

        /// <summary>
        /// Converts a data view model to a data model.
        /// </summary>
        /// <returns>
        /// The converted <see cref="AnimatedDynamicPropertyDataModel"/>.
        /// </returns>
        public AnimatedDynamicPropertyDataModel ToDataModel()
        {
            var model = new AnimatedDynamicPropertyDataModel();
            this.ConvertToDataModel(model);
            return model;
        }

        /// <summary>
        /// Creates a deep clone of the current object.
        /// </summary>
        /// <returns>
        /// The cloned object.
        /// </returns>
        public override object Clone()
        {
            return new AnimationDataViewModel(this.mediaShell, this);
        }

        /// <summary>
        /// Clears the dirty flag of all properties.
        /// </summary>
        public override void ClearDirty()
        {
            this.Duration.ClearDirty();
            this.Type.ClearDirty();
            base.ClearDirty();
        }

        /// <summary>
        /// compares two view models
        /// </summary>
        /// <param name="obj">the other view model</param>
        /// <returns>true if equal</returns>
        public override bool EqualsViewModel(object obj)
        {
            var result = base.EqualsViewModel(obj);
            var that = obj as AnimationDataViewModel;

            if (that == null)
            {
                result = false;
            }
            else if (this.Type.EqualsValue(that.Type))
            {
                result = false;
            }
            else if (this.Duration.EqualsValue(that.Duration))
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Converts this instance into a <see cref="AnimatedDynamicPropertyDataModel"/>.
        /// </summary>
        /// <param name="dataModel">
        /// The data model.
        /// </param>
        protected override void ConvertToDataModel(object dataModel)
        {
            base.ConvertToDataModel(dataModel);

            var model = dataModel as AnimatedDynamicPropertyDataModel;
            if (model == null)
            {
                return;
            }

            model.Animation = new PropertyChangeAnimationDataModel
                                  {
                                      Duration = this.Duration.Value,
                                      Type = this.Type.Value
                                  };
        }

        /// <summary>
        /// Exports this instance to a <see cref="PropertyChangeAnimation"/> object.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="exportParameters">
        /// The export Parameters.
        /// These are used to do automatic conversions if project is not compatible with the selected update group.
        /// </param>
        protected override void DoExport(object model, object exportParameters)
        {
            var exportModel = (PropertyChangeAnimation)model;
            exportModel.Duration = this.Duration.Value;
            exportModel.Type = this.Type.Value;
        }

        private void TypeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Type);
        }

        private void DurationChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                return;
            }

            this.RaisePropertyChanged(() => this.Duration);
        }
    }
}
