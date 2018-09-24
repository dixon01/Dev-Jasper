namespace Gorba.Motion.Infomedia.SharpDXRendererTest.DxExtensions
{
    public static class SharpDxExtensions
    {
        public static SharpDX.Rectangle ToSharpDx(this System.Drawing.Rectangle rectangle)
        {
            return new SharpDX.Rectangle(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        }
    }
}

namespace System.Runtime.CompilerServices
{
    internal class ExtensionAttribute : Attribute
    {
    }
}
