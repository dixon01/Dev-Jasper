// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageScreen.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LanguageScreen type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Control.Screens
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Obc.Terminal;
    using Gorba.Motion.Obc.Terminal.Control.Config;
    using Gorba.Motion.Obc.Terminal.Control.DFA;
    using Gorba.Motion.Obc.Terminal.Core;

    /// <summary>
    /// The language screen.
    /// </summary>
    internal class LanguageScreen : SimpleListScreen
    {
        private readonly List<Language> languages = new List<Language>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageScreen"/> class.
        /// </summary>
        /// <param name="mainField">
        /// The main field.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public LanguageScreen(IList mainField, IContext context)
            : base(mainField, context)
        {
            TerminalConfig config = context.ConfigHandler.GetConfig();

            foreach (var langName in config.Languages.Value)
            {
                var language = LanguageManager.Instance.GetLanguage(langName);
                if (language != null)
                {
                    this.languages.Add(language);
                }
            }
        }

        /// <summary>
        /// Gets the list that will be shown to the user. The user will/may select an item from this list.
        /// </summary>
        protected override List<string> List
        {
            get
            {
                return this.languages.ConvertAll(lang => lang.NativeShortName);
            }
        }

        /// <summary>
        /// Gets the caption from the screen
        /// </summary>
        protected override string Caption
        {
            get
            {
                return ml.ml_string(51, "Change Language"); // MLHIDE
            }
        }

        /// <summary>
        ///   This method will be called when the user has selected an entry.
        ///   Implement your action here. The index is the selected item from the GetList() method
        /// </summary>
        /// <param name = "index">
        /// The selected index.
        /// </param>
        protected override void ItemSelected(int index)
        {
            LanguageManager.Instance.CurrentLanguage = this.languages[index];
            this.Context.ShowRootScreen();
        }
    }
}