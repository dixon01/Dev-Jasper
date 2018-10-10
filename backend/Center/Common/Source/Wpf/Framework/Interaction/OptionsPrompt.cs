// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionsPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The options prompt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Interaction
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Model;
    using Gorba.Center.Common.Wpf.Framework.Model.Options;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Common.Wpf.Framework.ViewModels.Options;

    /// <summary>
    /// The options prompt.
    /// </summary>
    public class OptionsPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        private readonly ObservableCollection<OptionCategoryViewModelBase> categories;
        private OptionCategoryViewModelBase selectedCategory;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsPrompt"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        /// <param name="applicationOptions">
        /// The application Options.
        /// </param>
        public OptionsPrompt(ICommandRegistry commandRegistry, ApplicationOptions applicationOptions)
        {
            this.commandRegistry = commandRegistry;
            this.categories = new ObservableCollection<OptionCategoryViewModelBase>();
            foreach (var category in applicationOptions.Categories)
            {
                this.Categories.Add(this.CreateCategoryViewModel(category));
            }
        }

        /// <summary>
        /// Gets the option categories.
        /// </summary>
        public ObservableCollection<OptionCategoryViewModelBase> Categories
        {
            get
            {
                return this.categories;
            }
        }

        /// <summary>
        /// Gets or sets the selected category.
        /// </summary>
        public OptionCategoryViewModelBase SelectedCategory
        {
            get
            {
                return this.selectedCategory;
            }

            set
            {
                this.SetProperty(ref this.selectedCategory, value, () => this.SelectedCategory);
            }
        }

        /// <summary>
        /// Gets the save command.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(FrameworkCommandCompositionKeys.SaveOptions);
            }
        }

        private OptionCategoryViewModelBase CreateCategoryViewModel(OptionCategoryBase model)
        {
            if (model == null)
            {
                return null;
            }

            var typeName = model.GetType().Name + "ViewModel";
            var assembly = model.GetType().Assembly;
            var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
            if (type == null)
            {
                throw new Exception(
                    string.Format("Type '{0}' for option category model '{1} not found", typeName, model));
            }

            return (OptionCategoryViewModelBase)Activator.CreateInstance(type, model);
        }
    }
}
