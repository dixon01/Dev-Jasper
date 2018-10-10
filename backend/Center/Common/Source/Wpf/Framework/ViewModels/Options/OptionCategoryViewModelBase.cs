// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionCategoryViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The option category view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.ViewModels.Options
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Model.Options;

    /// <summary>
    /// The option category view model.
    /// </summary>
    public abstract class OptionCategoryViewModelBase : ViewModelBase
    {
        private ObservableCollection<OptionGroupViewModelBase> groups;
        private string title;

        private ImageSource categoryIconSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionCategoryViewModelBase"/> class.
        /// </summary>
        public OptionCategoryViewModelBase()
        {
            this.groups = new ObservableCollection<OptionGroupViewModelBase>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionCategoryViewModelBase"/> class.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        public OptionCategoryViewModelBase(OptionCategoryBase model)
        {
            this.groups = new ObservableCollection<OptionGroupViewModelBase>();
            foreach (var group in model.Groups)
            {
                this.groups.Add(this.CreateGroupViewModel(group));
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.SetProperty(ref this.title, value, () => this.Title);
            }
        }

        /// <summary>
        /// Gets or sets the title tooltip.
        /// </summary>
        public string TitleTooltip { get; set; }

        /// <summary>
        /// Gets or sets the category icon source.
        /// </summary>
        public ImageSource CategoryIconSource
        {
            get
            {
                return this.categoryIconSource;
            }

            set
            {
                this.SetProperty(ref this.categoryIconSource, value, () => this.CategoryIconSource);
            }
        }

        /// <summary>
        /// Gets or sets the group view models.
        /// </summary>
        public ObservableCollection<OptionGroupViewModelBase> Groups
        {
            get
            {
                return this.groups;
            }

            set
            {
                this.SetProperty(ref this.groups, value, () => this.Groups);
            }
        }

        /// <summary>
        /// Creates a model from this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="OptionCategoryBase"/> model.
        /// </returns>
        public abstract OptionCategoryBase CreateModel();

        private OptionGroupViewModelBase CreateGroupViewModel(OptionGroupBase model)
        {
            if (model == null)
            {
                return null;
            }

            var typeName = model.GetType().Name + "ViewModel";
            var assembly = this.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(string.Format("Type '{0}' for option group model '{1} not found", typeName, model));
            }

            return (OptionGroupViewModelBase)Activator.CreateInstance(type, model);
        }
    }
}
