// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditorViewBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The editor view base which contains shared code used by all editors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views.Editors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;

    /// <summary>
    /// The editor view base which contains shared code used by all editors.
    /// </summary>
    public class EditorViewBase : UserControl
    {
        /// <summary>
        /// the interaction start position property
        /// </summary>
        public static readonly DependencyProperty InteractionStartPositionProperty =
            DependencyProperty.Register(
                "InteractionStartPosition",
                typeof(Point?),
                typeof(EditorViewBase),
                new PropertyMetadata(default(Point?)));

        /// <summary>
        /// the interaction rectangle property
        /// </summary>
        public static readonly DependencyProperty InteractionRectangleProperty =
            DependencyProperty.Register(
               "InteractionRectangle", typeof(Rect), typeof(EditorViewBase), new PropertyMetadata(default(Rect)));

        /// <summary>
        /// the mouse position
        /// </summary>
        public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register(
            "MousePosition", typeof(Point), typeof(EditorViewBase), new PropertyMetadata(default(Point)));

        /// <summary>
        /// The position property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position",
            typeof(double),
            typeof(EditorViewBase),
            new PropertyMetadata(default(double)));

        /// <summary>
        /// The zoom property.
        /// </summary>
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            "Zoom",
            typeof(double),
            typeof(EditorViewBase),
            new PropertyMetadata(100d, OnZoomChanged));

        /// <summary>
        /// the zoom modifier key is pressed property
        /// </summary>
        public static readonly DependencyProperty ZoomModifierIsPressedProperty =
            DependencyProperty.Register(
                "ZoomModifierIsPressed",
                typeof(bool),
                typeof(EditorViewBase),
                new PropertyMetadata(default(bool)));

        /// <summary>
        /// the selected tool
        /// </summary>
        public static readonly DependencyProperty SelectedToolProperty = DependencyProperty.Register(
            "SelectedTool",
            typeof(EditorToolType),
            typeof(EditorViewBase),
            new PropertyMetadata(default(EditorToolType), OnSelectedToolChanged));

        /// <summary>
        /// The min drag distance.
        /// </summary>
        protected readonly Vector MinDragDistance = new Vector(
       SystemParameters.MinimumHorizontalDragDistance, SystemParameters.MinimumVerticalDragDistance);

        /// <summary>
        /// Gets or sets the currently selected tool
        /// </summary>
        public EditorToolType SelectedTool
        {
            get
            {
                return (EditorToolType)this.GetValue(SelectedToolProperty);
            }

            set
            {
                this.SetValue(SelectedToolProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the interaction start position
        /// </summary>
        public Point? InteractionStartPosition
        {
            get
            {
                return (Point?)this.GetValue(InteractionStartPositionProperty);
            }

            set
            {
                this.SetValue(InteractionStartPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the interaction rectangle
        /// </summary>
        public Rect InteractionRectangle
        {
            get
            {
                return (Rect)this.GetValue(InteractionRectangleProperty);
            }

            set
            {
                this.SetValue(InteractionRectangleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mouse position
        /// </summary>
        public Point MousePosition
        {
            get
            {
                return (Point)this.GetValue(MousePositionProperty);
            }

            set
            {
                this.SetValue(MousePositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public double Position
        {
            get
            {
                return (double)this.GetValue(PositionProperty);
            }

            set
            {
                this.SetValue(PositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the zoom.
        /// </summary>
        public double Zoom
        {
            get
            {
                return (double)this.GetValue(ZoomProperty);
            }

            set
            {
                this.SetValue(ZoomProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the zoom modifier key is pressed
        /// </summary>
        public bool ZoomModifierIsPressed
        {
            get
            {
                return (bool)this.GetValue(ZoomModifierIsPressedProperty);
            }

            set
            {
                this.SetValue(ZoomModifierIsPressedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current element under mouse.
        /// </summary>
        protected DrawableElementDataViewModelBase CurrentElementUnderMouse { get; set; }

        /// <summary>
        /// Gets or sets the highest element under mouse.
        /// </summary>
        protected DrawableElementDataViewModelBase HighestElementUnderMouse { get; set; }

        /// <summary>
        /// Gets or sets the last mouse position.
        /// </summary>
        protected Point LastMousePosition { get; set; }

        /// <summary>
        /// Gets or sets the last tool.
        /// </summary>
        protected EditorToolType? LastTool { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether suspend tool change events.
        /// </summary>
        protected bool SuspendToolChangeEvents { get; set; }

        /// <summary>
        /// The on zoom changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (EditorViewBase)d;
            if (control != null)
            {
                control.OnZoomChangedInternal();
            }
        }

        /// <summary>
        /// The on zoom changed internal.
        /// </summary>
        protected virtual void OnZoomChangedInternal()
        {
        }

        /// <summary>
        /// Checks if a modal dialog is open or a textbox has focus
        /// </summary>
        /// <returns>
        /// <c>true</c> if the focus is on the editor and the event should be handled; <c>false</c> otherwise.
        /// </returns>
        protected bool CanProcessKeyDown()
        {
            if (InteractionAction.HasOpenModalDialog)
            {
                return false;
            }

            var focusedControl = Keyboard.FocusedElement;
            var hasTextBoxFocus = focusedControl.GetType() == typeof(TextBox);
            if (hasTextBoxFocus)
            {
                return false;
            }

            var modifiers = new ModifiersState();
            if (modifiers.IsControlPressed)
            {
                this.ZoomModifierIsPressed = true;
            }

            return true;
        }

        /// <summary>
        /// Checks if a modal dialog is open or a textbox has focus.
        /// </summary>
        /// <param name="e">
        /// The key event args.
        /// </param>
        /// <param name="modifiers">
        /// The modifiers that are pressed when the key up event is fired.
        /// </param>
        /// <returns>
        /// <c>true</c> if the focus is on the editor and the event should be handled; <c>false</c> otherwise.
        /// </returns>
        protected bool CanProcessKeyUp(KeyEventArgs e, out ModifiersState modifiers)
        {
            modifiers = new ModifiersState();
            if (InteractionAction.HasOpenModalDialog && e.Key != Key.S)
            {
                return false;
            }

            var focusedControl = Keyboard.FocusedElement;
            var hasTextBoxFocus = focusedControl.GetType() == typeof(TextBox);
            var hasListViewItemFocus = focusedControl is ListViewItem;
            if (hasTextBoxFocus || (hasListViewItemFocus && (e.Key != Key.S && e.Key != Key.Delete)))
            {
                return false;
            }

            modifiers = new ModifiersState();

            if (!modifiers.IsControlPressed)
            {
                this.ZoomModifierIsPressed = false;
            }

            return true;
        }

        /// <summary>
        /// Handles the keys that are shared on all editors.
        /// </summary>
        /// <param name="editor">
        /// The editor.
        /// </param>
        /// <param name="e">
        /// The key event args.
        /// </param>
        /// <param name="modifiers">
        /// The modifiers.
        /// </param>
        /// <returns>
        /// <c>true</c> if the key was handled; <c>false</c> otherwise.
        /// </returns>
        protected virtual bool HandleSharedKeyUp(EditorViewModelBase editor, KeyEventArgs e, ModifiersState modifiers)
        {
            switch (e.Key)
            {
                case Key.Space:
                    if (this.LastTool.HasValue)
                    {
                        this.SelectedTool = this.LastTool.Value;
                        return true;
                    }

                    return true;
                case Key.V:
                    this.SelectedTool = EditorToolType.Move;
                    return true;
            }

            return false;
        }

        private static void OnSelectedToolChanged(
         DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var instance = (EditorViewBase)dependencyObject;
            if (!instance.SuspendToolChangeEvents)
            {
                instance.LastTool = null;
            }
        }
    }
}
