namespace Microsoft.Samples.DirectX.UtilityToolkit
{
    using System.Windows.Forms;

    /// <summary>
    /// The main window that will be used for the sample framework
    /// </summary>
    public class GraphicsWindow : System.Windows.Forms.Form
    {
        private Framework frame = null;
        public GraphicsWindow(Framework f)
        {
            this.frame = f;
            this.MinimumSize = Framework.MinWindowSize;
        }

        /// <summary>
        /// Will call into the sample framework's window proc
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            this.frame.WindowsProcedure(ref m);
            base.WndProc (ref m);
        }


    }
}