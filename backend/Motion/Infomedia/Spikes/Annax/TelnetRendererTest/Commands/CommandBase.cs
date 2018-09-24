namespace Gorba.Motion.Infomedia.AnnaxRendererTest.Commands
{
    using System.Text;

    public abstract class CommandBase
    {
        public abstract void AppendCommandString(StringBuilder buffer);
    }
}