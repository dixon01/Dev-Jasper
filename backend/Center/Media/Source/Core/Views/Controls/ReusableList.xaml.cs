// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReusableList.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ReusableList.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Controls
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// Interaction logic for ReusableList.xaml
    /// </summary>
    public partial class ReusableList
    {
        /// <summary>
        /// the list of existing entities
        /// </summary>
        public static readonly DependencyProperty EntitiesProperty = DependencyProperty.Register(
            "Entities",
            typeof(IEnumerable),
            typeof(ReusableList),
            new PropertyMetadata(default(IEnumerable)));

        /// <summary>
        /// the selected entity
        /// </summary>
        public static readonly DependencyProperty SelectedEntityProperty = DependencyProperty.Register(
            "SelectedEntity",
            typeof(IReusableEntity),
            typeof(ReusableList),
            new PropertyMetadata(default(IReusableEntity)));

        /// <summary>
        /// the command property to choose an entity
        /// </summary>
        public static readonly DependencyProperty ChooseEntityProperty = DependencyProperty.Register(
            "ChooseEntity",
            typeof(ICommand),
            typeof(ReusableList),
            new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// the command to delete an entity
        /// </summary>
        public static readonly DependencyProperty DeleteEntityProperty = DependencyProperty.Register(
            "DeleteEntity",
            typeof(ICommand),
            typeof(ReusableList),
            new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// the command to clone an entity
        /// </summary>
        public static readonly DependencyProperty CloneEntityProperty = DependencyProperty.Register(
            "CloneEntity",
            typeof(ICommand),
            typeof(ReusableList),
            new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// The create new entity command property
        /// </summary>
        public static readonly DependencyProperty CreateNewEntityProperty =
            DependencyProperty.Register(
                "CreateNewEntity",
                typeof(ICommand),
                typeof(ReusableList),
                new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// The create new entity command property
        /// </summary>
        public static readonly DependencyProperty RenameEntityProperty =
            DependencyProperty.Register(
                "RenameEntity",
                typeof(ICommand),
                typeof(ReusableList),
                new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// The new button text property
        /// </summary>
        public static readonly DependencyProperty NewButtonTextProperty = DependencyProperty.Register(
            "NewButtonText",
            typeof(string),
            typeof(ReusableList),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The rename button text property
        /// </summary>
        public static readonly DependencyProperty RenameButtonTextProperty = DependencyProperty.Register(
            "RenameButtonText",
            typeof(string),
            typeof(ReusableList),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The remove button text property
        /// </summary>
        public static readonly DependencyProperty RemoveButtonTextProperty = DependencyProperty.Register(
            "RemoveButtonText",
            typeof(string),
            typeof(ReusableList),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The clone button text property
        /// </summary>
        public static readonly DependencyProperty CloneButtonTextProperty = DependencyProperty.Register(
            "CloneButtonText",
            typeof(string),
            typeof(ReusableList),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The name double click command property.
        /// </summary>
        public static readonly DependencyProperty NameDoubleClickCommandProperty =
            DependencyProperty.Register(
                "NameDoubleClickCommand",
                typeof(ICommand),
                typeof(ReusableList),
                new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// The is in edit mode property.
        /// </summary>
        public static readonly DependencyProperty IsInEditModeProperty = DependencyProperty.Register(
            "IsInEditMode",
            typeof(bool),
            typeof(ReusableList),
            new PropertyMetadata(default(bool)));

        /// <summary>
        /// Property indicating whether the TextBlock with the IsUsed flag is ever shown.
        /// By default it is set to <c>true</c>.
        /// </summary>
        public static readonly DependencyProperty ShowIsUsedProperty = DependencyProperty.Register(
            "ShowInUse", typeof(bool), typeof(ReusableList), new PropertyMetadata(true));

        /// <summary>
        /// Property indicating whether the reference flag is ever shown.
        /// By default it is set to <c>false</c>.
        /// </summary>
        public static readonly DependencyProperty ShowReferenceFlagProperty = DependencyProperty.Register(
            "ShowReferenceFlagFlag", typeof(bool), typeof(ReusableList), new PropertyMetadata(false));

        /// <summary>
        /// Property indicating whether the TextBlock with the IsUsed flag is ever shown.
        /// By default it is set to <c>true</c>.
        /// </summary>
        public static readonly DependencyProperty ShowRadioSelectorProperty = DependencyProperty.Register(
            "ShowRadioSelector", typeof(bool), typeof(ReusableList), new PropertyMetadata(true));

        /// <summary>
        /// the highlighted entity
        /// </summary>
        public static readonly DependencyProperty HighlightedEntityProperty =
            DependencyProperty.Register(
                "HighlightedEntity",
                typeof(IReusableEntity),
                typeof(ReusableList),
                new PropertyMetadata(default(IReusableEntity), OnHighlightedEntityChanged));

        /// <summary>
        /// the name of the not edited name
        /// </summary>
        public static readonly DependencyProperty OldNameProperty = DependencyProperty.Register(
            "OldName",
            typeof(string),
            typeof(ReusableList),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The show create button property
        /// </summary>
        public static readonly DependencyProperty ShowCreateButtonProperty =
            DependencyProperty.Register(
                "ShowCreateButton",
                typeof(bool),
                typeof(ReusableList),
                new PropertyMetadata(default(bool)));

        /// <summary>
        /// The show delete button property.
        /// </summary>
        public static readonly DependencyProperty ShowRemoveButtonProperty =
            DependencyProperty.Register(
                "ShowRemoveButton", typeof(bool), typeof(ReusableList), new PropertyMetadata(true));

        /// <summary>
        /// the prefix content property
        /// </summary>
        public static readonly DependencyProperty PrefixContentProperty = DependencyProperty.Register(
            "PrefixContent",
            typeof(object),
            typeof(ReusableList),
            new PropertyMetadata(default(object)));

        /// <summary>
        /// The reference flag icon property.
        /// </summary>
        public static readonly DependencyProperty ReferenceFlagIconProperty = DependencyProperty.Register(
            "ReferenceFlagIcon",
            typeof(ImageSource),
            typeof(ReusableList),
            new PropertyMetadata(default(ImageSource)));

        /// <summary>
        /// The use one click selection property.
        /// </summary>
        public static readonly DependencyProperty UseOneClickSelectionProperty =
            DependencyProperty.Register(
                "UseOneClickSelection",
                typeof(bool),
                typeof(ReusableList),
                new PropertyMetadata(default(bool)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ReusableList"/> class.
        /// </summary>
        public ReusableList()
        {
            InitializeComponent();
            this.Loaded += this.OnLoaded;
            this.ShowCreateButton = true;
        }

        /// <summary>
        /// The HighlightChanged event.
        /// </summary>
        public event SelectionChangedEventHandler HighlightChanged;

        /// <summary>
        /// The MouseDoubleClicked event for the radio button.
        /// </summary>
        public event EventHandler RadioButtonMouseDoubleClicked;

        /// <summary>
        /// The selected changed event.
        /// </summary>
        public event EventHandler<SelectReusableEntityEventArgs> SelectedChanged;

        /// <summary>
        /// Gets or sets the not edited name.
        /// </summary>
        public string OldName
        {
            get
            {
                return (string)this.GetValue(OldNameProperty);
            }

            set
            {
                this.SetValue(OldNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether use one click selection.
        /// </summary>
        public bool UseOneClickSelection
        {
            get
            {
                return (bool)GetValue(UseOneClickSelectionProperty);
            }

            set
            {
                SetValue(UseOneClickSelectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TextBlock with the IsUsed flag is ever shown.
        /// </summary>
        public bool ShowIsUsed
        {
            get
            {
                return (bool)GetValue(ShowIsUsedProperty);
            }

            set
            {
                SetValue(ShowIsUsedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the TextBlock with the IsUsed flag is ever shown.
        /// </summary>
        public bool ShowReferenceFlag
        {
            get
            {
                return (bool)GetValue(ShowReferenceFlagProperty);
            }

            set
            {
                SetValue(ShowReferenceFlagProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the rows have RadioButtons
        /// </summary>
        public bool ShowRadioSelector
        {
            get
            {
                return (bool)GetValue(ShowRadioSelectorProperty);
            }

            set
            {
                SetValue(ShowRadioSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets double click command for the "Name" TextBlock.
        /// </summary>
        public ICommand NameDoubleClickCommand
        {
            get
            {
                return (ICommand)this.GetValue(NameDoubleClickCommandProperty);
            }

            set
            {
                this.SetValue(NameDoubleClickCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is in edit mode.
        /// </summary>
        public bool IsInEditMode
        {
            get
            {
                return (bool)this.GetValue(IsInEditModeProperty);
            }

            set
            {
                this.SetValue(IsInEditModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected entity
        /// </summary>
        public IReusableEntity SelectedEntity
        {
            get
            {
                return (IReusableEntity)this.GetValue(SelectedEntityProperty);
            }

            set
            {
                this.SetValue(SelectedEntityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the list of entities
        /// </summary>
        public IEnumerable Entities
        {
            get
            {
                return (IEnumerable)this.GetValue(EntitiesProperty);
            }

            set
            {
                this.SetValue(EntitiesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command to select an Entity
        /// </summary>
        public ICommand ChooseEntity
        {
            get
            {
                return (ICommand)this.GetValue(ChooseEntityProperty);
            }

            set
            {
                this.SetValue(ChooseEntityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command to delete an entity
        /// </summary>
        public ICommand DeleteEntity
        {
            get
            {
                return (ICommand)this.GetValue(DeleteEntityProperty);
            }

            set
            {
                this.SetValue(DeleteEntityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command to clone an entity
        /// </summary>
        public ICommand CloneEntity
        {
            get
            {
                return (ICommand)this.GetValue(CloneEntityProperty);
            }

            set
            {
                this.SetValue(CloneEntityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the create new entity command
        /// </summary>
        public ICommand CreateNewEntity
        {
            get
            {
                return (ICommand)this.GetValue(CreateNewEntityProperty);
            }

            set
            {
                this.SetValue(CreateNewEntityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rename entity command
        /// </summary>
        public ICommand RenameEntity
        {
            get
            {
                return (ICommand)this.GetValue(RenameEntityProperty);
            }

            set
            {
                this.SetValue(RenameEntityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the new button text
        /// </summary>
        public string NewButtonText
        {
            get
            {
                return (string)this.GetValue(NewButtonTextProperty);
            }

            set
            {
                this.SetValue(NewButtonTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the rename button text
        /// </summary>
        public string RenameButtonText
        {
            get
            {
                return (string)this.GetValue(RenameButtonTextProperty);
            }

            set
            {
                this.SetValue(RenameButtonTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the remove button text
        /// </summary>
        public string RemoveButtonText
        {
            get
            {
                return (string)this.GetValue(RemoveButtonTextProperty);
            }

            set
            {
                this.SetValue(RemoveButtonTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the clone button text
        /// </summary>
        public string CloneButtonText
        {
            get
            {
                return (string)this.GetValue(CloneButtonTextProperty);
            }

            set
            {
                this.SetValue(CloneButtonTextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the highlighted entity
        /// </summary>
        public IReusableEntity HighlightedEntity
        {
            get
            {
                return (IReusableEntity)this.GetValue(HighlightedEntityProperty);
            }

            set
            {
                this.SetValue(HighlightedEntityProperty, value);
            }
        }

        /// <summary>
        /// Gets the name double click wrapper
        /// </summary>
        public ICommand NameDoubleClickWrapper
        {
            get
            {
                return new RelayCommand(this.OnNameDoubleClick, this.CanExecuteNameDoubleClick);
            }
        }

        /// <summary>
        /// Gets the create new entity wrapper.
        /// </summary>
        public ICommand CreateNewEntityWrapper
        {
            get
            {
                return new RelayCommand(this.OnCreateNewEntity, this.CanExecuteCreateNewEntity);
            }
        }

        /// <summary>
        /// Gets the edit entity name wrapper.
        /// </summary>
        public ICommand EditEntityNameWrapper
        {
            get
            {
                return new RelayCommand(this.OnNameDoubleClick, this.CanExecuteNameDoubleClick);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the create button is shown
        /// </summary>
        public bool ShowCreateButton
        {
            get
            {
                return (bool)this.GetValue(ShowCreateButtonProperty);
            }

            set
            {
                this.SetValue(ShowCreateButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the delete button is shown.
        /// </summary>
        public bool ShowRemoveButton
        {
            get
            {
                return (bool)GetValue(ShowRemoveButtonProperty);
            }

            set
            {
                SetValue(ShowRemoveButtonProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the prefix content
        /// </summary>
        public object PrefixContent
        {
            get
            {
                return this.GetValue(PrefixContentProperty);
            }

            set
            {
                this.SetValue(PrefixContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the indicator flag icon.
        /// </summary>
        public ImageSource ReferenceFlagIcon
        {
            get
            {
                return (ImageSource)this.GetValue(ReferenceFlagIconProperty);
            }

            set
            {
                this.SetValue(ReferenceFlagIconProperty, value);
            }
        }

        /// <summary>
        /// Gets the choose entity wrapper.
        /// </summary>
        public ICommand ChooseEntityWrapper
        {
            get
            {
                return new RelayCommand(
                    entity =>
                        {
                            if (this.UseOneClickSelection)
                            {
                                this.SelectedEntity = entity as IReusableEntity;
                                this.ReusableListBox.SelectedItem = this.SelectedEntity;
                            }
                            else
                            {
                                this.ChooseEntity.Execute(entity);
                            }

                            this.RaiseSelectedChanged(entity);
                        });
            }
        }

        /// <summary>
        /// the handler for radio button double clicked.
        /// </summary>
        /// <param name="e">the parameter</param>
        protected virtual void OnRadioButtonDoubleClick(EventArgs e)
        {
            var handler = this.RadioButtonMouseDoubleClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private static void OnHighlightedEntityChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            var list = (ReusableList)dependencyObject;

            if (e.NewValue != null && list.Entities != null
                && list.Entities.Cast<IReusableEntity>().Any(i => i == e.NewValue))
            {
                list.ReusableListBox.SelectedItem = e.NewValue;
            }
            else
            {
                list.ReusableListBox.SelectedItem = null;
            }
        }

        private void RaiseSelectedChanged(object entity)
        {
            if (this.SelectedChanged != null)
            {
                this.SelectedChanged(this, new SelectReusableEntityEventArgs { Entity = entity });
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null)
            {
                return;
            }

            Mouse.AddPreviewMouseUpHandler(window, this.OnWindowMouseUp);
            Mouse.AddPreviewMouseDownHandler(window, this.OnWindowMouseDown);
        }

        private void OnWindowMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsInEditMode)
            {
                // e.Handled = true;
                // TODO: remove all is in edit mode flags from datasource
            }
        }

        private void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsInEditMode)
            {
                // e.Handled = true;
                // TODO: remove all is in edit mode flags from datasource
            }
        }

        private void RadioButtonOnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.OnRadioButtonDoubleClick(e);
        }

        private void OnEditNameKeyUp(object sender, KeyEventArgs e)
        {
            var control = (Control)sender;
            var item = (IReusableEntity)control.DataContext;
            var prompt = this.DataContext as PromptNotification;

            if (prompt != null)
            {
                prompt.SuppressMouseEvents = !item.IsValid();
            }

            if (e.Key == Key.Escape)
            {
                this.StopEditing(item, true);
                if (prompt != null)
                {
                    prompt.SuppressMouseEvents = !item.IsValid();
                }

                e.Handled = true;
                return;
            }

            if (e.Key == Key.Return)
            {
                if (item.IsValid())
                {
                    this.StopEditing(item);
                }

                e.Handled = true;
            }
        }

        private void OnEditNameLostFocus(object sender, RoutedEventArgs e)
        {
            var control = (Control)sender;
            var item = control.DataContext as IReusableEntity;
            if (item != null && item.IsValid())
            {
                this.StopEditing(item);
            }
        }

        private bool CanExecuteNameDoubleClick(object parameter)
        {
            var result = false;
            var parameters = new RenameReusableEntityParameters { Entity = parameter as IReusableEntity };
            if (this.NameDoubleClickCommand == null)
            {
                if (this.RenameEntity != null && this.RenameEntity.CanExecute(parameters))
                {
                    result = true;
                }
            }
            else
            {
                if (this.NameDoubleClickCommand.CanExecute(parameter))
                {
                    result = true;
                }
            }

            return (parameter as IReusableEntity != null) && result;
        }

        private void OnNameDoubleClick(object parameter)
        {
            var entity = parameter as IReusableEntity;
            if (entity != null)
            {
                entity.IsInEditMode = true;
                this.OldName = entity.GetName();
            }

            this.IsInEditMode = true;

            if (this.NameDoubleClickCommand != null)
            {
                this.NameDoubleClickCommand.Execute(parameter);
            }
        }

        private bool CanExecuteCreateNewEntity(object o)
        {
            return this.CreateNewEntity == null || this.CreateNewEntity.CanExecute(o);
        }

        private void OnCreateNewEntity(object parameter)
        {
            this.CreateNewEntity.Execute(parameter);

            this.IsInEditMode = true;

            foreach (IReusableEntity entity in this.Entities)
            {
                if (entity.IsInEditMode)
                {
                    this.OldName = entity.GetName();
                    break;
                }
            }
        }

        private void StopEditing(IReusableEntity item, bool cancel = false)
        {
            if (this.IsInEditMode || item.IsInEditMode)
            {
                item.IsInEditMode = false;
                this.IsInEditMode = false;
                var newName = item.GetName();
                item.SetName(this.OldName);
                if (!cancel)
                {
                    var parameter = new RenameReusableEntityParameters
                                        {
                                            Entity = item,
                                            NewName = newName ?? string.Empty,
                                        };

                    if (this.RenameEntity != null)
                    {
                        this.RenameEntity.Execute(parameter);
                    }
                }
            }
        }

        private void OnListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEntity = this.ReusableListBox.SelectedItem as IReusableEntity;
            this.HighlightedEntity = selectedEntity;
            if (this.UseOneClickSelection && selectedEntity != null)
            {
                this.ChooseEntity.Execute(selectedEntity);
            }

            if (this.HighlightChanged != null)
            {
                this.HighlightChanged(this, e);
            }
        }

        private void OnScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - (e.Delta / 10.0));
            e.Handled = true;
        }

        private void OnEditNamePreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var control = (Control)sender;
            var item = control.DataContext as IReusableEntity;
            if (item != null && !item.IsValid())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// The select reusable entity event args.
        /// </summary>
        public class SelectReusableEntityEventArgs
        {
            /// <summary>
            /// Gets or sets the entity.
            /// </summary>
            public object Entity { get; set; }
        }
    }
}
