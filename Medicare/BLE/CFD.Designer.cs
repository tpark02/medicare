using System;

namespace Medicare.BLE
{
    partial class CFD
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CFD));
            this.label11 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.maxDriftRateTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.initialFreqDriftTextBox = new System.Windows.Forms.TextBox();
            this.freqDriftTextBox = new System.Windows.Forms.TextBox();
            this.freqAccuracyTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cfdSpecLabel = new System.Windows.Forms.Label();
            this.channelTextBox = new System.Windows.Forms.TextBox();
            this.cfdConditionLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.packetNumTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(645, 457);
            this.label11.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(122, 32);
            this.label11.TabIndex = 98;
            this.label11.Text = "kHz/50us";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(648, 402);
            this.label5.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 32);
            this.label5.TabIndex = 97;
            this.label5.Text = "kHz";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(648, 353);
            this.label3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 32);
            this.label3.TabIndex = 96;
            this.label3.Text = "kHz";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(648, 310);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 32);
            this.label2.TabIndex = 95;
            this.label2.Text = "kHz";
            // 
            // maxDriftRateTextBox
            // 
            this.maxDriftRateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.maxDriftRateTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maxDriftRateTextBox.Location = new System.Drawing.Point(338, 451);
            this.maxDriftRateTextBox.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.maxDriftRateTextBox.Name = "maxDriftRateTextBox";
            this.maxDriftRateTextBox.Size = new System.Drawing.Size(295, 33);
            this.maxDriftRateTextBox.TabIndex = 94;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(30, 448);
            this.label10.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(488, 39);
            this.label10.TabIndex = 93;
            this.label10.Text = "Max Drift Rate |fn-fn-5| (n=6,7...) <=";
            // 
            // initialFreqDriftTextBox
            // 
            this.initialFreqDriftTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.initialFreqDriftTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.initialFreqDriftTextBox.Location = new System.Drawing.Point(338, 399);
            this.initialFreqDriftTextBox.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.initialFreqDriftTextBox.Name = "initialFreqDriftTextBox";
            this.initialFreqDriftTextBox.Size = new System.Drawing.Size(295, 33);
            this.initialFreqDriftTextBox.TabIndex = 92;
            // 
            // freqDriftTextBox
            // 
            this.freqDriftTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.freqDriftTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.freqDriftTextBox.Location = new System.Drawing.Point(338, 351);
            this.freqDriftTextBox.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.freqDriftTextBox.Name = "freqDriftTextBox";
            this.freqDriftTextBox.Size = new System.Drawing.Size(295, 33);
            this.freqDriftTextBox.TabIndex = 91;
            // 
            // freqAccuracyTextBox
            // 
            this.freqAccuracyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.freqAccuracyTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.freqAccuracyTextBox.Location = new System.Drawing.Point(338, 304);
            this.freqAccuracyTextBox.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.freqAccuracyTextBox.Name = "freqAccuracyTextBox";
            this.freqAccuracyTextBox.Size = new System.Drawing.Size(295, 33);
            this.freqAccuracyTextBox.TabIndex = 87;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(30, 396);
            this.label9.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(444, 39);
            this.label9.TabIndex = 90;
            this.label9.Text = "Initial Frequency Drift |f1-f0| <=";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(30, 348);
            this.label8.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(482, 39);
            this.label8.TabIndex = 89;
            this.label8.Text = "Frequency Drift |f0-fn| (n=2,3...) <=";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(30, 301);
            this.label4.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(446, 39);
            this.label4.TabIndex = 88;
            this.label4.Text = "Frequency Accuracy  |ftx-fn| <=";
            // 
            // cfdSpecLabel
            // 
            this.cfdSpecLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.cfdSpecLabel.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cfdSpecLabel.Location = new System.Drawing.Point(19, 218);
            this.cfdSpecLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.cfdSpecLabel.Name = "cfdSpecLabel";
            this.cfdSpecLabel.Size = new System.Drawing.Size(694, 34);
            this.cfdSpecLabel.TabIndex = 86;
            this.cfdSpecLabel.Text = "Specification";
            this.cfdSpecLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // channelTextBox
            // 
            this.channelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.channelTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.channelTextBox.Location = new System.Drawing.Point(338, 107);
            this.channelTextBox.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.channelTextBox.Name = "channelTextBox";
            this.channelTextBox.Size = new System.Drawing.Size(295, 33);
            this.channelTextBox.TabIndex = 85;
            // 
            // cfdConditionLabel
            // 
            this.cfdConditionLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.cfdConditionLabel.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cfdConditionLabel.Location = new System.Drawing.Point(19, 38);
            this.cfdConditionLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.cfdConditionLabel.Name = "cfdConditionLabel";
            this.cfdConditionLabel.Size = new System.Drawing.Size(694, 34);
            this.cfdConditionLabel.TabIndex = 84;
            this.cfdConditionLabel.Text = "Condition";
            this.cfdConditionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(375, 538);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(219, 73);
            this.cancelButton.TabIndex = 83;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(97, 538);
            this.okButton.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(219, 73);
            this.okButton.TabIndex = 82;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(30, 104);
            this.label7.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 39);
            this.label7.TabIndex = 79;
            this.label7.Text = "Channel";
            // 
            // packetNumTextBox
            // 
            this.packetNumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packetNumTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetNumTextBox.Location = new System.Drawing.Point(338, 158);
            this.packetNumTextBox.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.packetNumTextBox.Name = "packetNumTextBox";
            this.packetNumTextBox.Size = new System.Drawing.Size(295, 33);
            this.packetNumTextBox.TabIndex = 81;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(30, 158);
            this.label6.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(274, 39);
            this.label6.TabIndex = 80;
            this.label6.Text = "Number of Packet";
            // 
            // CFD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(18F, 39F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(727, 644);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.maxDriftRateTextBox);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.initialFreqDriftTextBox);
            this.Controls.Add(this.freqDriftTextBox);
            this.Controls.Add(this.freqAccuracyTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cfdSpecLabel);
            this.Controls.Add(this.channelTextBox);
            this.Controls.Add(this.cfdConditionLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.packetNumTextBox);
            this.Controls.Add(this.label6);
            this.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.Name = "CFD";
            this.Text = "Carrier Frequence Offset and Drift";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox maxDriftRateTextBox;
        internal System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox initialFreqDriftTextBox;
        private System.Windows.Forms.TextBox freqDriftTextBox;
        private System.Windows.Forms.TextBox freqAccuracyTextBox;
        internal System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label cfdSpecLabel;
        private System.Windows.Forms.TextBox channelTextBox;
        private System.Windows.Forms.Label cfdConditionLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox packetNumTextBox;
        private System.Windows.Forms.Label label6;
    }
}