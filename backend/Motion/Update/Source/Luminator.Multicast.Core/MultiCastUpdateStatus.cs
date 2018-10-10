namespace Luminator.Multicast.Core
{
    using System.ComponentModel;

    public enum MultiCastUpdateStatus
    {
        [Description("Idle")]
        Idle,

        [Description("RestorePreviosNetworkState")]
        RestorePreviosNetworkState,

        [Description("Started")]
        Started,

        [Description("Waiting For Mcu Response")]
        WaitingForMcuResponse,

        [Description("Received Mcu Response")]
        ReceivedMcuResponse,

        [Description("Static Ip Set")]
        StaticIpSet,

        [Description("Static Ip Verified")]
        StaticIpVerified,

        [Description("Ftp Upload Initialized")]
        FtpUploadInitialized,

        [Description("Ftp Upload Verified")]
        FtpUploadVerified,

        [Description("Ftp Update Started")]
        FtpUpdateStarted,

        [Description("Ftp Update In Progress")]
        FtpUpdateInProgress,

        [Description("Ftp Transfer Complete")]
        FtpTransferComplete,

        [Description("Ftp Delete Start")]
        FtpDeleteStart,

        [Description("Ftp Delete Verified")]
        FtpDeleteVerified,

        [Description("All Done")]
        AllDone,

        [Description("Error During Multicast")]
        ErrorDuringMulticast
    }
}