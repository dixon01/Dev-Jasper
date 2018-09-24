// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="">
//   Copyright (c) 2013
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.UIFramework.ResourceLibrary.Behaviors
{
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    /// <summary>
    /// The template part helper.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class TemplatePartHelper<T>
        where T : IAttachedObject
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatePartHelper{T}"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public TemplatePartHelper(IAttachedObject parent)
        {
            this.Parent = parent;
            this.Children = new List<T>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the children.
        /// </summary>
        public List<T> Children { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public IAttachedObject Parent { get; set; }

        /// <summary>
        /// Gets or sets the template part.
        /// </summary>
        public Control TemplatePart { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The associate children.
        /// </summary>
        public void AssociateChildren()
        {
            if (this.TemplatePart != null)
            {
                return;
            }

            var control = this.Parent.AssociatedObject as Control;
            ControlTemplate template = control.Template;
            if (template == null)
            {
                return;
            }

            string partName = "PART_" + this.Name;
            this.TemplatePart = template.FindName(partName, control) as Control;
            if (this.TemplatePart == null)
            {
                return;
            }

            foreach (T child in this.Children)
            {
                child.Attach(this.TemplatePart);
            }
        }

        #endregion
    }
}