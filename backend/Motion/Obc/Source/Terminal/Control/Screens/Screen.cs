// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Screen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Screen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The base class for all screens that represent a main field.
    /// </summary>
    /// <typeparam name="TMainField">
    /// The type of main field represented by this screen.
    /// </typeparam>
    internal abstract class Screen<TMainField> : IScreen where TMainField : IMainField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Screen{TMainField}"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected Screen(TMainField mainField, IContext context)
        {
            this.MainField = mainField;
            this.Context = context;
        }

        /// <summary>
        /// Gets the MainField implementation for this screen
        /// </summary>
        public TMainField MainField { get; private set; }

        IMainField IScreen.MainField
        {
            get
            {
                return this.MainField;
            }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        protected IContext Context { get; private set; }

        /// <summary>
        /// Shows this screen.
        /// </summary>
        public virtual void Show()
        {
        }

        /// <summary>
        /// Hides this screen.
        /// </summary>
        public virtual void Hide()
        {
        }

        /// <summary>
        /// Shows a message box on this screen.
        /// </summary>
        /// <param name="msgBoxInfo">
        /// The message box information.
        /// </param>
        public virtual void ShowMessageBox(MessageBoxInfo msgBoxInfo)
        {
            this.MainField.ShowMessageBox(msgBoxInfo);
        }

        /// <summary>
        /// Hides the message box.
        /// </summary>
        public virtual void HideMessageBox()
        {
            this.MainField.HideMessageBox();
        }

        /// <summary>
        /// Shows a progress bar on this screen.
        /// </summary>
        /// <param name="progressInfo">
        /// The progress information.
        /// </param>
        public virtual void ShowProgressBar(IProgressBarInfo progressInfo)
        {
            this.MainField.ShowProgressBar(progressInfo);
        }

        /// <summary>
        /// Hides the progress bar.
        /// </summary>
        public virtual void HideProgressBar()
        {
            this.MainField.HideProgressBar();
        }
    }
}