using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.GUIS;

namespace Server
{
    public class Form1 : System.Windows.Forms.Form
    {
        #region DELEGATES
        delegate void UpdateClientCountProc(int howMany);
        #endregion DELEGATES

        private bool started = false;
        private bool reverseReply = false;
        private bool shuttingDown = false;
        private Socket serverSocket = null;
        private AsyncCallback acceptFunction;
        private AsyncCallback receiveFunction;
        private ArrayList clientsList = ArrayList.Synchronized(new ArrayList());

        private ThreadSafeControlUpdater lastExceptionUpdater;
        private ThreadSafeControlUpdater requestTextUpdater;
        private ThreadSafeControlUpdater replyTextUpdater;
        private UpdateClientCountProc updateConnectCountProc;

        private Label label1;
        private TextBox portNum;
        private Label label2;
        private Label clientCount;
        private TextBox lastException;
        private Button clearException;
        private GroupBox groupBox3;
        private Label label7;
        private GroupBox groupBox2;
        private Label label3;
        private GroupBox groupBox1;
        private Button startStop;
        private CheckBox reverseReplyCheckBox;
        private Button button_clearReqeuestReply;
        private CheckBox checkBox_autoReply;
        private GroupBox groupBox4;
        private Button button_sendData;
        private TextBox textBox_sendData;
        private TextBox replyText;
        private TextBox requestText;
        private Button button_startStopCycle;
        private Label label_msCycle;
        private NumericUpDown numericUpDown_period;
        private CheckBox checkBox_cycle;

        private int cyclePeriod;
        private bool endedCycle;
        private bool toStopCycle;
        private Thread threadCycle;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem genericViewToolStripMenuItem;
        private SplitContainer splitContainer1;
        private Label label_howMany;
        private NumericUpDown numericUpDown_howMany;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public Form1()
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            acceptFunction = new AsyncCallback(OnAccept);
            receiveFunction = new AsyncCallback(OnReceive);

            requestTextUpdater = new ThreadSafeControlUpdater(requestText);
            replyTextUpdater = new ThreadSafeControlUpdater(replyText);
            lastExceptionUpdater = new ThreadSafeControlUpdater(lastException);
            updateConnectCountProc = new UpdateClientCountProc(UpdateClientCount);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.checkBox_cycle.Checked = false;
            this.checkBox_cycle_CheckedChanged(this, null);

            this.threadCycle = null;
            this.toStopCycle = false;
            this.endedCycle = true;
            this.cyclePeriod = 10;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.portNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.clientCount = new System.Windows.Forms.Label();
            this.lastException = new System.Windows.Forms.TextBox();
            this.clearException = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.replyText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.requestText = new System.Windows.Forms.TextBox();
            this.checkBox_autoReply = new System.Windows.Forms.CheckBox();
            this.button_clearReqeuestReply = new System.Windows.Forms.Button();
            this.reverseReplyCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.startStop = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label_howMany = new System.Windows.Forms.Label();
            this.numericUpDown_howMany = new System.Windows.Forms.NumericUpDown();
            this.button_startStopCycle = new System.Windows.Forms.Button();
            this.label_msCycle = new System.Windows.Forms.Label();
            this.numericUpDown_period = new System.Windows.Forms.NumericUpDown();
            this.checkBox_cycle = new System.Windows.Forms.CheckBox();
            this.button_sendData = new System.Windows.Forms.Button();
            this.textBox_sendData = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.genericViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_howMany)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_period)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Port:";
            // 
            // portNum
            // 
            this.portNum.Location = new System.Drawing.Point(54, 19);
            this.portNum.Name = "portNum";
            this.portNum.Size = new System.Drawing.Size(56, 20);
            this.portNum.TabIndex = 2;
            this.portNum.Text = "1598";
            this.portNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(128, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "Active connections:";
            // 
            // clientCount
            // 
            this.clientCount.Location = new System.Drawing.Point(240, 22);
            this.clientCount.Name = "clientCount";
            this.clientCount.Size = new System.Drawing.Size(48, 23);
            this.clientCount.TabIndex = 4;
            this.clientCount.Text = "0";
            // 
            // lastException
            // 
            this.lastException.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lastException.Location = new System.Drawing.Point(89, 19);
            this.lastException.Multiline = true;
            this.lastException.Name = "lastException";
            this.lastException.ReadOnly = true;
            this.lastException.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.lastException.Size = new System.Drawing.Size(442, 71);
            this.lastException.TabIndex = 13;
            // 
            // clearException
            // 
            this.clearException.Location = new System.Drawing.Point(6, 19);
            this.clearException.Name = "clearException";
            this.clearException.Size = new System.Drawing.Size(41, 23);
            this.clearException.TabIndex = 14;
            this.clearException.Text = "Clear";
            this.clearException.Click += new System.EventHandler(this.clearException_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.clearException);
            this.groupBox3.Controls.Add(this.lastException);
            this.groupBox3.Location = new System.Drawing.Point(8, 555);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(536, 96);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Most Recent Exception";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 155);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 23);
            this.label7.TabIndex = 10;
            this.label7.Text = "Last reply:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.replyText);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.requestText);
            this.groupBox2.Controls.Add(this.checkBox_autoReply);
            this.groupBox2.Controls.Add(this.button_clearReqeuestReply);
            this.groupBox2.Controls.Add(this.reverseReplyCheckBox);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(0, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(539, 222);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Send/Receive";
            // 
            // replyText
            // 
            this.replyText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replyText.BackColor = System.Drawing.Color.White;
            this.replyText.Location = new System.Drawing.Point(89, 152);
            this.replyText.Multiline = true;
            this.replyText.Name = "replyText";
            this.replyText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.replyText.Size = new System.Drawing.Size(444, 60);
            this.replyText.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 23);
            this.label3.TabIndex = 8;
            this.label3.Text = "Last request:";
            // 
            // requestText
            // 
            this.requestText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.requestText.BackColor = System.Drawing.Color.White;
            this.requestText.Location = new System.Drawing.Point(89, 47);
            this.requestText.Multiline = true;
            this.requestText.Name = "requestText";
            this.requestText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.requestText.Size = new System.Drawing.Size(444, 99);
            this.requestText.TabIndex = 17;
            // 
            // checkBox_autoReply
            // 
            this.checkBox_autoReply.AutoSize = true;
            this.checkBox_autoReply.Location = new System.Drawing.Point(11, 24);
            this.checkBox_autoReply.Name = "checkBox_autoReply";
            this.checkBox_autoReply.Size = new System.Drawing.Size(72, 17);
            this.checkBox_autoReply.TabIndex = 16;
            this.checkBox_autoReply.Text = "auto reply";
            this.checkBox_autoReply.UseVisualStyleBackColor = true;
            // 
            // button_clearReqeuestReply
            // 
            this.button_clearReqeuestReply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button_clearReqeuestReply.Location = new System.Drawing.Point(243, 20);
            this.button_clearReqeuestReply.Name = "button_clearReqeuestReply";
            this.button_clearReqeuestReply.Size = new System.Drawing.Size(0, 23);
            this.button_clearReqeuestReply.TabIndex = 15;
            this.button_clearReqeuestReply.Text = "Clear";
            this.button_clearReqeuestReply.Click += new System.EventHandler(this.button_clearReqeuestReply_Click);
            // 
            // reverseReplyCheckBox
            // 
            this.reverseReplyCheckBox.Location = new System.Drawing.Point(89, 20);
            this.reverseReplyCheckBox.Name = "reverseReplyCheckBox";
            this.reverseReplyCheckBox.Size = new System.Drawing.Size(160, 24);
            this.reverseReplyCheckBox.TabIndex = 7;
            this.reverseReplyCheckBox.Text = "Reverse text when replying";
            this.reverseReplyCheckBox.Click += new System.EventHandler(this.reverseReplyCheckBox_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.startStop);
            this.groupBox1.Controls.Add(this.portNum);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.clientCount);
            this.groupBox1.Location = new System.Drawing.Point(8, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(535, 61);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Connections";
            // 
            // startStop
            // 
            this.startStop.Location = new System.Drawing.Point(258, 17);
            this.startStop.Name = "startStop";
            this.startStop.Size = new System.Drawing.Size(41, 23);
            this.startStop.TabIndex = 5;
            this.startStop.Text = "Start";
            this.startStop.Click += new System.EventHandler(this.startStop_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label_howMany);
            this.groupBox4.Controls.Add(this.numericUpDown_howMany);
            this.groupBox4.Controls.Add(this.button_startStopCycle);
            this.groupBox4.Controls.Add(this.label_msCycle);
            this.groupBox4.Controls.Add(this.numericUpDown_period);
            this.groupBox4.Controls.Add(this.checkBox_cycle);
            this.groupBox4.Controls.Add(this.button_sendData);
            this.groupBox4.Controls.Add(this.textBox_sendData);
            this.groupBox4.Location = new System.Drawing.Point(0, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(535, 228);
            this.groupBox4.TabIndex = 15;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Send";
            // 
            // label_howMany
            // 
            this.label_howMany.AutoSize = true;
            this.label_howMany.Location = new System.Drawing.Point(8, 93);
            this.label_howMany.Name = "label_howMany";
            this.label_howMany.Size = new System.Drawing.Size(55, 13);
            this.label_howMany.TabIndex = 7;
            this.label_howMany.Text = "how many";
            // 
            // numericUpDown_howMany
            // 
            this.numericUpDown_howMany.Location = new System.Drawing.Point(6, 109);
            this.numericUpDown_howMany.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_howMany.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numericUpDown_howMany.Name = "numericUpDown_howMany";
            this.numericUpDown_howMany.Size = new System.Drawing.Size(59, 20);
            this.numericUpDown_howMany.TabIndex = 6;
            this.numericUpDown_howMany.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_howMany.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // button_startStopCycle
            // 
            this.button_startStopCycle.Location = new System.Drawing.Point(6, 135);
            this.button_startStopCycle.Name = "button_startStopCycle";
            this.button_startStopCycle.Size = new System.Drawing.Size(70, 23);
            this.button_startStopCycle.TabIndex = 5;
            this.button_startStopCycle.Text = "Start";
            this.button_startStopCycle.UseVisualStyleBackColor = true;
            this.button_startStopCycle.Click += new System.EventHandler(this.button_startStopCycle_Click);
            // 
            // label_msCycle
            // 
            this.label_msCycle.AutoSize = true;
            this.label_msCycle.Location = new System.Drawing.Point(65, 74);
            this.label_msCycle.Name = "label_msCycle";
            this.label_msCycle.Size = new System.Drawing.Size(20, 13);
            this.label_msCycle.TabIndex = 4;
            this.label_msCycle.Text = "ms";
            // 
            // numericUpDown_period
            // 
            this.numericUpDown_period.Location = new System.Drawing.Point(6, 71);
            this.numericUpDown_period.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown_period.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown_period.Name = "numericUpDown_period";
            this.numericUpDown_period.Size = new System.Drawing.Size(59, 20);
            this.numericUpDown_period.TabIndex = 3;
            this.numericUpDown_period.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown_period.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // checkBox_cycle
            // 
            this.checkBox_cycle.AutoSize = true;
            this.checkBox_cycle.Location = new System.Drawing.Point(6, 48);
            this.checkBox_cycle.Name = "checkBox_cycle";
            this.checkBox_cycle.Size = new System.Drawing.Size(51, 17);
            this.checkBox_cycle.TabIndex = 2;
            this.checkBox_cycle.Text = "cycle";
            this.checkBox_cycle.UseVisualStyleBackColor = true;
            this.checkBox_cycle.CheckedChanged += new System.EventHandler(this.checkBox_cycle_CheckedChanged);
            // 
            // button_sendData
            // 
            this.button_sendData.Location = new System.Drawing.Point(6, 19);
            this.button_sendData.Name = "button_sendData";
            this.button_sendData.Size = new System.Drawing.Size(41, 23);
            this.button_sendData.TabIndex = 1;
            this.button_sendData.Text = "Send";
            this.button_sendData.UseVisualStyleBackColor = true;
            this.button_sendData.Click += new System.EventHandler(this.button_sendData_Click);
            // 
            // textBox_sendData
            // 
            this.textBox_sendData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_sendData.Location = new System.Drawing.Point(86, 15);
            this.textBox_sendData.MaxLength = 500000;
            this.textBox_sendData.Multiline = true;
            this.textBox_sendData.Name = "textBox_sendData";
            this.textBox_sendData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_sendData.Size = new System.Drawing.Size(447, 207);
            this.textBox_sendData.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(547, 24);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.genericViewToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // genericViewToolStripMenuItem
            // 
            this.genericViewToolStripMenuItem.Name = "genericViewToolStripMenuItem";
            this.genericViewToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.genericViewToolStripMenuItem.Text = "Generic View";
            this.genericViewToolStripMenuItem.Click += new System.EventHandler(this.genericViewToolStripMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(8, 94);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox4);
            this.splitContainer1.Size = new System.Drawing.Size(535, 455);
            this.splitContainer1.SplitterDistance = 217;
            this.splitContainer1.TabIndex = 17;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(547, 663);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Server";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_howMany)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_period)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
        }

        private void startStop_Click(object sender, System.EventArgs e)
        {
            if (!started)
            {
                this.StartServer();
            }
            else
            {
                this.CloseServer();
            }

            started = !started;
            if (started)
            {
                startStop.Text = "Stop";
                portNum.Enabled = false;
            }
            else
            {
                startStop.Text = "Start";
                portNum.Enabled = true;
            }
        }

        private void StartServer()
        {
            clientCount.Text = "0";
            // Start things up...
            this.clientsList = ArrayList.Synchronized(new ArrayList());
            IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Any, int.Parse(portNum.Text));
            shuttingDown = false;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipEndpoint);
            serverSocket.Listen(5);
            serverSocket.BeginAccept(acceptFunction, null);
            this.serverSocket.LingerState.Enabled = false;
        }

        private void CloseServer()
        {
            // Shut things down (work on a clone of clientSockets
            // because each socket shutdown is going to trigger
            // a call to OnReceive, which will remove the client
            // from the connection list).
            ArrayList temp = (ArrayList)clientsList.Clone();
            foreach (ClientInfo ci in temp)
            {
                DropClient(ci.socket);
            }

            // This will trigger OnAccept.
            shuttingDown = true;
            serverSocket.Close(3);
            this.serverSocket = null;
            this.clientsList.Clear();
        }

        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                if (shuttingDown)
                {
                    // The "stop" button was pressed, which caused the app to close
                    // this socket and reset itself to be started again.
                    return;
                }
                // else...

                Socket clientSocket = serverSocket.EndAccept(ar);

                if (!clientSocket.Connected)
                {
                    return;
                }
                // else...

                ClientInfo ci = new ClientInfo(clientSocket);

                // Register the new client.
                clientsList.Add(ci);

                // Issue an async read just so that we can detect
                // a disconnect w/o polling.
                try
                {
                    clientSocket.BeginReceive(ci.dataBuf, 0, ci.dataBuf.Length,
                                               SocketFlags.None, receiveFunction, ci);
                    // Update the UI.
                    UpdateClientCount(1);
                }
                catch (Exception e)
                {
                    lastExceptionUpdater.ExceptionText = e;
                }

                // Get ready for next incoming connection.
                serverSocket.BeginAccept(acceptFunction, null);
            }
            catch (Exception e)
            {
                lastExceptionUpdater.ExceptionText = e;
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            ClientInfo ci = (ClientInfo)ar.AsyncState;

            try
            {
                // Since the client won't actually be sending data, the
                // following call to EndReceive should either throw an exception
                // or return 0.
                int cbReceived = ci.socket.EndReceive(ar);

                if (cbReceived > 0)
                {
                    // If we didn't fail, echo the data back and
                    // then queue up another receive buffer.
                    try
                    {
                        // Extract string, possibly reversing it.
                        string replyMsg = Encoding.ASCII.GetString(ci.dataBuf, 0, cbReceived);
                        char[] replyChars = replyMsg.ToCharArray();

                        if (reverseReply)
                        {
                            Array.Reverse(replyChars);
                        }

                        if (this.checkBox_autoReply.Checked)
                        {
                            // Send reply.
                            byte[] replyBuf = Encoding.ASCII.GetBytes(replyChars);
                            ci.socket.BeginSend(replyBuf, 0, replyBuf.Length, SocketFlags.None, null, null);

                            Invoke((MethodInvoker)delegate()
                            {
                                this.replyText.Text = Encoding.UTF8.GetString(replyBuf);
                            });
                        }

                        Invoke((MethodInvoker)delegate()
                        {
                            this.requestText.Text = Encoding.UTF8.GetString(ci.dataBuf);
                        });

                        // Queue up for another request.
                        ci.socket.BeginReceive(ci.dataBuf, 0, ci.dataBuf.Length, SocketFlags.None, receiveFunction, ci);
                    }
                    catch (Exception e)
                    {
                        DropClient(ci.socket);
                        lastExceptionUpdater.ExceptionText = e;
                    }
                }
                else
                {
                    DropClient(ci.socket);
                }
            }
            catch (Exception e)
            {
                DropClient(ci.socket);
                lastExceptionUpdater.ExceptionText = e;
            }
        }

        private void DropClient(Socket s)
        {
            try
            {
                // s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
            catch { }

            bool removed = false;
            lock (clientsList.SyncRoot)
            {
                try
                {
                    // Find and remove the associated ClientInfo object from
                    // our list.
                    for (int n = 0; !removed && (n < clientsList.Count); n++)
                    {
                        ClientInfo ci = (ClientInfo)clientsList[n];
                        if (ci.socket == s)
                        {
                            clientsList.Remove(ci);
                            removed = true;
                        }
                    }
                }
                catch (Exception e) { lastExceptionUpdater.ExceptionText = e; }
            }

            if (removed)
            {
                UpdateClientCount(-1);
            }
        }

        private void UpdateClientCount(int howMany)
        {
            if (clientCount.InvokeRequired)
            {
                clientCount.BeginInvoke(updateConnectCountProc, new object[] { howMany });
                return;
            }

            clientCount.Text = (int.Parse(clientCount.Text) + howMany).ToString();
        }

        private void clearException_Click(object sender, System.EventArgs e)
        {
            lastException.Text = "";
        }

        private void reverseReplyCheckBox_Click(object sender, System.EventArgs e)
        {
            reverseReply = reverseReplyCheckBox.Checked;
        }

        private void button_clearReqeuestReply_Click(object sender, EventArgs e)
        {
            this.requestText.Text = "";
            this.replyText.Text = "";
        }

        private void button_sendData_Click(object sender, EventArgs e)
        {
            this.Send(this.textBox_sendData.Text);
        }

        private void Send(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            // else...

            byte[] replyBuf = Encoding.UTF8.GetBytes(text);
            for (int n = 0; n < clientsList.Count; n++)
            {
                ClientInfo ci = (ClientInfo)clientsList[n];
                try
                {
                    ci.socket.BeginSend(replyBuf, 0, replyBuf.Length, SocketFlags.None, null, null);
                }
                catch (Exception) { }
            }
        }

        private void checkBox_cycle_CheckedChanged(object sender, EventArgs e)
        {
            this.numericUpDown_period.Visible =
            this.label_msCycle.Visible =
            this.numericUpDown_howMany.Visible =
            this.label_howMany.Visible =
            this.button_startStopCycle.Visible = this.checkBox_cycle.Checked;
        }

        private void button_startStopCycle_Click(object sender, EventArgs e)
        {
            if (!this.endedCycle)
            {
                // the user wants to stop the cycle.
                this.checkBox_cycle.Enabled =
                this.numericUpDown_period.Enabled =
                this.button_sendData.Enabled =
                this.textBox_sendData.Enabled = true;
                this.StopCycle();
            }
            else
            {
                // the user wants to start the cycle.
                this.checkBox_cycle.Enabled =
                this.numericUpDown_period.Enabled =
                this.button_sendData.Enabled =
                this.textBox_sendData.Enabled = false;
                this.StartCycle();
            }
        }

        private void StartCycle()
        {
            this.button_startStopCycle.Text = "Stop";
            this.toStopCycle = false;
            this.endedCycle = false;
            this.threadCycle = new Thread(new ThreadStart(SendCycle));
            threadCycle.Name = "Th_Cycle";
            this.cyclePeriod = (int)this.numericUpDown_period.Value;
            threadCycle.Start();
        }

        private void StopCycle()
        {
            this.button_startStopCycle.Text = "Start";
            if (this.threadCycle != null)
            {
                this.toStopCycle = true;
                while (!this.endedCycle)
                {
                    Thread.Sleep(10);
                }
                Thread.Sleep(100);
                this.threadCycle = null;
            }
        }

        private void SendCycle()
        {
            int counter = 0;
            string text = this.textBox_sendData.Text;
            int howMany = (int)this.numericUpDown_howMany.Value;
            string[] frames = text.Split(new string[] { "***" }, StringSplitOptions.RemoveEmptyEntries);
            bool checkHowMany = (howMany != -1);
            while (!this.toStopCycle)
            {
                Thread.Sleep(this.cyclePeriod);
                if (frames != null && frames.Length > 0)
                {
                    for (int i = 0; i < frames.Length; i++)
                    {
                        string frame = frames[i];
                        this.Send(frame);
                        Thread.Sleep(this.cyclePeriod);
                    }
                }

                counter++;
                if (checkHowMany && counter == howMany)
                {
                    break;
                }
            }
            this.endedCycle = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CloseServer();
            Application.Exit();
        }

        private void genericViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenViewEditorUI genViewUI = new GenViewEditorUI();
            genViewUI.XimpleSendAllarmer += new GenViewEditorUI.XimpleSendHandler(OnGenViewXimpleToSend);
            genViewUI.Show();
        }

        private void OnGenViewXimpleToSend(string xml)
        {
            this.Send(xml);
        }
    }

    /// <summary>
    /// An instance of this class is allocated for each client
    /// connection accepted.
    /// </summary>
    class ClientInfo
    {
        #region VARIABLES
        public byte[] dataBuf;
        public Socket socket;
        #endregion VARIABLES

        public ClientInfo(Socket socket)
        {
            this.socket = socket;
            this.dataBuf = new byte[4096];
        }
    }
}
