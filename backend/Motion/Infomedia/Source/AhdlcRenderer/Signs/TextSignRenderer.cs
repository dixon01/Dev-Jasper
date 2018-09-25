// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextSignRenderer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextSignRenderer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Signs
{
    using System.Collections.Generic;
    using System.Text;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Protocols.Ahdlc.Providers;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Engines;
    using Gorba.Motion.Infomedia.AhdlcRenderer.Parts;

    /// <summary>
    /// A sign renderer that only sends text content to the sign.
    /// </summary>
    public class TextSignRenderer : SignRendererBase
    {
        /// <summary>
        /// Renders the given components onto the sign.
        /// </summary>
        /// <param name="components">
        /// The components to render.
        /// </param>
        /// <param name="context">
        /// The rendering context.
        /// </param>
        /// <returns>
        /// The <see cref="IFrameProvider"/> that provides all frames to show the contents of the given components.
        /// </returns>
        public override IFrameProvider Render(ICollection<ComponentBase> components, IAhdlcRenderContext context)
        {
            var scrolling = false;
            var scrollAlways = false;

            var builder = new StringBuilder();
            foreach (var item in components)
            {
                var text = item as TextComponent;
                if (text == null)
                {
                    this.Logger.Warn("{0} is not supported by text sign rendering", item.GetType().Name);
                    continue;
                }

                if (text.Overflow == TextOverflow.Scroll)
                {
                    scrolling = true;
                }
                else if (text.Overflow == TextOverflow.ScrollAlways)
                {
                    scrolling = true;
                    scrollAlways = true;
                }

                builder.Append(text.Text);
            }

            var texts = this.CreateTexts(builder.ToString());

            if (!scrolling)
            {
                return new StaticTextFrameProvider(0, 0, texts);
            }

            if (scrollAlways)
            {
                return new ScrollTextFrameProvider(0, 0, texts);
            }

            return new AutoTextFrameProvider(0, 0, texts);
        }

        private string[] CreateTexts(string text)
        {
            var textFactory = new TextPartFactory();
            var alternatives = textFactory.ParseAlternatives(text, new Font());

            string[] texts;
            if (alternatives.Count > 3)
            {
                this.Logger.Warn(
                    "Text contains more than three alternatives, only showing the first three: {0}", text);
                texts = new string[3];
            }
            else
            {
                texts = new string[alternatives.Count];
            }

            var builder = new StringBuilder();
            for (int i = 0; i < texts.Length; i++)
            {
                builder.Length = 0;
                if (alternatives[i].Lines.Count > 1)
                {
                    this.Logger.Warn("[br] is not supported by text sign rendering");
                }

                foreach (var line in alternatives[i].Lines)
                {
                    foreach (var part in line.Parts)
                    {
                        var textPart = part as TextPart;
                        if (textPart == null)
                        {
                            this.Logger.Warn("{0} is not supported by text sign rendering", part.GetType().Name);
                            continue;
                        }

                        builder.Append(textPart.Text);
                    }
                }

                texts[i] = builder.ToString();
            }

            return texts;
        }
    }
}