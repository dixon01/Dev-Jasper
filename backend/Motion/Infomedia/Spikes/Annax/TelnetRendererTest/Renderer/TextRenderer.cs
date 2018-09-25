namespace Gorba.Motion.Infomedia.AnnaxRendererTest.Renderer
{
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.AnnaxRendererTest.Commands;
    using Gorba.Motion.Infomedia.Entities;
    using Gorba.Motion.Infomedia.Entities.Screen;
    using Gorba.Motion.Infomedia.RendererBase.Text;

    public class TextRenderer : RendererBase
    {
        private List<FormattedText<SimplePartBase>> alternatives;

        private int bitmapNumber;

        public TextRenderer(TextItem text)
            : base(text)
        {
            var factory = new SimpleTextFactory();
            this.alternatives = factory.ParseAlternatives(text.Text, text.Font);
        }

        public override IEnumerable<CommandBase> Setup(IRenderContext context)
        {
            this.bitmapNumber = context.GetAvailableBitmapNumber();
            return this.Render(context);
        }

        public override IEnumerable<CommandBase> Update(ItemUpdate update, IRenderContext context)
        {
            if (update.ScreenItemId != this.Item.Id)
            {
                yield break;
            }

            this.Item.Update(update);
            var factory = new SimpleTextFactory();
            var text = (TextItem)this.Item;
            this.alternatives = factory.ParseAlternatives(text.Text, text.Font);
            foreach (var command in this.Render(context))
            {
                yield return command;
            }
        }

        private IEnumerable<CommandBase> Render(IRenderContext context)
        {
            var item = (TextItem)this.Item;
            bool blink;
            var text = this.GetText(out blink);
            if (string.IsNullOrEmpty(text))
            {
                yield break;
            }

            TextCommand.Align align;
            switch (item.Align)
            {
                case HorizontalAlignment.Left:
                    align = TextCommand.Align.Left;
                    break;
                case HorizontalAlignment.Center:
                    align = TextCommand.Align.Center;
                    break;
                case HorizontalAlignment.Right:
                    align = TextCommand.Align.Right;
                    break;
                default:
                    align = TextCommand.Align.Left;
                    break;
            }

            var textCommand = new TextCommand(this.bitmapNumber, this.GetFont(), text, align);
            yield return textCommand;

            // TODO: set other properties
            var window = new WindowCommand(
                this.bitmapNumber,
                this.Item.X,
                this.Item.Y,
                this.Item.Width,
                this.Item.Height);

            window.Display = item.Visible ? WindowCommand.DisplayAttr.Normal : WindowCommand.DisplayAttr.Blank;

            window.Duration = WindowCommand.DurationAttr.Endless;

            switch (item.Overflow)
            {
                case TextOverflow.Clip:
                    window.Base = WindowCommand.BaseAttr.NoAction;
                    break;
                case TextOverflow.Scroll:
                    window.Base = WindowCommand.BaseAttr.AutoScroll;
                    break;
                case TextOverflow.ScrollAlways:
                    window.Base = item.ScrollSpeed < 0
                                      ? WindowCommand.BaseAttr.ScrollLeft
                                      : WindowCommand.BaseAttr.ScrollRight;
                    break;
                default:
                    // currently not supported: Scale, Wrap, Overflow
                    window.Base = WindowCommand.BaseAttr.NoAction;
                    break;
            }

            if (window.Base != WindowCommand.BaseAttr.NoAction)
            {
                // TODO: figure out the right values
                // TODO: take scroll speed into account!!!
                window.Counting = 50;
                window.Timing = 3;
            }

            if (blink)
            {
                // TODO: figure out the right values
                window.Base |= WindowCommand.BaseAttr.BlinkNormalBlank
                    /*| WindowCommand.BaseAttr.Synchronize*/;
                window.Counting = 2;
                window.Timing = 50;
            }

            yield return window;
        }

        private AnnaxFont GetFont()
        {
            return new AnnaxFont(-8);
        }

        private string GetText(out bool blink)
        {
            blink = false;

            if (this.alternatives.Count == 0)
            {
                return string.Empty;
            }

            var buffer = new StringBuilder();
            foreach (var part in this.alternatives[0].Parts)
            {
                var text = part as SimpleTextPart;
                if (text == null)
                {
                    continue;
                }

                blink |= part.Blink;
                buffer.Append(text.Text);
            }

            return buffer.ToString();
        }
    }
}