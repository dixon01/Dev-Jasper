namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    public interface IAdHocRegistrationResponse : IAdHocResponse
    {
        bool IsRegistered { get; }
    }
}