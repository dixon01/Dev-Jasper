// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopupWindow.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PopupWindow type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;

    /// <summary>
    /// Interaction logic for PopupWindowHeader.xaml
    /// </summary>
    [ContentProperty("PopupContent")]
    public partial class PopupWindow
    {
        /// <summary>
        /// The Dependency Property to the PopupContent Property defining the Content of the PopupWindow
        /// </summary>
        public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(object), typeof(PopupWindow), new PropertyMetadata(default(FrameworkElement)));

        /// <summary>
        /// The Dependency Property to the IsClosable Property
        /// </summary>
        public static readonly DependencyProperty IsClosableProperty = DependencyProperty.Register("IsClosable", typeof(bool), typeof(PopupWindow), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The Position Changed Event
        /// </summary>
        public static readonly RoutedEvent PositionChangedEvent = EventManager.RegisterRoutedEvent("PositionChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PopupWindow));

        /// <summary>
        /// The Property determining if the window can be dragged
        /// </summary>
        public static readonly DependencyProperty IsDragableProperty = DependencyProperty.Register("IsDragable", typeof(bool), typeof(PopupWindow), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The Property indicating if the Window can be collapsed
        /// </summary>
        public static readonly DependencyProperty IsCollapsibleProperty = DependencyProperty.Register("IsCollapsible", typeof(bool), typeof(PopupWindow), new PropertyMetadata(default(bool)));

        /// <summary>
        /// The Property Containing the Window Title
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PopupWindow), new PropertyMetadata(default(string)));

        private bool isDragging;

        private Point dragStartPosition;

        private Point dragMouseOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupWindow" /> class.
        /// </summary>
        public PopupWindow()
        {
            InitializeComponent();

            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.MouseMove += this.DragHandleMouseMove;

                Mouse.AddMouseUpHandler(this, this.DragHandleMouseUp);
            }
        }

        /// <summary>
        /// The Position Changed Event Accessor
        /// </summary>
        public event RoutedEventHandler PositionChanged
        {
            add { this.AddHandler(PositionChangedEvent, value); }
            remove { this.RemoveHandler(PositionChangedEvent, value); }
        }

        /// <summary>
        /// Gets or sets the Content of the PopupWindow
        /// </summary>
        public object PopupContent
        {
            get
            {
                return GetValue(PopupContentProperty);
            }

            set
            {
                SetValue(PopupContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the PopupWindow is closable
        /// </summary>
        public bool IsClosable
        {
            get
            {
                return (bool)GetValue(IsClosableProperty);
            }

            set
            {
                SetValue(IsClosableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the PopupWindow is to be dragged
        /// </summary>
        public bool IsDragable
        {
            get
            {
                return (bool)GetValue(IsDragableProperty);
            }

            set
            {
                SetValue(IsDragableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the PopupWindow is collapsible
        /// </summary>
        public bool IsCollapsible
        {
            get
            {
                return (bool)GetValue(IsCollapsibleProperty);
            }

            set
            {
                SetValue(IsCollapsibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title
        {
            get
            {
                return (string)GetValue(TitleProperty);
            }

            set
            {
                SetValue(TitleProperty, value);
            }
        }

        /// <summary>
        /// handles the drag start event
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the event args</param>
        private void DragHandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsDragable)
            {
                this.isDragging = true;
                this.dragStartPosition = e.GetPosition(Application.Current.MainWindow);
                var x = Canvas.GetLeft(this);
                var y = Canvas.GetTop(this);
                x = double.IsNaN(x) ? 0 : x;
                y = double.IsNaN(y) ? 0 : y;
                this.dragMouseOffset = new Point(x, y);
            }
        }

        /// <summary>
        /// handles the mouse movements
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the even arguments</param>
        private void DragHandleMouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDragging)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    var pos = e.GetPosition(Application.Current.MainWindow);
                    var delta = pos - this.dragStartPosition;

                    var x = this.dragMouseOffset.X + delta.X;
                    var y = this.dragMouseOffset.Y + delta.Y;
                    x = x > 0 ? x : 0;
                    y = y > 0 ? y : 0;
                    Canvas.SetLeft(this, x);
                    Canvas.SetTop(this, y);
                }
                else
                {
                    this.StopDragging();
                }
            }
        }

        /// <summary>
        /// handles the event if the user let's go of the mouse
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the event arguments</param>
        private void DragHandleMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.StopDragging();
        }

        /// <summary>
        /// handler to be executed after the dragging process ended
        /// </summary>
        private void StopDragging()
        {
            this.isDragging = false;

            var x = Canvas.GetLeft(this);
            var y = Canvas.GetTop(this);
            x = double.IsNaN(x) ? 0 : x;
            y = double.IsNaN(y) ? 0 : y;
            this.RaiseEvent(new PositionChangedRoutedEventArgs(new Point(x, y)));
        }

        /// <summary>
        /// The Event Arguments for the Position Changed event
        /// </summary>
        public class PositionChangedRoutedEventArgs : RoutedEventArgs
        {
            private readonly Point position;

            /// <summary>
            /// Initializes a new instance of the <see cref="PositionChangedRoutedEventArgs" /> class.
            /// </summary>
            /// <param name="position">the position</param>
            public PositionChangedRoutedEventArgs(Point position)
                : base(PopupWindow.PositionChangedEvent)
            {
                this.position = position;
            }

            /// <summary>
            /// Gets the Position
            /// </summary>
            public Point Position
            {
                get
                {
                    return this.position;
                }
            }
        }
    }
}
