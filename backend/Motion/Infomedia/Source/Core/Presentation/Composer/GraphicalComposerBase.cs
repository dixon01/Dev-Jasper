// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GraphicalComposerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GraphicalComposerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Base class for all composers that handle graphical elements.
    /// </summary>
    /// <typeparam name="TElement">
    /// The type of the element used by this composer.
    /// </typeparam>
    public abstract partial class GraphicalComposerBase<TElement> : IGraphicalComposer
    {
        private IGraphicalComposer graphicalParent;

        /// <summary>
        /// Event for subclasses that is fired whenever the
        /// <see cref="GraphicalElementBase.VisibleProperty"/> changes
        /// </summary>
        public event EventHandler VisibleChanged;

        /// <summary>
        /// Gets the X coordinate of this presenter. It takes
        /// into account the parent's X coordinate.
        /// </summary>
        public int X
        {
            get
            {
                int x = this.Element.X;
                if (this.graphicalParent != null)
                {
                    x += this.graphicalParent.X;
                }

                return x;
            }
        }

        /// <summary>
        /// Gets the Y coordinate of this presenter. It takes
        /// into account the parent's Y coordinate.
        /// </summary>
        public int Y
        {
            get
            {
                int y = this.Element.Y;
                if (this.graphicalParent != null)
                {
                    y += this.graphicalParent.Y;
                }

                return y;
            }
        }

        /// <summary>
        /// Gets the width of this presenter. It takes
        /// into account the parent's width if available.
        /// This can be zero meaning this presenter has no horizontal bounds.
        /// </summary>
        public int Width
        {
            get
            {
                int width = this.Element.Width;
                if (this.graphicalParent != null && this.graphicalParent.Width > 0)
                {
                    // reduce the width if we don't fit in the parent's bounds
                    width = Math.Min(width, this.graphicalParent.Width - this.Element.X);
                }

                return width;
            }
        }

        /// <summary>
        /// Gets the height of this presenter. It takes
        /// into account the parent's height if available.
        /// This can be zero meaning this presenter has no vertical bounds.
        /// </summary>
        public int Height
        {
            get
            {
                int height = this.Element.Height;
                if (this.graphicalParent != null && this.graphicalParent.Height > 0)
                {
                    // reduce the height if we don't fit in the parent's bounds
                    height = Math.Min(height, this.graphicalParent.Height - this.Element.Y);
                }

                return height;
            }
        }

        /// <summary>
        /// Gets the dynamic property handler for the visible flag.
        /// </summary>
        protected DynamicPropertyHandler VisibleHandler
        {
            get
            {
                return this.HandlerVisible;
            }
        }

        /// <summary>
        /// Method to check if the enabled property on this element is true.
        /// This method also checks the parent's enabled property.
        /// If no dynamic "Enabled" property is defined, this method returns true.
        /// Subclasses can override this method to provide additional
        /// evaluation (e.g. check if files are available or other conditions
        /// are met).
        /// </summary>
        /// <returns>
        /// true if there is no property defined or if the property evaluates to true is valid.
        /// </returns>
        public virtual bool IsVisible()
        {
            if (this.graphicalParent != null && !this.graphicalParent.IsVisible())
            {
                return false;
            }

            return this.HandlerVisible.BoolValue;
        }

        /// <summary>
        /// Raises the <see cref="VisibleChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseVisibleChanged(EventArgs e)
        {
            var handler = this.VisibleChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        partial void Initialize()
        {
            this.graphicalParent = this.Parent as IGraphicalComposer;
            if (this.graphicalParent != null)
            {
                this.graphicalParent.VisibleChanged += this.OnVisibleChanged;
            }
        }

        partial void Update()
        {
            this.RaiseVisibleChanged(EventArgs.Empty);
        }

        partial void Deinitialize()
        {
            if (this.graphicalParent != null)
            {
                this.graphicalParent.VisibleChanged -= this.OnVisibleChanged;
            }
        }

        private void OnVisibleChanged(object sender, EventArgs e)
        {
            this.RaiseVisibleChanged(e);
        }
    }
}