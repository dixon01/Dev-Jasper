namespace Gorba.Motion.Obc.Terminal.Gui.MainFields
{
    using Gorba.Motion.Obc.Terminal.Gui.Control;

  partial class NumberInput
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NumberInput));
          this.lblCaption = new System.Windows.Forms.Label();
          this.lblMainCaption = new System.Windows.Forms.Label();
          this.timerStartValidation = new System.Windows.Forms.Timer();
          this.btnEsc = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btnEnglish = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btnFrench = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btnGerman = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btnEnter = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btnBackspace = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn0 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btnClear = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn9 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn8 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn7 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn4 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn5 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn6 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn3 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn2 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.btn1 = new Gorba.Motion.Obc.Terminal.Gui.Control.VCustomButton();
          this.txtInputNumber = new Gorba.Motion.Obc.Terminal.Gui.Control.CustomTextbox();
          this.lblHint = new System.Windows.Forms.Label();
          this.txtInputNumber2 = new Gorba.Motion.Obc.Terminal.Gui.Control.CustomTextbox();
          this.SuspendLayout();
          // 
          // lblCaption
          // 
          resources.ApplyResources(this.lblCaption, "lblCaption");
          this.lblCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.lblCaption.Name = "lblCaption";
          // 
          // lblMainCaption
          // 
          resources.ApplyResources(this.lblMainCaption, "lblMainCaption");
          this.lblMainCaption.ForeColor = System.Drawing.Color.White;
          this.lblMainCaption.Name = "lblMainCaption";
          // 
          // timerStartValidation
          // 
          this.timerStartValidation.Tick += new System.EventHandler(this.TimerStartValidationTick);
          // 
          // btnEsc
          // 
          this.btnEsc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btnEsc.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnEsc.BorderColorClicked = System.Drawing.Color.Black;
          this.btnEsc.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnEsc.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btnEsc.ClickedBgImage")));
          this.btnEsc.DisabledBgImage = null;
          this.btnEsc.FgImage = null;
          this.btnEsc.FocusedBgImage = null;
          resources.ApplyResources(this.btnEsc, "btnEsc");
          this.btnEsc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btnEsc.IsPushed = false;
          this.btnEsc.Name = "btnEsc";
          this.btnEsc.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btnEsc.NormalBgImage")));
          this.btnEsc.TabStop = false;
          this.btnEsc.ToggleMode = false;
          this.btnEsc.Click += new System.EventHandler(this.BtnEscClick);
          // 
          // btnEnglish
          // 
          this.btnEnglish.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnEnglish.BorderColorClicked = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
          this.btnEnglish.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnEnglish.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btnEnglish.ClickedBgImage")));
          this.btnEnglish.DisabledBgImage = null;
          this.btnEnglish.FgImage = null;
          this.btnEnglish.FocusedBgImage = ((System.Drawing.Image)(resources.GetObject("btnEnglish.FocusedBgImage")));
          this.btnEnglish.IsPushed = false;
          resources.ApplyResources(this.btnEnglish, "btnEnglish");
          this.btnEnglish.Name = "btnEnglish";
          this.btnEnglish.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btnEnglish.NormalBgImage")));
          this.btnEnglish.ToggleMode = false;
          this.btnEnglish.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnEnglishMouseDown);
          // 
          // btnFrench
          // 
          this.btnFrench.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnFrench.BorderColorClicked = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
          this.btnFrench.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnFrench.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btnFrench.ClickedBgImage")));
          this.btnFrench.DisabledBgImage = null;
          this.btnFrench.FgImage = null;
          this.btnFrench.FocusedBgImage = ((System.Drawing.Image)(resources.GetObject("btnFrench.FocusedBgImage")));
          this.btnFrench.IsPushed = false;
          resources.ApplyResources(this.btnFrench, "btnFrench");
          this.btnFrench.Name = "btnFrench";
          this.btnFrench.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btnFrench.NormalBgImage")));
          this.btnFrench.ToggleMode = false;
          this.btnFrench.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnFrenchMouseDown);
          // 
          // btnGerman
          // 
          this.btnGerman.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnGerman.BorderColorClicked = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
          this.btnGerman.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnGerman.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btnGerman.ClickedBgImage")));
          this.btnGerman.DisabledBgImage = null;
          this.btnGerman.FgImage = null;
          this.btnGerman.FocusedBgImage = ((System.Drawing.Image)(resources.GetObject("btnGerman.FocusedBgImage")));
          this.btnGerman.IsPushed = false;
          resources.ApplyResources(this.btnGerman, "btnGerman");
          this.btnGerman.Name = "btnGerman";
          this.btnGerman.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btnGerman.NormalBgImage")));
          this.btnGerman.ToggleMode = false;
          this.btnGerman.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnGermanMouseDown);
          // 
          // btnEnter
          // 
          this.btnEnter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btnEnter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnEnter.BorderColorClicked = System.Drawing.Color.Black;
          this.btnEnter.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnEnter.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btnEnter.ClickedBgImage")));
          this.btnEnter.DisabledBgImage = null;
          this.btnEnter.FgImage = null;
          this.btnEnter.FocusedBgImage = null;
          resources.ApplyResources(this.btnEnter, "btnEnter");
          this.btnEnter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btnEnter.IsPushed = false;
          this.btnEnter.Name = "btnEnter";
          this.btnEnter.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btnEnter.NormalBgImage")));
          this.btnEnter.TabStop = false;
          this.btnEnter.ToggleMode = false;
          this.btnEnter.Click += new System.EventHandler(this.BtnEnterClick);
          // 
          // btnBackspace
          // 
          this.btnBackspace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btnBackspace.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnBackspace.BorderColorClicked = System.Drawing.Color.Black;
          this.btnBackspace.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnBackspace.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btnBackspace.ClickedBgImage")));
          this.btnBackspace.DisabledBgImage = null;
          this.btnBackspace.FgImage = null;
          this.btnBackspace.FocusedBgImage = null;
          resources.ApplyResources(this.btnBackspace, "btnBackspace");
          this.btnBackspace.ForeColor = System.Drawing.Color.Black;
          this.btnBackspace.IsPushed = false;
          this.btnBackspace.Name = "btnBackspace";
          this.btnBackspace.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btnBackspace.NormalBgImage")));
          this.btnBackspace.TabStop = false;
          this.btnBackspace.ToggleMode = false;
          this.btnBackspace.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnBackspaceMouseDown);
          // 
          // btn0
          // 
          this.btn0.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btn0.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn0.BorderColorClicked = System.Drawing.Color.Black;
          this.btn0.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn0.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn0.ClickedBgImage")));
          this.btn0.DisabledBgImage = null;
          this.btn0.FgImage = null;
          this.btn0.FocusedBgImage = null;
          resources.ApplyResources(this.btn0, "btn0");
          this.btn0.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn0.IsPushed = false;
          this.btn0.Name = "btn0";
          this.btn0.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn0.NormalBgImage")));
          this.btn0.TabStop = false;
          this.btn0.ToggleMode = false;
          this.btn0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn0MouseDown);
          // 
          // btnClear
          // 
          this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btnClear.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnClear.BorderColorClicked = System.Drawing.Color.Black;
          this.btnClear.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btnClear.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btnClear.ClickedBgImage")));
          this.btnClear.DisabledBgImage = null;
          this.btnClear.FgImage = null;
          this.btnClear.FocusedBgImage = null;
          resources.ApplyResources(this.btnClear, "btnClear");
          this.btnClear.ForeColor = System.Drawing.Color.Black;
          this.btnClear.IsPushed = false;
          this.btnClear.Name = "btnClear";
          this.btnClear.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btnClear.NormalBgImage")));
          this.btnClear.TabStop = false;
          this.btnClear.ToggleMode = false;
          this.btnClear.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnClearMouseDown);
          // 
          // btn9
          // 
          this.btn9.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn9.BorderColorClicked = System.Drawing.Color.Black;
          this.btn9.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn9.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn9.ClickedBgImage")));
          this.btn9.DisabledBgImage = null;
          this.btn9.FgImage = null;
          this.btn9.FocusedBgImage = null;
          resources.ApplyResources(this.btn9, "btn9");
          this.btn9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn9.IsPushed = false;
          this.btn9.Name = "btn9";
          this.btn9.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn9.NormalBgImage")));
          this.btn9.TabStop = false;
          this.btn9.ToggleMode = false;
          this.btn9.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn9MouseDown);
          // 
          // btn8
          // 
          this.btn8.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn8.BorderColorClicked = System.Drawing.Color.Black;
          this.btn8.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn8.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn8.ClickedBgImage")));
          this.btn8.DisabledBgImage = null;
          this.btn8.FgImage = null;
          this.btn8.FocusedBgImage = null;
          resources.ApplyResources(this.btn8, "btn8");
          this.btn8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn8.IsPushed = false;
          this.btn8.Name = "btn8";
          this.btn8.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn8.NormalBgImage")));
          this.btn8.TabStop = false;
          this.btn8.ToggleMode = false;
          this.btn8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn8MouseDown);
          // 
          // btn7
          // 
          this.btn7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btn7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn7.BorderColorClicked = System.Drawing.Color.Black;
          this.btn7.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn7.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn7.ClickedBgImage")));
          this.btn7.DisabledBgImage = null;
          this.btn7.FgImage = null;
          this.btn7.FocusedBgImage = null;
          resources.ApplyResources(this.btn7, "btn7");
          this.btn7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn7.IsPushed = false;
          this.btn7.Name = "btn7";
          this.btn7.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn7.NormalBgImage")));
          this.btn7.TabStop = false;
          this.btn7.ToggleMode = false;
          this.btn7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn7MouseDown);
          // 
          // btn4
          // 
          this.btn4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btn4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn4.BorderColorClicked = System.Drawing.Color.Black;
          this.btn4.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn4.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn4.ClickedBgImage")));
          this.btn4.DisabledBgImage = null;
          this.btn4.FgImage = null;
          this.btn4.FocusedBgImage = null;
          resources.ApplyResources(this.btn4, "btn4");
          this.btn4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn4.IsPushed = false;
          this.btn4.Name = "btn4";
          this.btn4.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn4.NormalBgImage")));
          this.btn4.TabStop = false;
          this.btn4.ToggleMode = false;
          this.btn4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn4MouseDown);
          // 
          // btn5
          // 
          this.btn5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btn5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn5.BorderColorClicked = System.Drawing.Color.Black;
          this.btn5.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn5.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn5.ClickedBgImage")));
          this.btn5.DisabledBgImage = null;
          this.btn5.FgImage = null;
          this.btn5.FocusedBgImage = null;
          resources.ApplyResources(this.btn5, "btn5");
          this.btn5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn5.IsPushed = false;
          this.btn5.Name = "btn5";
          this.btn5.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn5.NormalBgImage")));
          this.btn5.TabStop = false;
          this.btn5.ToggleMode = false;
          this.btn5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn5MouseDown);
          // 
          // btn6
          // 
          this.btn6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btn6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn6.BorderColorClicked = System.Drawing.Color.Black;
          this.btn6.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn6.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn6.ClickedBgImage")));
          this.btn6.DisabledBgImage = null;
          this.btn6.FgImage = null;
          this.btn6.FocusedBgImage = null;
          resources.ApplyResources(this.btn6, "btn6");
          this.btn6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn6.IsPushed = false;
          this.btn6.Name = "btn6";
          this.btn6.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn6.NormalBgImage")));
          this.btn6.TabStop = false;
          this.btn6.ToggleMode = false;
          this.btn6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn6MouseDown);
          // 
          // btn3
          // 
          this.btn3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btn3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn3.BorderColorClicked = System.Drawing.Color.Black;
          this.btn3.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn3.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn3.ClickedBgImage")));
          this.btn3.DisabledBgImage = null;
          this.btn3.FgImage = null;
          this.btn3.FocusedBgImage = null;
          resources.ApplyResources(this.btn3, "btn3");
          this.btn3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn3.IsPushed = false;
          this.btn3.Name = "btn3";
          this.btn3.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn3.NormalBgImage")));
          this.btn3.TabStop = false;
          this.btn3.ToggleMode = false;
          this.btn3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn3MouseDown);
          // 
          // btn2
          // 
          this.btn2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btn2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn2.BorderColorClicked = System.Drawing.Color.Black;
          this.btn2.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn2.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn2.ClickedBgImage")));
          this.btn2.DisabledBgImage = null;
          this.btn2.FgImage = null;
          this.btn2.FocusedBgImage = null;
          resources.ApplyResources(this.btn2, "btn2");
          this.btn2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn2.IsPushed = false;
          this.btn2.Name = "btn2";
          this.btn2.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn2.NormalBgImage")));
          this.btn2.TabStop = false;
          this.btn2.ToggleMode = false;
          this.btn2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn2MouseDown);
          // 
          // btn1
          // 
          this.btn1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
          this.btn1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn1.BorderColorClicked = System.Drawing.Color.Black;
          this.btn1.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.btn1.ClickedBgImage = ((System.Drawing.Image)(resources.GetObject("btn1.ClickedBgImage")));
          this.btn1.DisabledBgImage = null;
          this.btn1.FgImage = null;
          this.btn1.FocusedBgImage = null;
          resources.ApplyResources(this.btn1, "btn1");
          this.btn1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.btn1.IsPushed = false;
          this.btn1.Name = "btn1";
          this.btn1.NormalBgImage = ((System.Drawing.Image)(resources.GetObject("btn1.NormalBgImage")));
          this.btn1.TabStop = false;
          this.btn1.ToggleMode = false;
          this.btn1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Btn1MouseDown);
          // 
          // txtInputNumber
          // 
          this.txtInputNumber.BackColor = System.Drawing.SystemColors.ControlDarkDark;
          this.txtInputNumber.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.txtInputNumber.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          resources.ApplyResources(this.txtInputNumber, "txtInputNumber");
          this.txtInputNumber.ForeColor = System.Drawing.Color.White;
          this.txtInputNumber.Name = "txtInputNumber";
          this.txtInputNumber.TextChanged += new System.EventHandler(this.TxtInputNumberTextChanged);
          // 
          // lblHint
          // 
          resources.ApplyResources(this.lblHint, "lblHint");
          this.lblHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
          this.lblHint.Name = "lblHint";
          // 
          // txtInputNumber2
          // 
          this.txtInputNumber2.BackColor = System.Drawing.SystemColors.ControlDarkDark;
          this.txtInputNumber2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          this.txtInputNumber2.BorderColorFocused = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
          resources.ApplyResources(this.txtInputNumber2, "txtInputNumber2");
          this.txtInputNumber2.ForeColor = System.Drawing.Color.White;
          this.txtInputNumber2.Name = "txtInputNumber2";
          // 
          // NumberInput
          // 
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
          this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
          this.Controls.Add(this.txtInputNumber2);
          this.Controls.Add(this.btnEsc);
          this.Controls.Add(this.btnEnter);
          this.Controls.Add(this.btnBackspace);
          this.Controls.Add(this.btn0);
          this.Controls.Add(this.btnClear);
          this.Controls.Add(this.btn9);
          this.Controls.Add(this.btn8);
          this.Controls.Add(this.btn7);
          this.Controls.Add(this.btn4);
          this.Controls.Add(this.btn5);
          this.Controls.Add(this.btn6);
          this.Controls.Add(this.btn3);
          this.Controls.Add(this.btn2);
          this.Controls.Add(this.btn1);
          this.Controls.Add(this.lblHint);
          this.Controls.Add(this.lblMainCaption);
          this.Controls.Add(this.btnEnglish);
          this.Controls.Add(this.btnFrench);
          this.Controls.Add(this.btnGerman);
          this.Controls.Add(this.txtInputNumber);
          this.Controls.Add(this.lblCaption);
          this.ForeColor = System.Drawing.Color.Black;
          this.Name = "NumberInput";
          resources.ApplyResources(this, "$this");
          this.ResumeLayout(false);

        }

        #endregion

        private VCustomButton btnBackspace;
        private VCustomButton btn0;
        private VCustomButton btnClear;
        private VCustomButton btn9;
        private VCustomButton btn8;
        private VCustomButton btn7;
        private VCustomButton btn4;
        private VCustomButton btn5;
        private VCustomButton btn6;
        private VCustomButton btn3;
        private VCustomButton btn2;
        private VCustomButton btn1;
        internal CustomTextbox txtInputNumber;
        private System.Windows.Forms.Label lblCaption;
        private VCustomButton btnEnter;
        private VCustomButton btnEnglish;
        private VCustomButton btnFrench;
        private VCustomButton btnGerman;
        internal System.Windows.Forms.Label lblMainCaption;
        private VCustomButton btnEsc;
        private System.Windows.Forms.Timer timerStartValidation;
        private System.Windows.Forms.Label lblHint;
        internal CustomTextbox txtInputNumber2;
    }
}
