namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    public class Fcs16
    {
        private readonly ushort[] fcsTab = new ushort[256];

        public Fcs16(int polynomial)
        {
            for (int i = 0; i < 256; i++)
            {
                int value = i;
                for (int j = 0; j < 8; j++)
                {
                    value = (value & 1) == 1 ? (value >> 1) ^ polynomial : value >> 1;
                }

                this.fcsTab[i] = (ushort)(value & 0xFFFF);
            }
        }

        public void Update(ref ushort fcs, byte value)
        {
            fcs = (ushort)((fcs >> 8) ^ this.fcsTab[(fcs ^ value) & 0xFF]);
        }
    }
}
