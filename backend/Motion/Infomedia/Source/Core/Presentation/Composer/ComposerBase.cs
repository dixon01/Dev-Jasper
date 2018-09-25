// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ComposerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ComposerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Base class for all composers that are generated using <c>Composers.tt</c>.
    /// </summary>
    /// <typeparam name="TElement">
    /// The type of the element used by this composer.
    /// </typeparam>
    public class ComposerBase<TElement> : IComposer
        where TElement : ElementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComposerBase{TElement}"/> class.
        /// </summary>
        /// <param name="context">
        /// The presentation context.
        /// </param>
        /// <param name="parent">
        /// The parent composer.
        /// </param>
        /// <param name="element">
        /// The element used by this composer.
        /// </param>
        protected ComposerBase(IPresentationContext context, IComposer parent, TElement element)
        {
            this.Context = context;
            this.Parent = parent;
            this.Element = element;
        }

        /// <summary>
        /// Gets the generic context for which this presenter was created.
        /// </summary>
        protected IPresentationContext Context { get; private set; }

        /// <summary>
        /// Gets the parent composer. Can be null.
        /// </summary>
        protected IComposer Parent { get; private set; }

        /// <summary>
        /// Gets the element from which this composer should create an item.
        /// </summary>
        protected TElement Element { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
