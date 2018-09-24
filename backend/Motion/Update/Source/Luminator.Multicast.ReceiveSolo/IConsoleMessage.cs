namespace Luminator.Multicast.ReceiveSolo
{
    public interface IConsoleMessage
    {
        void Success(string message);

        void Error(string message);

        void Warning(string message);

        void Highlight(string message);
    }
}