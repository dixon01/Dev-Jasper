// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslateExtension.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TranslateExtension.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Extensions
{
    using System;
    using System.Resources;
    using System.Windows.Markup;

    /// <summary>
    /// The translate extension.
    /// </summary>
    public class TranslateExtension : MarkupExtension
    {
        /// <summary>
        /// The resource man.
        /// </summary>
        private static ResourceManager resourceMan;

        /// <summary>
        /// The key.
        /// </summary>
        private string key;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateExtension"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        public TranslateExtension(string key)
        {
            this.key = key;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [ConstructorArgument("key")]
        public string Key
        {
            get { return this.key; }
            set { this.key = value; }
        }

        /// <summary>
        /// Gets the resource manager.
        /// </summary>
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    var temp = new ResourceManager("Gorba.Center.Media.Core.Resources.MediaStrings", typeof(TranslateExtension).Assembly);
                    resourceMan = temp;
                }

                return resourceMan;
            }
        }

        /// <summary>
        /// The get string.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetString(string key)
        {
            return ResourceManager.GetString(key);
        }

        /// <summary>
        /// The provide value.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetString(this.key);
        }
    }
}
