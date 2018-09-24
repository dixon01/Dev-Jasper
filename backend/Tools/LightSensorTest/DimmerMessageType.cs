namespace LightSensorTestor
{
    /// <summary>The dimmer message type.</summary>
    public enum DimmerMessageType : byte
    {
        /// <summary>The unknown.</summary>
        Unknown = 0,

        /// <summary>The dimmer poll.</summary>
        Poll = 0x1,

        /// <summary>The dimmer status.</summary>
        DimmerStatus = 0x2,

        /// <summary>The ack.</summary>
        Ack = 0x4,

        /// <summary>The nak.</summary>
        Nak = 0x5,

        /// <summary>The version request.</summary>
        VersionRequest = 0x10,

        /// <summary>The set sensor scale.</summary>
        SetSensorScale = 0x11,

        /// <summary>The set brightness.</summary>
        SetBrightness = 0x12,

        /// <summary>The set monitor power.</summary>
        SetMonitorPower = 0x13,

        /// <summary>The set mode.</summary>
        SetMode = 0x14,

        /// <summary>The query request.</summary>
        QueryRequest = 0x15,

        /// <summary>The version response.</summary>
        VersionResponse = 0x20,

        /// <summary>The query response.</summary>
        QueryResponse = 0x21
    }
}