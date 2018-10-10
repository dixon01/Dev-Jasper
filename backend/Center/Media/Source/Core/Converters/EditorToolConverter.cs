// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditorToolConverter.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EditorToolConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Converters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Windows.Data;
    using System.Windows.Input;

    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The EditorToolConverter
    /// </summary>
    public class EditorToolConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts an icon file into a cursor stream
        /// </summary>
        /// <param name="cursorname">the name of the file</param>
        /// <param name="hotspotx">the hotspot x position</param>
        /// <param name="hotspoty">the hotspot y position</param>
        /// <returns>a stream containing a cursor</returns>
        public static Stream GetCursorFromIco(string cursorname, byte hotspotx, byte hotspoty)
        {
            var cursorStream =
                Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream("Gorba.Center.Media.Core.Resources.Cursors." + cursorname + ".ico");

            if (cursorStream != null)
            {
                var buffer = new byte[cursorStream.Length];

                cursorStream.Read(buffer, 0, (int)cursorStream.Length);

                var ms = new MemoryStream();

                buffer[2] = 2; // change to CUR file type
                buffer[10] = hotspotx;
                buffer[12] = hotspoty;

                ms.Write(buffer, 0, (int)cursorStream.Length);

                ms.Position = 0;

                return ms;
            }

            return null;
        }

        /// <summary>
        /// Converts the editor tool type to a cursor
        /// </summary>
        /// <param name="values">the values</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the result</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length > 2 && values[0] is EditorToolType && values[1] is bool && values[2] is bool)
            {
                var result = Cursors.Arrow;
                Stream cursorStream = null;

                var editorToolType = (EditorToolType)values[0];
                switch (editorToolType)
                {
                    case EditorToolType.Move:
                        cursorStream = GetCursorFromIco("cursor_32x32", 5, 5);
                        break;
                    case EditorToolType.Zoom:
                        if ((bool)values[2])
                        {
                            cursorStream = GetCursorFromIco("cursor_zoom_out_32x32", 12, 12);
                        }
                        else
                        {
                            cursorStream = GetCursorFromIco("cursor_zoom_in_32x32", 12, 12);
                        }

                        break;
                    case EditorToolType.Hand:
                        var cursorname = (bool)values[1] ? "cursor_hand__closed_32x32" : "cursor_hand_32x32";
                        cursorStream = GetCursorFromIco(cursorname, 10, 4);
                        break;
                    case EditorToolType.StaticText:
                    case EditorToolType.DynamicText:
                    case EditorToolType.RssTicker:
                        cursorStream = GetCursorFromIco("cursor_text_32x32", 2, 2);
                        break;
                    case EditorToolType.Image:
                    case EditorToolType.LiveStream:
                    case EditorToolType.Video:
                    case EditorToolType.Frame:
                    case EditorToolType.ImageList:
                    case EditorToolType.Template:
                    case EditorToolType.Rectangle:
                    case EditorToolType.AnalogClock:
                        result = Cursors.Cross;
                        break;
                    default:
                        throw new ArgumentException("unhandled tool type '" + editorToolType + "'");
                }

                if (cursorStream != null)
                {
                    result = new Cursor(cursorStream);
                }

                return result;
            }

            return Cursors.Arrow;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetTypes">the target types</param>
        /// <param name="parameter">the parameter</param>
        /// <param name="culture">the culture</param>
        /// <returns>the results</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
