// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDriverBlockInput.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDriverBlockInput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// The drive number block input field interface.
    /// </summary>
    public interface IDriverBlockInput : INumberInputBase
    {
        /// <summary>
        /// Gets or sets the block number.
        /// </summary>
        string Block { get; set; }

        /// <summary>
        /// Initialize the main field. Call it in the method show()
        /// </summary>
        /// <param name="mainCaption">
        /// The main caption
        /// </param>
        /// <param name="inputCaption">
        /// The caption directly above the input box
        /// </param>
        /// <param name="maxLen">
        /// The maximum input length.
        /// </param>
        /// <param name="alpha1">
        /// The alpha 1.
        /// </param>
        /// <param name="alpha2">
        /// The alpha 2.
        /// </param>
        /// <param name="alpha3">
        /// The alpha 3.
        /// </param>
        /// <param name="alpha4">
        /// The alpha 4.
        /// </param>
        /// <param name="alpha5">
        /// The alpha 5.
        /// </param>
        /// <param name="alpha6">
        /// The alpha 6.
        /// </param>
        /// <param name="alpha7">
        /// The alpha 7.
        /// </param>
        /// <param name="alpha8">
        /// The alpha 8.
        /// </param>
        /// <param name="alpha1ShortName">
        /// The alpha 1 Short Name.
        /// </param>
        /// <param name="alpha2ShortName">
        /// The alpha 2 Short Name.
        /// </param>
        /// <param name="alpha3ShortName">
        /// The alpha 3 Short Name.
        /// </param>
        /// <param name="alpha4ShortName">
        /// The alpha 4 Short Name.
        /// </param>
        /// <param name="alpha5ShortName">
        /// The alpha 5 Short Name.
        /// </param>
        /// <param name="alpha6ShortName">
        /// The alpha 6 Short Name.
        /// </param>
        /// <param name="alpha7ShortName">
        /// The alpha 7 Short Name.
        /// </param>
        /// <param name="alpha8ShortName">
        /// The alpha 8 Short Name.
        /// </param>
        void Init(
            string mainCaption,
            string inputCaption,
            int maxLen,
            string alpha1,
            string alpha2,
            string alpha3,
            string alpha4,
            string alpha5,
            string alpha6,
            string alpha7,
            string alpha8,
            string alpha1ShortName,
            string alpha2ShortName,
            string alpha3ShortName,
            string alpha4ShortName,
            string alpha5ShortName,
            string alpha6ShortName,
            string alpha7ShortName,
            string alpha8ShortName);
    }
}