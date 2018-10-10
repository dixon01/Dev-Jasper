// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LanguageManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// The language manager.
    /// </summary>
    public class LanguageManager
    {
        private static readonly object Locker = new object();

        private static LanguageManager instance;

        private readonly List<Language> languages = new List<Language>();

        private Language currentLanguage;

        private LanguageManager()
        {
            this.languages.Add(new Language(CultureInfo.CreateSpecificCulture("de"), 1));
            this.languages.Add(new Language(CultureInfo.CreateSpecificCulture("fr"), 2));
            this.languages.Add(new Language(CultureInfo.CreateSpecificCulture("en"), 3));

            this.currentLanguage = this.languages[0];
        }

        /// <summary>
        /// The current language changing event.
        /// This event can be used to cancel the change of the language.
        /// </summary>
        public event EventHandler<LanguageChangingEventArgs> CurrentLanguageChanging;

        /// <summary>
        /// The current language changed event.
        /// </summary>
        public event EventHandler CurrentLanguageChanged;

        /// <summary>
        /// Gets the single instance of this manager.
        /// </summary>
        public static LanguageManager Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                lock (Locker)
                {
                    return instance ?? (instance = new LanguageManager());
                }
            }
        }

        /// <summary>
        /// Gets or sets the current language.
        /// </summary>
        public Language CurrentLanguage
        {
            get
            {
                return this.currentLanguage;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (this.currentLanguage == value)
                {
                    return;
                }

                if (this.CurrentLanguageChanging != null)
                {
                    var args = new LanguageChangingEventArgs(value);
                    this.CurrentLanguageChanging(this, args);

                    if (args.Cancel)
                    {
                        return;
                    }
                }

                this.currentLanguage = value;

                if (this.CurrentLanguageChanged != null)
                {
                    this.CurrentLanguageChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the list of supported languages.
        /// </summary>
        public Language[] SupportedLanguages
        {
            get
            {
                return this.languages.ToArray();
            }
        }

        /// <summary>
        /// Gets a language by its name.
        /// </summary>
        /// <param name="langName">
        /// The language name.
        /// </param>
        /// <returns>
        /// The <see cref="Language"/> or null if not found.
        /// </returns>
        public Language GetLanguage(string langName)
        {
            foreach (var language in this.languages)
            {
                if (language.Name.Equals(langName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return language;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a language by its number.
        /// </summary>
        /// <param name="langNumber">
        /// The language number.
        /// </param>
        /// <returns>
        /// The <see cref="Language"/> or null if not found.
        /// </returns>
        public Language GetLanguage(int langNumber)
        {
            foreach (var language in this.languages)
            {
                if (language.Number == langNumber)
                {
                    return language;
                }
            }

            return null;
        }

        /// <summary>
        /// The language changing event args.
        /// </summary>
        public class LanguageChangingEventArgs : CancelEventArgs
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LanguageChangingEventArgs"/> class.
            /// </summary>
            /// <param name="language">
            /// The language.
            /// </param>
            internal LanguageChangingEventArgs(Language language)
            {
                this.Language = language;
            }

            /// <summary>
            /// Gets the language to change to.
            /// </summary>
            public Language Language { get; private set; }
        }
    }
}
