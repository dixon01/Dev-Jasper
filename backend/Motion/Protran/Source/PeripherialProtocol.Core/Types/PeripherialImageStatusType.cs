
namespace Luminator.PeripheralProtocol.Core.Types
{
    //IMAGE_ACCEPTED  = 0,    // Downloaded Image was accepted
    //DOWNLOAD_TIMEOUT,       // Image Download Timed Out
    //IMAGE_TOO_BIG,          // Image had too many records - see PCP_07_IMAGE_START_PKT.recordCount
    //RECORD_ERROR,           // Format or checksum error in embedded Intel Hex record

    /// <summary>The peripheral image status type.</summary>
    public enum PeripheralImageStatusType : byte
    {
        Undefined = 0xFF,
        Accepted = 0,
        DownloadTimeout = 1,
        InvalidRecordCount = 2,
        RecordError = 3
    }
}
