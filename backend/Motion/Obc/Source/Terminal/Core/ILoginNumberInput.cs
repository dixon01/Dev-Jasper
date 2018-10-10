// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILoginNumberInput.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ILoginNumberInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    using System;

    /// <summary>
    /// The login number input interface.
    /// </summary>
    public interface ILoginNumberInput : INumberInputBase
    {
        /// <summary>
        ///   The changed language event. Parameter is an int. 1: German, 2: French, 3: English
        /// </summary>
        event EventHandler<IndexEventArgs> LanguageChanged;

        /// <summary>
        ///   Initialize the main field. Call it in the method show()
        /// </summary>
        /// <param name="mainCaption">The main caption</param>
        /// <param name="inputCaption">The input caption</param>
        /// <param name="maxLen">The maximum length of the input</param>
        /// <param name="input2Caption">The second input caption</param>
        /// <param name="showGerman">Enable German button. -> LanguageEvent 1</param>
        /// <param name="showFrench">Enable French button. -> LanguageEvent 2</param>
        /// <param name="showEnglish">Enable English button. -> LanguageEvent 3</param>
        void Init(
            string mainCaption,
            string inputCaption,
            int maxLen,
            string input2Caption,
            bool showGerman,
            bool showFrench,
            bool showEnglish);
    }
}