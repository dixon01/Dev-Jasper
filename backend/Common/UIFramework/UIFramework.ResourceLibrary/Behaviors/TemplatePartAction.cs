// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplatePartAction.cs" company="">
//   Copyright (c) 2013
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.ResourceLibrary.Behaviors
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Markup;

    /// <summary>
    ///     The template part action.
    /// </summary>
    [ContentProperty("Actions")]
    public class TemplatePartAction : TriggerAction<Control>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TemplatePartAction" /> class.
        /// </summary>
        public TemplatePartAction()
        {
            this.Helper = new TemplatePartHelper<TriggerAction>(this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the actions.
        /// </summary>
        public List<TriggerAction> Actions
        {
            get
            {
                return this.Helper.Children;
            }
        }

        /// <summary>
        ///     Gets the helper.
        /// </summary>
        public TemplatePartHelper<TriggerAction> Helper { get; private set; }

        /// <summary>
        ///     Gets or sets the template part name.
        /// </summary>
        public string TemplatePartName
        {
            get
            {
                return this.Helper.Name;
            }

            set
            {
                this.Helper.Name = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        protected override void Invoke(object parameter)
        {
            foreach (TriggerAction action in this.Actions)
            {
                if (action.IsEnabled)
                {
                    MethodInfo methodInfo = typeof(TriggerAction).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(object) }, null);
                    methodInfo.Invoke(action, new[] { parameter });
                }
            }
        }

        /// <summary>
        ///     The on attached.
        /// </summary>
        protected override void OnAttached()
        {
            this.AssociatedObject.Loaded += (s, e) => this.Helper.AssociateChildren();
        }

        #endregion
    }
}