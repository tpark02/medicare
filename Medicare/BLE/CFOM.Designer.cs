namespace Medicare.BLE
{
    partial class CFOM
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CFOM));
            this.channelTextBox = new System.Windows.Forms.TextBox();
            this.cfomCondLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.packetNumTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cfomSpecLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.deltaf2minTextBox = new System.Windows.Forms.TextBox();
            this.deltaf2avgTextBox = new System.Windows.Forms.TextBox();
            this.maxFreqTol_LowerTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.maxFreqTol_UpperTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // channelTextBox
            // 
            this.channelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.channelTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.channelTextBox.Location = new System.Drawing.Point(231, 89);
            this.channelTextBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.channelTextBox.Name = "channelTextBox";
            this.channelTextBox.Size = new System.Drawing.Size(230, 33);
            this.channelTextBox.TabIndex = 85;
            // 
            // cfomCondLabel
            // 
            this.cfomCondLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.cfomCondLabel.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cfomCondLabel.Location = new System.Drawing.Point(11, 10);
            this.cfomCondLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.cfomCondLabel.Name = "cfomCondLabel";
            this.cfomCondLabel.Size = new System.Drawing.Size(530, 54);
            this.cfomCondLabel.TabIndex = 84;
            this.cfomCondLabel.Text = "Condition";
            this.cfomCondLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(11, 96);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 28);
            this.label7.TabIndex = 81;
            this.label7.Text = "Channel";
            // 
            // packetNumTextBox
            // 
            this.packetNumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packetNumTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetNumTextBox.Location = new System.Drawing.Point(231, 164);
            this.packetNumTextBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.packetNumTextBox.Name = "packetNumTextBox";
            this.packetNumTextBox.Size = new System.Drawing.Size(230, 33);
            this.packetNumTextBox.TabIndex = 83;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(11, 170);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(218, 28);
            this.label6.TabIndex = 82;
            this.label6.Text = "Number of Packet";
            // 
            // cfomSpecLabel
            // 
            this.cfomSpecLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.cfomSpecLabel.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cfomSpecLabel.Location = new System.Drawing.Point(14, 222);
            this.cfomSpecLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.cfomSpecLabel.Name = "cfomSpecLabel";
            this.cfomSpecLabel.Size = new System.Drawing.Size(530, 54);
            this.cfomSpecLabel.TabIndex = 86;
            this.cfomSpecLabel.Text = "Specification";
            this.cfomSpecLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(280, 527);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(178, 72);
            this.cancelButton.TabIndex = 94;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(75, 527);
            this.okButton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(178, 72);
            this.okButton.TabIndex = 93;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // deltaf2minTextBox
            // 
            this.deltaf2minTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deltaf2minTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deltaf2minTextBox.Location = new System.Drawing.Point(226, 457);
            this.deltaf2minTextBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.deltaf2minTextBox.Name = "deltaf2minTextBox";
            this.deltaf2minTextBox.Size = new System.Drawing.Size(236, 33);
            this.deltaf2minTextBox.TabIndex = 92;
            // 
            // deltaf2avgTextBox
            // 
            this.deltaf2avgTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.deltaf2avgTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deltaf2avgTextBox.Location = new System.Drawing.Point(226, 387);
            this.deltaf2avgTextBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.deltaf2avgTextBox.Name = "deltaf2avgTextBox";
            this.deltaf2avgTextBox.Size = new System.Drawing.Size(236, 33);
            this.deltaf2avgTextBox.TabIndex = 91;
            // 
            // maxFreqTol_LowerTextBox
            // 
            this.maxFreqTol_LowerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxFreqTol_LowerTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxFreqTol_LowerTextBox.Location = new System.Drawing.Point(231, 322);
            this.maxFreqTol_LowerTextBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.maxFreqTol_LowerTextBox.Name = "maxFreqTol_LowerTextBox";
            this.maxFreqTol_LowerTextBox.Size = new System.Drawing.Size(112, 33);
            this.maxFreqTol_LowerTextBox.TabIndex = 87;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(13, 466);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(152, 28);
            this.label9.TabIndex = 90;
            this.label9.Text = "Delta f2 Min";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(13, 392);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(162, 28);
            this.label8.TabIndex = 89;
            this.label8.Text = "Delta f2 avg.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 322);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(174, 28);
            this.label4.TabIndex = 88;
            this.label4.Text = "Max. Freq. Tol";
            // 
            // maxFreqTol_UpperTextBox
            // 
            this.maxFreqTol_UpperTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxFreqTol_UpperTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxFreqTol_UpperTextBox.Location = new System.Drawing.Point(350, 322);
            this.maxFreqTol_UpperTextBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.maxFreqTol_UpperTextBox.Name = "maxFreqTol_UpperTextBox";
            this.maxFreqTol_UpperTextBox.Size = new System.Drawing.Size(112, 33);
            this.maxFreqTol_UpperTextBox.TabIndex = 95;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(237, 276);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 28);
            this.label2.TabIndex = 96;
            this.label2.Text = "Lower";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(352, 276);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 28);
            this.label3.TabIndex = 97;
            this.label3.Text = "Upper";
            // 
            // CFOM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(555, 630);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.maxFreqTol_UpperTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.deltaf2minTextBox);
            this.Controls.Add(this.deltaf2avgTextBox);
            this.Controls.Add(this.maxFreqTol_LowerTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cfomSpecLabel);
            this.Controls.Add(this.channelTextBox);
            this.Controls.Add(this.cfomCondLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.packetNumTextBox);
            this.Controls.Add(this.label6);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CFOM";
            this.Text = "Carrier Freq. Offset + Mod Char (Preamble) 1M";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox channelTextBox;
        private System.Windows.Forms.Label cfomCondLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox packetNumTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label cfomSpecLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox deltaf2minTextBox;
        private System.Windows.Forms.TextBox deltaf2avgTextBox;
        private System.Windows.Forms.TextBox maxFreqTol_LowerTextBox;
        internal System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox maxFreqTol_UpperTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}