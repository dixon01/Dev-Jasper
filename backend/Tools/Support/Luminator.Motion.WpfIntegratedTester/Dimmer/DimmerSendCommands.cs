namespace Luminator.Motion.WpfIntegratedTester.Dimmer
{
    using System.ComponentModel;

    public enum DimmerSendCommands
    {
        [Description("Poll Request")]
        PollRequest,

        [Description("Version Request")]
        VersionRequest,

        [Description("Query Request")]
        QueryRequest
    }
}
