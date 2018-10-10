namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    public enum Command
    {
        Bitmap = 0x2000,
        Font = 0x2001,
        Text = 0x2002,
        Window = 0x2004,

        ClearWindow = 0x3000,
        ClearBitmap = 0x3001,
        ClearAll = 0x3002,

        GetStatus = 0x500B
    }
}