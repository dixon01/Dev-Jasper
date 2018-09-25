// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteViewerTabView.xaml.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interaction logic for RemoteViewerTabView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Views
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Media;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Diag.Core.ViewModels.Unit;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Win32.Api.DLLs;
    using Gorba.Common.Utility.Win32.Api.Structs;

    using NLog;

    using RemoteViewing.Vnc;

    /// <summary>
    /// Interaction logic for RemoteViewerTabView.xaml
    /// </summary>
    public partial class RemoteViewerTabView
    {
        private const int WmClipboardUpdate = 0x031D;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private RemoteViewerSectionViewModel viewModel;

        private long lastMouseMove;

        private byte lastButtons;

        private HwndSource source;

        private string currentClipboard;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteViewerTabView"/> class.
        /// </summary>
        public RemoteViewerTabView()
        {
            this.InitializeComponent();

            this.ImageBox.MouseEnter += (s, e) =>
                {
                    Keyboard.Focus(this.ImageBox);
                    this.ImageBox.Focus();
                };
            this.ImageBox.MouseLeave += (s, e) => Keyboard.ClearFocus();

            this.ImageBox.PreviewMouseDown += this.ImageBoxOnPreviewMouseAction;
            this.ImageBox.PreviewMouseUp += this.ImageBoxOnPreviewMouseAction;
            this.ImageBox.PreviewMouseMove += this.ImageBoxOnPreviewMouseAction;
            this.ImageBox.PreviewMouseWheel += this.ImageBoxOnPreviewMouseWheel;

            this.ImageBox.KeyDown += this.ImageBoxOnKeyAction;
            this.ImageBox.PreviewKeyUp += this.ImageBoxOnKeyAction;

            this.Loaded += this.ViewOnLoaded;
            this.Unloaded += this.ViewOnUnloaded;
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Big switch statement.")]
        private static int GetKeyCode(KeyEventArgs e)
        {
            // adapted from RemoteViewing.Windows.Forms.VncKeysym.cs
            switch (e.Key)
            {
                case Key.Back: return 0xff08;
                case Key.Tab: return 0xff09;
                case Key.Enter: return 0xff0d;
                case Key.Escape: return 0xff1b;
                case Key.Insert: return 0xff63;
                case Key.Delete: return 0xffff;
                case Key.Home: return 0xff50;
                case Key.End: return 0xff57;
                case Key.PageUp: return 0xff55;
                case Key.PageDown: return 0xff56;
                case Key.Left: return 0xff51;
                case Key.Up: return 0xff52;
                case Key.Right: return 0xff53;
                case Key.Down: return 0xff54;
                case Key.F1: return 0xffbe;
                case Key.F2: return 0xffbf;
                case Key.F3: return 0xffc0;
                case Key.F4: return 0xffc1;
                case Key.F5: return 0xffc2;
                case Key.F6: return 0xffc3;
                case Key.F7: return 0xffc4;
                case Key.F8: return 0xffc5;
                case Key.F9: return 0xffc6;
                case Key.F10: return 0xffc7;
                case Key.F11: return 0xffc8;
                case Key.F12: return 0xffc9;
                case Key.F13: return 0xffca;
                case Key.F14: return 0xffcb;
                case Key.F15: return 0xffcc;
                case Key.F16: return 0xffcd;
                case Key.F17: return 0xffce;
                case Key.F18: return 0xffcf;
                case Key.F19: return 0xffd0;
                case Key.F20: return 0xffd1;
                case Key.F21: return 0xffd2;
                case Key.F22: return 0xffd3;
                case Key.F23: return 0xffd4;
                case Key.F24: return 0xffd5;
                case Key.LeftShift: return 0xffe1;
                case Key.RightShift: return 0xffe2;
                case Key.LeftCtrl: return 0xffe3;
                case Key.RightCtrl: return 0xffe4;
                case Key.LeftAlt: return 0xffe9;
                case Key.RightAlt: return 0xffea;
                case Key.OemOpenBrackets: return 0x005b;
                case Key.OemCloseBrackets: return 0x005d;
                case Key.OemBackslash: return 0x005c;
                case Key.OemComma: return 0x002c;
                case Key.OemPeriod: return 0x002e;
                case Key.OemSemicolon: return 0x003b;
                case Key.OemQuestion: return 0x002f;
                case Key.OemMinus:
                case Key.Subtract: return 0x002d;
                case Key.OemPlus:
                case Key.Add: return 0x002b;
                case Key.Multiply: return 0x002a;
                case Key.Divide: return 0x002f;
                case Key.OemQuotes: return 0x0027;
                case Key.OemTilde: return 0x0060;
                case Key.Pause: return 0xff13;
                case Key.Scroll: return 0xff14;
                case Key.PrintScreen: return 0xff15;
                case Key.NumPad0: return 0xffb0;
                case Key.NumPad1: return 0xffb1;
                case Key.NumPad2: return 0xffb2;
                case Key.NumPad3: return 0xffb3;
                case Key.NumPad4: return 0xffb4;
                case Key.NumPad5: return 0xffb5;
                case Key.NumPad6: return 0xffb6;
                case Key.NumPad7: return 0xffb7;
                case Key.NumPad8: return 0xffb8;
                case Key.NumPad9: return 0xffb9;
            }

            // TODO: This is still incomplete (e.g. alt-gr combinations are not yet working)
            return GetCharFromKey(e.Key);
        }

        private static int GetCharFromKey(Key key)
        {
            var virtualKey = KeyInterop.VirtualKeyFromKey(key);
            var keyboardState = new byte[256];
            User32.GetKeyboardState(keyboardState);

            var scanCode = User32.MapVirtualKey((uint)virtualKey, MapVirtualKeyType.VirtualKeyToScanCode);
            var stringBuilder = new StringBuilder(2);

            var result = User32.ToUnicode(
                (uint)virtualKey,
                scanCode,
                keyboardState,
                stringBuilder,
                stringBuilder.Capacity,
                0);
            switch (result)
            {
                case -1:
                case 0:
                    return -1;
                default:
                    return stringBuilder[0];
            }
        }

        private void ConnectVnc()
        {
            this.DisconnectVnc();

            this.ImageBox.Source = null;
            this.viewModel.Bitmap = null;

            this.viewModel.ConnectCommand.Execute(null);
        }

        private void DisconnectVnc()
        {
            this.ResetModifiers();

            this.viewModel.VncClient.Close();
        }

        private void ResetModifiers()
        {
            this.viewModel.IsShiftPressed = false;
            this.viewModel.IsCtrlPressed = false;
            this.viewModel.IsWinPressed = false;
            this.viewModel.IsAltPressed = false;
        }

        private void UpdateImage(VncFramebuffer framebuffer)
        {
            if (this.viewModel.Bitmap == null || this.viewModel.Bitmap.PixelWidth != framebuffer.Width
                || this.viewModel.Bitmap.PixelHeight != framebuffer.Height)
            {
                this.viewModel.Bitmap = new WriteableBitmap(
                    framebuffer.Width,
                    framebuffer.Height,
                    96,
                    96,
                    PixelFormats.Bgr32,
                    null);
                this.ImageBox.Source = this.viewModel.Bitmap;
            }

            this.viewModel.Bitmap.WritePixels(
                new Int32Rect(0, 0, framebuffer.Width, framebuffer.Height),
                framebuffer.GetBuffer(),
                framebuffer.Stride,
                0);

            this.ImageBox.InvalidateVisual();
        }

        private void HandleLocalClipboardUpdate()
        {
            try
            {
                string clipboard = null;
                if (Clipboard.ContainsText())
                {
                    clipboard = Clipboard.GetText();
                }

                if (string.IsNullOrEmpty(clipboard) || this.currentClipboard == clipboard)
                {
                    return;
                }

                this.currentClipboard = clipboard;
                this.viewModel.VncClient.SendLocalClipboardChange(clipboard);
            }
            catch (Exception ex)
            {
                Logger.WarnException("Couldn't update clipboard text", ex);
            }
        }

        private void SendPointerEvent(MouseEventArgs e, byte buttons)
        {
            if (!this.viewModel.VncClient.IsConnected || this.viewModel.Bitmap == null || !this.viewModel.IsInteractive)
            {
                return;
            }

            e.Handled = true;
            var position = e.GetPosition(this.ImageBox);
            var x = (int)(position.X / this.ImageBox.ActualWidth * this.viewModel.Bitmap.PixelWidth);
            var y = (int)(position.Y / this.ImageBox.ActualHeight * this.viewModel.Bitmap.PixelHeight);
            this.viewModel.VncClient.SendPointerEvent(x, y, buttons);
        }

        private void SendModifierKey(bool pressed, int keyCode)
        {
            if (!this.viewModel.IsInteractive)
            {
                return;
            }

            this.viewModel.VncClient.SendKeyEvent(keyCode, pressed);
        }

        private void ViewOnLoaded(object sender, RoutedEventArgs e)
        {
            this.viewModel = (RemoteViewerSectionViewModel)this.DataContext;
            this.viewModel.VncClient.FramebufferChanged += this.VncClientOnFramebufferChanged;
            this.viewModel.VncClient.RemoteClipboardChanged += this.VncClientOnRemoteClipboardChanged;
            this.viewModel.VncClient.Bell += this.VncClientOnBell;
            this.viewModel.PropertyChanged += this.ViewModelOnPropertyChanged;

            this.ImageBox.Source = this.viewModel.Bitmap;

            this.source = PresentationSource.FromVisual(this) as HwndSource;
            if (this.source == null)
            {
                return;
            }

            User32.AddClipboardFormatListener(this.source.Handle);
            this.source.AddHook(this.WndProc);
        }

        private void ViewOnUnloaded(object sender, RoutedEventArgs e)
        {
            this.viewModel.VncClient.FramebufferChanged -= this.VncClientOnFramebufferChanged;
            this.viewModel.VncClient.RemoteClipboardChanged -= this.VncClientOnRemoteClipboardChanged;
            this.viewModel.VncClient.Bell -= this.VncClientOnBell;
            this.viewModel.PropertyChanged -= this.ViewModelOnPropertyChanged;

            if (this.source == null)
            {
                return;
            }

            User32.RemoveClipboardFormatListener(this.source.Handle);
            this.source.RemoveHook(this.WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            if (msg != WmClipboardUpdate)
            {
                return IntPtr.Zero;
            }

            handled = true;
            this.HandleLocalClipboardUpdate();
            return IntPtr.Zero;
        }

        private void ConnectButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (this.ConnectButton.IsChecked.HasValue && this.ConnectButton.IsChecked.Value)
            {
                this.ConnectVnc();
            }
            else
            {
                this.DisconnectVnc();
            }
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsShiftPressed":
                    this.SendModifierKey(this.viewModel.IsShiftPressed, 0xffe1);
                    break;
                case "IsCtrlPressed":
                    this.SendModifierKey(this.viewModel.IsCtrlPressed, 0xffe3);
                    break;
                case "IsWinPressed":
                    this.SendModifierKey(this.viewModel.IsWinPressed, 0xffeb);
                    break;
                case "IsAltPressed":
                    this.SendModifierKey(this.viewModel.IsAltPressed, 0xffe9);
                    break;
            }
        }

        private void VncClientOnFramebufferChanged(object sender, FramebufferChangedEventArgs e)
        {
            if (e.RectangleCount == 0)
            {
                return;
            }

            this.ImageBox.Dispatcher.Invoke(() => this.UpdateImage(this.viewModel.VncClient.Framebuffer));
        }

        private void VncClientOnRemoteClipboardChanged(object sender, RemoteClipboardChangedEventArgs e)
        {
            if (e.Contents.Length == 0 || this.currentClipboard == e.Contents)
            {
                return;
            }

            this.Dispatcher.Invoke(
                () =>
                    {
                        try
                        {
                            this.currentClipboard = e.Contents;
                            Clipboard.SetText(e.Contents);
                        }
                        catch (Exception ex)
                        {
                            Logger.WarnException("Couldn't update local clipboard", ex);
                        }
                    });
        }

        private void VncClientOnBell(object s, EventArgs ev)
        {
            SystemSounds.Beep.Play();
        }

        private void ImageBoxOnPreviewMouseAction(object s, MouseEventArgs e)
        {
            byte buttons = 0;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                buttons |= 1;
            }

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                buttons |= 2;
            }

            if (e.RightButton == MouseButtonState.Pressed)
            {
                buttons |= 4;
            }

            var nowTicks = TimeProvider.Current.TickCount;

            // don't send if there is no MouseClick and an event was triggered less than 200 ms before.
            if (buttons == 0 && this.lastButtons == 0 && nowTicks - this.lastMouseMove < 200)
            {
                return;
            }

            this.lastButtons = buttons;
            this.lastMouseMove = nowTicks;
            this.SendPointerEvent(e, buttons);
        }

        private void ImageBoxOnPreviewMouseWheel(object s, MouseWheelEventArgs e)
        {
            byte buttons;
            if (e.Delta > 0)
            {
                // wheel moved up
                buttons = 8;
            }
            else if (e.Delta < 0)
            {
                // wheel moved down
                buttons = 16;
            }
            else
            {
                return;
            }

            this.SendPointerEvent(e, buttons);
        }

        private void ImageBoxOnKeyAction(object sender, KeyEventArgs e)
        {
            if (this.viewModel.Bitmap == null || !this.viewModel.IsInteractive)
            {
                return;
            }

            e.Handled = true;

            var key = GetKeyCode(e);
            if (key < 0)
            {
                return;
            }

            this.viewModel.VncClient.SendKeyEvent(key, e.IsDown);

            if (!e.IsDown)
            {
                this.ResetModifiers();
            }
        }
    }
}
