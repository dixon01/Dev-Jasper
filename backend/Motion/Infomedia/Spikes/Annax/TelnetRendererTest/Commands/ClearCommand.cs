namespace Gorba.Motion.Infomedia.AnnaxRendererTest.Commands
{
    using System.Text;

    public class ClearCommand : CommandBase
    {
        public override void AppendCommandString(StringBuilder buffer)
        {
            buffer.Append("clear\n");
        }
    }
}