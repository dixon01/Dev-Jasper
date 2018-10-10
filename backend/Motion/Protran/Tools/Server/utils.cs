using System;
using System.Windows.Forms;

public class ThreadSafeControlUpdater
{
    delegate void SetTextProc( string newText );

    Control targetControl;
    SetTextProc setText;

    public ThreadSafeControlUpdater( Control ctrl )
    {
        targetControl = ctrl;
        setText = new SetTextProc(SetText);
    }

    public string Text
    {
        set { SetText(value); }
    }

    public Exception ExceptionText
    {
        set { SetText(string.Format("{0}\r\n{1}", value.Message, value.StackTrace)); }
    }

    void SetText( string newText )
    {
        if( targetControl.InvokeRequired )
        {
            targetControl.BeginInvoke(setText, new object[]{newText});
            return;
        }

        targetControl.Text = newText;
    }
}
