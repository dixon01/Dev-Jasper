// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TextComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.BbCode;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Presenter for an <see cref="TextElement"/>.
    /// It creates an <see cref="TextItem"/>.
    /// </summary>
    public partial class TextComposer : IBbParserContext
    {
        string IBbParserContext.GetAbsolutePathRelatedToConfig(string filename)
        {
            return this.Context.Config.GetAbsolutePathRelatedToConfig(filename);
        }

        partial void Update()
        {
            var parser = new BbParser();
            var root = parser.Parse(this.HandlerValue.StringValue, this);
            var text = parser.Serialize(root);

            this.Item.SetText(text, this.HandlerValue.Animation);
        }
    }
}