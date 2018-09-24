//--------------------------------------------------------------------------------------
// File: DXMUTData.cs
//
// DirectX SDK Managed Direct3D sample framework data class
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//--------------------------------------------------------------------------------------
using System;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    /// <summary>Event arguments for device creation/reset</summary>
    public class DeviceEventArgs : EventArgs
    {
        // Class data
        public Device Device;
        public SurfaceDescription BackBufferDescription;

        public DeviceEventArgs(Device d, SurfaceDescription desc) 
        {
            Device = d;
            BackBufferDescription = desc;
        }
    }
}