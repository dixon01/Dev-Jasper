// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DropTargetAdorner.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The DropTargetAdorner.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions.DragDropExtension
{
    using System;
    using System.Windows;
    using System.Windows.Documents;

    /// <summary>
    /// The DropTargetAdorner class
    /// </summary>
    public abstract class DropTargetAdorner : Adorner
    {
        private readonly AdornerLayer adornerLayer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropTargetAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">the UI element</param>
        protected DropTargetAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this.adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            this.adornerLayer.Add(this);
            this.IsHitTestVisible = false;
        }

        /// <summary>
        /// Gets or sets the drop info
        /// </summary>
        public DropInfo DropInfo { get; set; }

        /// <summary>
        /// The create drop target adorner method
        /// </summary>
        /// <param name="type">the type of adorner to create</param>
        /// <param name="adornedElement">the adorned element</param>
        /// <returns>the target adorner</returns>
        public static DropTargetAdorner Create(Type type, UIElement adornedElement)
        {
            if (!typeof(DropTargetAdorner).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    "The requested adorner class does not derive from DropTargetAdorner.");
            }

            var constructorInfo = type.GetConstructor(new[] { typeof(UIElement) });

            return (DropTargetAdorner)constructorInfo.Invoke(new object[] { adornedElement });
        }

        /// <summary>
        /// detaches the adorner layer
        /// </summary>
        public void Detach()
        {
            this.adornerLayer.Remove(this);
        }
    }
}