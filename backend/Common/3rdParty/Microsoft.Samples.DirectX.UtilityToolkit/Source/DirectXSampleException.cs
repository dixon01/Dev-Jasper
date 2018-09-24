//--------------------------------------------------------------------------------------
// File: DXMUTException.cs
//
// Holds all exception classes for the DX Managed Utility Toolkit
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;
using System.Collections;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    /// <summary>Base class for sample exceptions</summary>
    public class DirectXSampleException : System.ApplicationException 
    {
        public DirectXSampleException(string message) : base(message) {}
        public DirectXSampleException(string message, Exception inner) : base(message, inner) {}
    }
}