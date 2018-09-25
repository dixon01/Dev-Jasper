//--------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      Microsoft Public License (Ms-PL) which can be found in the License.rtf 
//      at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Reflection;
    using System.Windows.Forms;

    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Policy.Exclusions;
    using Gorba.Common.Tfs.Tools.StyleCopCheckInPolicy.Properties;

    /// <summary>
    /// Contains various utility methods.
    /// </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Displays an error message box.
        /// </summary>
        /// <param name="owner">The owner of the message box.</param>
        /// <param name="message">The unformatted message to be displayed.</param>
        /// <param name="ex">The exception that was caught.</param>
        public static void DisplayErrorMessageBox(IWin32Window owner, string message, Exception ex)
        {
            string text = null;

            if (ex != null)
            {
                // Locate the root cause of the problem.
                Exception current = ex;

                while (current.InnerException != null)
                {
                    current = current.InnerException;
                }

                text = string.Format(CultureInfo.CurrentCulture, message, current.Message);
            }
            else
            {
                text = message;
            }

            MessageBox.Show(owner, text, Resources.Message_PolicyType, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Gets a collection containing all the policy exclusion types.
        /// </summary>
        /// <returns>A new collection of policy exclusion types.</returns>
        public static Collection<PolicyExclusionType> GetPolicyExclusionTypes()
        {
            Collection<PolicyExclusionType> result = new Collection<PolicyExclusionType>();

            foreach (FieldInfo field in typeof(PolicyExclusionType).GetFields())
            {
                ExclusionAttribute attribute = GetExclusionAttributeByField(field);
                if (attribute != null)
                {
                    result.Add((PolicyExclusionType)Enum.Parse(typeof(PolicyExclusionType), field.Name, true));
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether a flag has been set.
        /// </summary>
        /// <param name="input">The value to test.</param>
        /// <param name="match">The value to check for.</param>
        /// <returns><b>true</b> if the flag has been set, otherwise <b>false</b>.</returns>
        public static bool IsFlagSet(EvaluateOnType input, EvaluateOnType match)
        {
            return (input & match) == match;
        }

        /// <summary>
        /// Retrieves a <see cref="ExclusionAttribute"/> for the exclusion type specified.
        /// </summary>
        /// <param name="exclusionType">The exclusion type to check.</param>
        /// <returns>A <see cref="ExclusionAttribute"/> if available, otherwise a null reference (<b>Nothing</b> in Visual Basic).</returns>
        public static ExclusionAttribute GetExclusionAttributeByEnum(PolicyExclusionType exclusionType)
        {
            ExclusionAttribute attribute = null;

            FieldInfo field = typeof(PolicyExclusionType).GetField(exclusionType.ToString());
            if (field != null)
            {
                attribute = GetExclusionAttributeByField(field);
            }

            return attribute;
        }

        /// <summary>
        /// Retrieves a <see cref="ExclusionAttribute"/> for the field specified.
        /// </summary>
        /// <param name="field">The field to check.</param>
        /// <returns>A <see cref="ExclusionAttribute"/> if available, otherwise a null reference (<b>Nothing</b> in Visual Basic).</returns>
        public static ExclusionAttribute GetExclusionAttributeByField(FieldInfo field)
        {
            ExclusionAttribute result = null;

            object[] attributes = field.GetCustomAttributes(typeof(ExclusionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                result = (ExclusionAttribute)attributes[0];
            }

            return result;
        }

        /// <summary>
        /// Retrieves a <see cref="ExclusionAttribute"/> for the type specified.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>A <see cref="ExclusionAttribute"/> if available, otherwise a null reference (<b>Nothing</b> in Visual Basic).</returns>
        public static ExclusionAttribute GetExclusionAttributeByType(Type type)
        {
            ExclusionAttribute attribute = null;

            if (type != null)
            {
                object[] attributes = type.GetCustomAttributes(typeof(ExclusionAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    attribute = (ExclusionAttribute)attributes[0];
                }
            }

            return attribute;
        }
    }
}