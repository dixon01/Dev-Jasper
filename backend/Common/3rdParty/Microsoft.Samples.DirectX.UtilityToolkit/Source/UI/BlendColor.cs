//--------------------------------------------------------------------------------------
// File: DXMUTGui.cs
//
// DirectX SDK Managed Direct3D GUI Sample Code
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;

using Microsoft.DirectX.Direct3D;

namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    /// <summary>
    /// Blends colors
    /// </summary>
    public struct BlendColor
    {
        public ColorValue[] States; // Modulate colors for all possible control states
        public ColorValue Current; // Current color

        /// <summary>Initialize the color blending</summary>
        public void Initialize(ColorValue defaultColor, ColorValue disabledColor, ColorValue hiddenColor)
        {
            // Create the array
            States = new ColorValue[(int)ControlState.LastState];
            for(int i = 0; i < States.Length; i++)
            {
                States[i] = defaultColor;
            }

            // Store the data
            States[(int)ControlState.Disabled] = disabledColor;
            States[(int)ControlState.Hidden] = hiddenColor;
            Current = hiddenColor;
        }
        /// <summary>Initialize the color blending</summary>
        public void Initialize(ColorValue defaultColor) { Initialize( defaultColor, new ColorValue(0.5f, 0.5f, 0.5f, 0.75f),new ColorValue()); }

        /// <summary>Blend the colors together</summary>
        public void Blend(ControlState state, float elapsedTime, float rate)
        {
            if ((States == null) || (States.Length == 0) )
                return; // Nothing to do

            ColorValue destColor = States[(int)state];
            Current = ColorOperator.Lerp(Current, destColor, 1.0f - (float)Math.Pow(rate, 30 * elapsedTime) );
        }
        /// <summary>Blend the colors together</summary>
        public void Blend(ControlState state, float elapsedTime) { Blend(state, elapsedTime, 0.7f); }
    }

    #region Dialog Resource Manager

    #endregion

    #region Abstract Control class

    #endregion

    #region StaticText control

    #endregion

    #region Button control

    #endregion

    #region Checkbox Control

    #endregion

    #region RadioButton Control

    #endregion

    #region Scrollbar control

    #endregion

    #region ComboBox Control

    #endregion

    #region Slider Control

    #endregion

    #region Listbox Control

    #endregion
}