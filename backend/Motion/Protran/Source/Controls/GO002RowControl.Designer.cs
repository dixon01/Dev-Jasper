namespace Gorba.Motion.Protran.Controls
{
    partial class GO002RowControl
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
            this.textBox_DataLength = new SuggestTextBox();
            this.textBox_StopIndex = new SuggestTextBox();
            this.textBox_Pictogram = new SuggestTextBox();
            this.textBox_RowNumber = new SuggestTextBox();
            this.textBox_Deviation = new SuggestTextBox();
            this.textBox_TrackNumber = new SuggestTextBox();
            this.textBox_DepartureTime = new SuggestTextBox();
            this.textBox_LineNumber = new SuggestTextBox();
            this.textBox_DestinationName = new SuggestTextBox();
            this.ApplyButtonGO001 = new Gorba.Motion.Protran.Controls.ApplySendButton();
            this.SuspendLayout();
            // 
            // textBox_DataLength
            // 
            this.textBox_DataLength.Location = new System.Drawing.Point(3, 3);
            this.textBox_DataLength.Name = "textBox_DataLength";
            this.textBox_DataLength.Size = new System.Drawing.Size(66, 20);
            this.textBox_DataLength.Suggestion = "XXX";
            this.textBox_DataLength.TabIndex = 0;
            this.textBox_DataLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_StopIndex
            // 
            this.textBox_StopIndex.Location = new System.Drawing.Point(75, 3);
            this.textBox_StopIndex.Name = "textBox_StopIndex";
            this.textBox_StopIndex.Size = new System.Drawing.Size(66, 20);
            this.textBox_StopIndex.Suggestion = "HH";
            this.textBox_StopIndex.TabIndex = 1;
            this.textBox_StopIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_StopIndex.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox_Pictogram
            // 
            this.textBox_Pictogram.Location = new System.Drawing.Point(219, 3);
            this.textBox_Pictogram.Name = "textBox_Pictogram";
            this.textBox_Pictogram.Size = new System.Drawing.Size(66, 20);
            this.textBox_Pictogram.Suggestion = "P";
            this.textBox_Pictogram.TabIndex = 3;
            this.textBox_Pictogram.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_Pictogram.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox_RowNumber
            // 
            this.textBox_RowNumber.Location = new System.Drawing.Point(147, 3);
            this.textBox_RowNumber.Name = "textBox_RowNumber";
            this.textBox_RowNumber.Size = new System.Drawing.Size(66, 20);
            this.textBox_RowNumber.Suggestion = "Z";
            this.textBox_RowNumber.TabIndex = 2;
            this.textBox_RowNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_RowNumber.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox_Deviation
            // 
            this.textBox_Deviation.Location = new System.Drawing.Point(507, 3);
            this.textBox_Deviation.Name = "textBox_Deviation";
            this.textBox_Deviation.Size = new System.Drawing.Size(66, 20);
            this.textBox_Deviation.Suggestion = "VVVV";
            this.textBox_Deviation.TabIndex = 7;
            this.textBox_Deviation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_Deviation.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox_TrackNumber
            // 
            this.textBox_TrackNumber.Location = new System.Drawing.Point(435, 3);
            this.textBox_TrackNumber.Name = "textBox_TrackNumber";
            this.textBox_TrackNumber.Size = new System.Drawing.Size(66, 20);
            this.textBox_TrackNumber.Suggestion = "MM";
            this.textBox_TrackNumber.TabIndex = 6;
            this.textBox_TrackNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_TrackNumber.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox_DepartureTime
            // 
            this.textBox_DepartureTime.Location = new System.Drawing.Point(363, 3);
            this.textBox_DepartureTime.Name = "textBox_DepartureTime";
            this.textBox_DepartureTime.Size = new System.Drawing.Size(66, 20);
            this.textBox_DepartureTime.Suggestion = "UUUU";
            this.textBox_DepartureTime.TabIndex = 5;
            this.textBox_DepartureTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_DepartureTime.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox_LineNumber
            // 
            this.textBox_LineNumber.Location = new System.Drawing.Point(291, 3);
            this.textBox_LineNumber.Name = "textBox_LineNumber";
            this.textBox_LineNumber.Size = new System.Drawing.Size(66, 20);
            this.textBox_LineNumber.Suggestion = "LLLLL";
            this.textBox_LineNumber.TabIndex = 4;
            this.textBox_LineNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_LineNumber.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox_DestinationName
            // 
            this.textBox_DestinationName.Location = new System.Drawing.Point(579, 3);
            this.textBox_DestinationName.Name = "textBox_DestinationName";
            this.textBox_DestinationName.Size = new System.Drawing.Size(109, 20);
            this.textBox_DestinationName.Suggestion = "Max. 30 characters";
            this.textBox_DestinationName.TabIndex = 8;
            this.textBox_DestinationName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_DestinationName.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // ApplyButtonGO001
            // 
            this.ApplyButtonGO001.Location = new System.Drawing.Point(694, 0);
            this.ApplyButtonGO001.Name = "ApplyButtonGO001";
            this.ApplyButtonGO001.Size = new System.Drawing.Size(45, 23);
            this.ApplyButtonGO001.TabIndex = 9;
            this.ApplyButtonGO001.Text = "Apply";
            this.ApplyButtonGO001.UseVisualStyleBackColor = true;
            this.ApplyButtonGO001.SendClick += new System.EventHandler(this.ButtonSendClick);
            this.ApplyButtonGO001.Click += new System.EventHandler(this.ButtonApplyClick);
            // 
            // GO002Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ApplyButtonGO001);
            this.Controls.Add(this.textBox_DestinationName);
            this.Controls.Add(this.textBox_Deviation);
            this.Controls.Add(this.textBox_TrackNumber);
            this.Controls.Add(this.textBox_DepartureTime);
            this.Controls.Add(this.textBox_LineNumber);
            this.Controls.Add(this.textBox_Pictogram);
            this.Controls.Add(this.textBox_RowNumber);
            this.Controls.Add(this.textBox_StopIndex);
            this.Controls.Add(this.textBox_DataLength);
            this.Name = "GO002Control";
            this.Size = new System.Drawing.Size(743, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SuggestTextBox textBox_DataLength;
        private SuggestTextBox textBox_StopIndex;
        private SuggestTextBox textBox_Pictogram;
        private SuggestTextBox textBox_RowNumber;
        private SuggestTextBox textBox_Deviation;
        private SuggestTextBox textBox_TrackNumber;
        private SuggestTextBox textBox_DepartureTime;
        private SuggestTextBox textBox_LineNumber;
        private SuggestTextBox textBox_DestinationName;
        private ApplySendButton ApplyButtonGO001;
    }
}
