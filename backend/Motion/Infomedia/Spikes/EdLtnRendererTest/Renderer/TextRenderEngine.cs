namespace Gorba.Motion.Infomedia.EdLtnRendererTest.Renderer
{
    using System;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.BbCode;
    using Gorba.Motion.Infomedia.EdLtnRendererTest.Protocol;
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Engine;
    using Gorba.Motion.Infomedia.RendererBase.Manager;

    public class TextRenderEngine
        : RenderEngineBase<TextItem, ITextRenderEngine<ILtnRenderContext>, TextRenderManager<ILtnRenderContext>>,
          ITextRenderEngine<ILtnRenderContext>
    {
        private byte cellNumber;

        private string lastValue;

        public TextRenderEngine(TextRenderManager<ILtnRenderContext> manager)
            : base(manager)
        {
        }

        public override void Render(double alpha, ILtnRenderContext context)
        {
            if (this.Manager.Text.OldValue != null)
            {
                this.Render(this.Manager.Text.OldValue, context);
            }
            else if (this.Manager.Text.NewValue != null)
            {
                this.Render(this.Manager.Text.NewValue, context);
            }
        }

        private void Render(string value, ILtnRenderContext context)
        {
            if (this.lastValue == value)
            {
                return;
            }

            if (context.State == RenderState.Setup)
            {
                this.CreateCell(context);
                return;
            }

            this.lastValue = value;

            var root = new BbParser().Parse(value);
            var telegram = new SetText(root.ToPlainString()) { CellNumber = this.cellNumber };

            byte fontNumber;
            if (byte.TryParse(this.Manager.Font.Face, out fontNumber))
            {
                telegram.FontNumber = fontNumber;
            }

            if (root.FindNodesOfType<Blink>().GetEnumerator().MoveNext())
            {
                telegram.Mode = TextMode.Blink;
            }
            else if (this.Manager.Overflow == TextOverflow.ScrollAlways)
            {
                telegram.Mode = this.Manager.ScrollSpeed < 0 ? TextMode.ScrollLeftBlank : TextMode.ScrollRightBlank;
            }

            this.SerialPort.EnqueueTelegram(telegram);
        }

        private void CreateCell(ILtnRenderContext context)
        {
            this.cellNumber = context.GetNextCellNumber();
            var telegram = new SetCell
                               {
                                   CellNumber = this.cellNumber,
                                   X = (byte)this.Manager.X,
                                   Y = (byte)this.Manager.Y,
                                   Width = (byte)this.Manager.Width,
                                   Height = (byte)this.Manager.Height
                               };

            switch (this.Manager.Align)
            {
                case HorizontalAlignment.Left:
                    telegram.Align = CellAlignment.Left;
                    break;
                case HorizontalAlignment.Center:
                    telegram.Align = CellAlignment.Center;
                    break;
                case HorizontalAlignment.Right:
                    telegram.Align = CellAlignment.Right;
                    break;
            }

            this.SerialPort.EnqueueTelegram(telegram);
        }
    }
}
