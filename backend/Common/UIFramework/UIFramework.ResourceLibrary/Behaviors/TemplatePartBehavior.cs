namespace Luminator.UIFramework.ResourceLibrary.Behaviors
{
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Markup;

    /// <summary>
    /// The template part behavior.
    /// </summary>
    [ContentProperty("Behaviors")]
    public class TemplatePartBehavior : Behavior<Control>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatePartBehavior"/> class.
        /// </summary>
        public TemplatePartBehavior()
        {
            this.Helper = new TemplatePartHelper<Behavior>(this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the behaviors.
        /// </summary>
        public List<Behavior> Behaviors
        {
            get
            {
                return this.Helper.Children;
            }
        }

        /// <summary>
        /// Gets the helper.
        /// </summary>
        public TemplatePartHelper<Behavior> Helper { get; private set; }

        /// <summary>
        /// Gets or sets the template part name.
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
        /// The on attached.
        /// </summary>
        protected override void OnAttached()
        {
            this.AssociatedObject.Initialized += (s, e) => this.Helper.AssociateChildren();
            this.AssociatedObject.Loaded += (s, e) => this.Helper.AssociateChildren();
        }

        #endregion
    }
}