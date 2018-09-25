// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReusableEntitySelector.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ReusableEntitySelector.xaml.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// Interaction logic for ReusableEntitySelector.xaml
    /// </summary>
    public partial class ReusableEntitySelector
    {
        /// <summary>
        /// The context menu for the right hand entities.
        /// </summary>
        public static readonly DependencyProperty EntityContextMenuProperty =
            DependencyProperty.Register(
                "EntityContextMenu",
                typeof(ContextMenu),
                typeof(ReusableEntitySelector),
                new PropertyMetadata(default(ContextMenu)));

        /// <summary>
        /// the list of previously defined entities
        /// </summary>
        public static readonly DependencyProperty PreviouslyDefinedEntitiesProperty =
            DependencyProperty.Register(
                "PreviouslyDefinedEntities",
                typeof(IEnumerable),
                typeof(ReusableEntitySelector),
                new PropertyMetadata(default(IEnumerable)));

        /// <summary>
        /// the list of existing entities
        /// </summary>
        public static readonly DependencyProperty EntitiesProperty = DependencyProperty.Register(
            "Entities",
            typeof(IEnumerable),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(IEnumerable)));

        /// <summary>
        /// the selected entity
        /// </summary>
        public static readonly DependencyProperty SelectedEntityProperty = DependencyProperty.Register(
            "SelectedEntity",
            typeof(IReusableEntity),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(IReusableEntity)));

        /// <summary>
        /// the command property to choose an entity
        /// </summary>
        public static readonly DependencyProperty ChooseEntityProperty = DependencyProperty.Register(
            "ChooseEntity",
            typeof(ICommand),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// the command to delete an entity
        /// </summary>
        public static readonly DependencyProperty DeleteEntityProperty = DependencyProperty.Register(
            "DeleteEntity",
            typeof(ICommand),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// the command to clone an entity
        /// </summary>
        public static readonly DependencyProperty CloneEntityProperty = DependencyProperty.Register(
            "CloneEntity",
            typeof(ICommand),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// The create new entity command property
        /// </summary>
        public static readonly DependencyProperty CreateNewEntityProperty =
            DependencyProperty.Register(
                "CreateNewEntity",
                typeof(ICommand),
                typeof(ReusableEntitySelector),
                new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// The left hand title property
        /// </summary>
        public static readonly DependencyProperty LeftHandTitleProperty = DependencyProperty.Register(
            "LeftHandTitle",
            typeof(string),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The right hand title property
        /// </summary>
        public static readonly DependencyProperty RightHandTitleProperty = DependencyProperty.Register(
            "RightHandTitle",
            typeof(string),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The new button text property
        /// </summary>
        public static readonly DependencyProperty NewButtonTextProperty = DependencyProperty.Register(
            "NewButtonText",
            typeof(string),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The Rename button text property
        /// </summary>
        public static readonly DependencyProperty RenameButtonTextProperty = DependencyProperty.Register(
            "RenameButtonText",
            typeof(string),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The remove button text property
        /// </summary>
        public static readonly DependencyProperty RemoveButtonTextProperty = DependencyProperty.Register(
            "RemoveButtonText",
            typeof(string),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// The clone button text property
        /// </summary>
        public static readonly DependencyProperty CloneButtonTextProperty = DependencyProperty.Register(
            "CloneButtonText",
            typeof(string),
            typeof(ReusableEntitySelector),
            new PropertyMetadata(default(string)));

        /// <summary>
        /// the previously defined entities template
        /// </summary>
        public static readonly DependencyProperty PreviouslyDefinedEntitiesTemplateProperty =
            DependencyProperty.Register(
                "PreviouslyDefinedEntitiesTemplate",
                typeof(HierarchicalDataTemplate),
                typeof(ReusableEntitySelector),
                new PropertyMetadata(default(HierarchicalDataTemplate)));

        /// <summary>
        /// The rename command property.
        /// </summary>
        public static readonly DependencyProperty RenameEntityProperty =
            DependencyProperty.Register(
                "RenameEntity",
                typeof(ICommand),
                typeof(ReusableEntitySelector),
                new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// Initializes a new instance of the <see cref="ReusableEntitySelector"/> class.
        /// </summary>
        public ReusableEntitySelector()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The MouseDoubleClicked event for the radio button.
        /// </summary>
        public event EventHandler RadioButtonMouseDoubleClicked;

        /// <summary>
        /// Gets or sets the RenameEntity
        /// </summary>
        public ICommand RenameEntity
        {
            get
            {
                return (ICommand)GetValue(RenameEntityProperty);
            }

            set
            {
                SetValue(RenameEntityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the context menu for the right hand entities.
        /// </summary>
        public ContextMenu EntityContextMenu
        {
            get
            {
                return (ContextMenu)GetValue(EntityContextMenuProperty);
            }

            set
            {
                SetValue(EntityContextMenuProperty, value);
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
        /// Gets or sets the list of previously defined entities
        /// </summary>
        public IEnumerable PreviouslyDefinedEntities
        {
            get
            {
                return (IEnumerable)this.GetValue(PreviouslyDefinedEntitiesProperty);
            }

            set
            {
                this.SetValue(PreviouslyDefinedEntitiesProperty, value);
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
        /// Gets or sets the left hand title
        /// </summary>
        public string LeftHandTitle
        {
            get
            {
                return (string)this.GetValue(LeftHandTitleProperty);
            }

            set
            {
                this.SetValue(LeftHandTitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the right hand title
        /// </summary>
        public string RightHandTitle
        {
            get
            {
                return (string)this.GetValue(RightHandTitleProperty);
            }

            set
            {
                this.SetValue(RightHandTitleProperty, value);
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
        /// Gets or sets the Rename button text
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
        /// Gets or sets the previously
        /// </summary>
        public HierarchicalDataTemplate PreviouslyDefinedEntitiesTemplate
        {
            get
            {
                return (HierarchicalDataTemplate)this.GetValue(PreviouslyDefinedEntitiesTemplateProperty);
            }

            set
            {
                this.SetValue(PreviouslyDefinedEntitiesTemplateProperty, value);
            }
        }

        /// <summary>
        /// the handler for radio button double clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// the parameter
        /// </param>
        protected virtual void OnRadioButtonDoubleClick(object sender, EventArgs e)
        {
            var handler = this.RadioButtonMouseDoubleClicked;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnWindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            var control = (Control)sender;
            var item = (PromptNotification)control.DataContext;
            if (item.IsValid)
            {
                EntitiesGrid.Focus();
            }
        }
    }
}
