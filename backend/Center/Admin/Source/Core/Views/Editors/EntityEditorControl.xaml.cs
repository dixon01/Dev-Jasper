// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityEditorControl.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Admin.Core.Views.Editors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.Interaction;
    using Gorba.Center.Admin.Core.ViewModels.Editor;
    using Gorba.Center.Admin.Core.Views.Widgets;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Converters;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Common.Utility.Core;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.Data.PropertyGrid;

    /// <summary>
    /// Interaction logic for EntityEditorControl.xaml
    /// </summary>
    public partial class EntityEditorControl
    {
        private readonly List<Window> editorWindows = new List<Window>();

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEditorControl"/> class.
        /// </summary>
        public EntityEditorControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the selected error property.
        /// </summary>
        public PropertyDefinition SelectedErrorProperty { get; private set; }

        private void PropertyGridOnAutoGeneratingPropertyDefinition(
            object sender, AutoGeneratingPropertyDefinitionEventArgs e)
        {
            var propertyName = e.PropertyDefinition.SourceProperty.Name;
            if (propertyName == "ClonedFrom"
                || propertyName == "DisplayName"
                || propertyName == "DisplayText"
                || propertyName == "Model"
                || propertyName == "ReadOnlyDataViewModel"
                || propertyName == "Factory"
                || propertyName == "IsLoading"
                || propertyName == "HasErrors")
            {
                // internal properties are never shown in the form
                e.Cancel = true;
                return;
            }

            var propertyType = e.PropertyDefinition.SourceProperty.PropertyType;
            if (typeof(ICommand).IsAssignableFrom(propertyType))
            {
                // command properties are never shown in the form
                e.Cancel = true;
                return;
            }

            var binding = e.PropertyDefinition.Binding as Binding;
            if (binding != null)
            {
                // change the trigger to be on property changed, not on lost focus (triggers the validation immediately)
                binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                binding.Converter = new UtcToUiTimeConverter();
            }

            if (e.PropertyDefinition.SourceProperty != null
                && e.PropertyDefinition.SourceProperty.Descriptor is UserDefinedPropertyDescriptor)
            {
                // always show User Defined Properties and show them at the end
                e.PropertyDefinition.OrderIndex = int.MaxValue;
            }
            else
            {
                var viewModel = this.DataContext as EntityEditorViewModel;
                if (viewModel != null && viewModel.EditingEntity != null)
                {
                    var parameters = new PropertyDisplayParameters(viewModel.EditingEntity, propertyName);
                    viewModel.UpdatePropertyDisplayCommand.Execute(parameters);
                    if (!parameters.IsVisible)
                    {
                        e.Cancel = true;
                        return;
                    }

                    e.PropertyDefinition.DisplayName = parameters.DisplayName;
                    e.PropertyDefinition.OrderIndex = parameters.OrderIndex;

                    var errors = viewModel.EditingEntity.GetErrors(parameters.PropertyName);
                    var hasError = errors != null && errors.GetEnumerator().MoveNext();

                    if (hasError
                        && (this.SelectedErrorProperty == null
                            || e.PropertyDefinition.OrderIndex < this.SelectedErrorProperty.OrderIndex))
                    {
                        this.SelectedErrorProperty = e.PropertyDefinition;
                    }
                }
            }

            this.SelectCustomEditor(sender, e, propertyType, propertyName);
        }

        private void SelectCustomEditor(
            object sender,
            AutoGeneratingPropertyDefinitionEventArgs e,
            Type propertyType,
            string propertyName)
        {
            string editorResource = null;
            if (typeof(ItemSelectionViewModelBase).IsAssignableFrom(propertyType))
            {
                editorResource = "ItemSelectionEditor";
            }
            else if (typeof(XmlDataViewModel).IsAssignableFrom(propertyType))
            {
                editorResource = "EntityXmlEditor";
            }
            else if (propertyName.Contains("Password"))
            {
                editorResource = "PasswordEditor";
            }

            if (editorResource == null)
            {
                // use default editor
                return;
            }

            var frameworkElement = (FrameworkElement)sender;
            e.PropertyDefinition.EditorTemplate = (DataTemplate)frameworkElement.FindResource(editorResource);
            e.PropertyDefinition.IsReadOnly = false;
        }

        private void OnXmlEditorPopupOpened(EntityXmlEditor editor, Window window)
        {
            this.editorWindows.Add(window);
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            foreach (var editorWindow in this.editorWindows)
            {
                editorWindow.Close();
            }

            this.editorWindows.Clear();
        }

        private void PasswordChangeOnClick(object sender, RoutedEventArgs e)
        {
            var prompt = new ChangePasswordPrompt(Strings.ChangePassword_Title, string.Empty);
            prompt.ShowCurrentPassword = false;
            InteractionManager<ChangePasswordPrompt>.Current.Raise(prompt, this.OnPasswordChanged);
        }

        private void OnPasswordChanged(ChangePasswordPrompt prompt)
        {
            if (prompt == null || !prompt.Confirmed)
            {
                return;
            }

            var viewModel = this.DataContext as EntityEditorViewModel;
            if (viewModel == null || viewModel.EditingEntity == null)
            {
                return;
            }

            var property =
                viewModel.EditingEntity.GetType().GetProperties().FirstOrDefault(p => p.Name.Contains("Password"));
            if (property != null)
            {
                property.SetValue(viewModel.EditingEntity, SecurityUtility.Md5(prompt.NewPassword));
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.SelectedErrorProperty = null;
        }

        private void RadPropertyGrid_OnSelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            var grid = sender as RadPropertyGrid;
            if (grid == null || grid.SelectedPropertyDefinition == null)
            {
                return;
            }

            grid.BeginEdit(grid.SelectedPropertyDefinition);
        }
    }
}
