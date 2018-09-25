// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderManagerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Base class for all render managers.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of the <see cref="ItemBase"/> subclass that is
    /// managed by this manager.
    /// </typeparam>
    /// <typeparam name="TContext">
    /// The type of render context to be given to the <see cref="Render"/>
    /// method. It has to be a sub-interface of <see cref="IRenderContext"/>.
    /// </typeparam>
    public abstract class RenderManagerBase<TItem, TContext> : IDisposable
        where TItem : ItemBase
        where TContext : IRenderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderManagerBase{TItem,TContext}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        internal RenderManagerBase(TItem item)
        {
            this.Item = item;
            this.Item.PropertyValueChanged += this.ItemOnPropertyValueChanged;
        }

        /// <summary>
        /// Gets the id of the <see cref="ItemBase"/>.
        /// <seealso cref="ItemBase.Id"/>
        /// </summary>
        public int ItemId
        {
            get
            {
                return this.Item.Id;
            }
        }

        /// <summary>
        /// Gets the element id.
        /// </summary>
        public int ElementId
        {
            get
            {
                return this.Item.ElementId;
            }
        }

        /// <summary>
        /// Gets the item which is managed by this class.
        /// </summary>
        protected TItem Item { get; private set; }

        /// <summary>
        /// Update the contents of this render manager.
        /// </summary>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public virtual void Update(TContext context)
        {
        }

        /// <summary>
        /// The update item position.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        public virtual void UpdateItemPosition(Rectangle position)
        {
            var textitem = this.Item as TextItem;
            if (textitem != null)
            {
                textitem.X = position.X;
            }
        }

        /// <summary>
        /// Renders the contents.
        /// </summary>
        /// <param name="alpha">
        /// The alpha value (0 = transparent, 1 = opaque).
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        public abstract void Render(double alpha, TContext context);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            this.Item.PropertyValueChanged -= this.ItemOnPropertyValueChanged;
        }

        /// <summary>
        /// Find the <see cref="RootRenderManager{TContext}"/> for a given
        /// screen id. If no corresponding manager is found, null is returned.
        /// </summary>
        /// <param name="id">
        /// The screen id.
        /// </param>
        /// <returns>
        /// The root renderer manager responsible for the given screen id or
        /// null if none was found.
        /// </returns>
        internal virtual RootRenderManager<TContext> FindRoot(int id)
        {
            return null;
        }

        /// <summary>
        /// Updates all items with the given update.
        /// This method does not traverse through different includes,
        /// therefore it is called after searching the right root
        /// (<see cref="FindRoot"/>)
        /// </summary>
        /// <param name="update">
        /// The screen update.
        /// </param>
        protected virtual void UpdateItems(ScreenUpdate update)
        {
            var id = this.ItemId;
            foreach (var itemUpdate in update.Updates)
            {
                if (itemUpdate.ScreenItemId == id)
                {
                    
                    this.Item.Update(itemUpdate);
                    
                }
            }
        }

        /// <summary>
        /// Method that can be overridden by subclasses to handle
        /// changes to properties of the <see cref="Item"/>.
        /// </summary>
        /// <param name="change">
        /// The change information.
        /// </param>
        protected virtual void HandlePropertyChange(AnimatedPropertyChangedEventArgs change)
        {
        }

        private void ItemOnPropertyValueChanged(object sender, AnimatedPropertyChangedEventArgs e)
        {
            this.HandlePropertyChange(e);
        }
    }
}