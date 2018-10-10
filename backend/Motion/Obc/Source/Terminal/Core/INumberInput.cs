// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INumberInput.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the INumberInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// The number input interface.
    /// </summary>
    public interface INumberInput : INumberInputBase
    {
        /// <summary>
        ///   Initialize the main field. Call it in the method show()
        /// </summary>
        /// <param name = "mainCaption">The main caption</param>
        /// <param name = "inputCaption">The caption directly above the input box</param>
        /// <param name="maxLen">The maximum length of the input</param>
        void Init(string mainCaption, string inputCaption, int maxLen);
    }
}