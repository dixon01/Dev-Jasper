// -----------------------------------------------------------------------
// <copyright file="GorbaSettingsBase.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Gorba.Common.ComponentModel.Base
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.XPath;

    /// <summary>
    /// Base class to handle settings.
    /// This class offers functionalities to read the applicationSettings section from config file based on same file like app.config.
    /// </summary>
    public abstract class GorbaSettingsBase
    {
        /// <summary>
        /// Store field for the name of the section.
        /// </summary>
        private string sectionName = "//configuration//add";

        /// <summary>
        /// Cache dictionary for the settings.
        /// </summary>
        private Dictionary<string, string> settings = new Dictionary<string, string>();

        /// <summary>
        /// Store field for the <see cref="XPathExpression"/>.
        /// </summary>
        private XPathExpression expression = null;

        /// <summary>
        /// Store field for the <see cref="XPathDocument"/>;
        /// </summary>
        private XPathDocument document = null;

        /// <summary>
        /// Store field to handle the <see cref="XPathNavigator"/>.
        /// </summary>
        private XPathNavigator navigator = null;

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <param name="key">Desired key for the setting.</param>
        /// <returns>the string or an empty string.</returns>
        public string this[string key]
        {
            get
            {
                if (this.settings != null)
                {
                    if (this.settings.ContainsKey(key))
                    {
                        return this.settings[key];
                    } // if
                } // if

                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the value as a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The trimmed string.</returns>
        protected string AsString(string value, string defaultValue)
        {
            var trimmedValue = value.Trim();
            if (string.IsNullOrWhiteSpace(trimmedValue))
            {
                return defaultValue;
            }
            else
            {
                return trimmedValue;
            } // else
        }

        /// <summary>
        /// Casts the string to an integer.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The <see cref="Int32"/> value.</returns>
        protected int AsInteger(string value, int defaultValue)
        {
            int ret = 0;
            if (int.TryParse(value, out ret))
            {
                return ret;
            }
            else
            {
                return defaultValue;
            } // else
        }

        /// <summary>
        /// Reads ApplicationSettings section form config file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        /// Returns true if the file exists and false else.
        /// </returns>
        protected bool ReadApplicationSettingsFromFile(string fileName)
        {
            this.document = null;
            this.navigator = null;
            this.expression = null;
            this.settings.Clear();

            var trimmedFileName = fileName.Trim();
            if (!string.IsNullOrWhiteSpace(trimmedFileName))
            {
                if (File.Exists(fileName))
                {
                    this.document = new XPathDocument(fileName);
                    this.navigator = this.document.CreateNavigator();
                    this.expression = this.navigator.Compile(this.sectionName);
                    var xpathNodeIterator = this.navigator.Select(this.expression);

                    // Get the appSettings section.
                    string key, value;
                    foreach (XPathNavigator nodeSettings in xpathNodeIterator)
                    {
                        key = nodeSettings.GetAttribute("key", string.Empty);
                        value = nodeSettings.GetAttribute("value", string.Empty);
                        if (key != string.Empty && value != string.Empty)
                        {
                            this.settings.Add(key, value);
                        } // if
                    } // foreach

                    return true;
                } // if
            } // if

            return false;
        }
    }
}
