// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageListComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageListComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Presenter for an <see cref="ImageListElement"/>.
    /// It creates an <see cref="ImageListItem"/>.
    /// </summary>
    public partial class ImageListComposer
    {
        private string[] filePatterns;

        private static bool AreEqual(string[] left, string[] right)
        {
            if (left == right)
            {
                return true;
            }

            if (left == null || right == null)
            {
                return false;
            }

            if (left.Length != right.Length)
            {
                return false;
            }

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    return false;
                }
            }

            return true;
        }

        partial void Initialize()
        {
            this.filePatterns = this.Element.FilePatterns.Split(';');

            if (!string.IsNullOrEmpty(this.Element.FallbackImage))
            {
                this.Item.FallbackImage =
                    this.Context.Config.GetAbsolutePathRelatedToConfig(this.Element.FallbackImage);
            }
        }

        partial void Update()
        {
            if (string.IsNullOrEmpty(this.HandlerValues.StringValue))
            {
                this.SetImages(new string[0]);
                return;
            }

            var imageNames = ArrayUtil.SplitString(this.HandlerValues.StringValue, this.Element.Delimiter);
            this.SetImages(ArrayUtil.ConvertAll<string, string>(imageNames, this.FindImage));
        }

        private void SetImages(string[] images)
        {
            // this comparison reduces the number of times we update the image list if actually nothing has changed
            if (!AreEqual(this.Item.Images, images))
            {
                this.Item.Images = images;
            }
        }

        private string FindImage(string image)
        {
            foreach (var pattern in this.filePatterns)
            {
                var fileName = string.Format(pattern, image);
                fileName = this.Context.Config.GetAbsolutePathRelatedToConfig(fileName);
                if (File.Exists(fileName))
                {
                    return fileName;
                }
            }

            return image;
        }
    }
}