// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionState{TData}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ActionState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Wizard.ViewModels
{
    /// <summary>
    /// <see cref="ActionState"/> with data relative to the operation.
    /// The <see cref="Data"/> property can be used to store any kind of data needed by this action.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class ActionState<TData> : ActionState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionState&lt;TData&gt;"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public ActionState(TData data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public TData Data { get; set; }
    }
}