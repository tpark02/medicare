using System;

namespace Medicare.BLE
{
    partial class OP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OP));
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.opSpecLabel = new System.Windows.Forms.Label();
            this.channelTextBox = new System.Windows.Forms.TextBox();
            this.opCondLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.powerDiffTextBox = new System.Windows.Forms.TextBox();
            this.avgPwrLowerTextBox = new System.Windows.Forms.TextBox();
            this.avgPwrUpperTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.packetNumTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(317, 272);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(27, 18);
            this.label5.TabIndex = 84;
            this.label5.Text = "dB";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(297, 231);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 18);
            this.label3.TabIndex = 83;
            this.label3.Text = "[dBM]";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(297, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 18);
            this.label2.TabIndex = 82;
            this.label2.Text = "[dBM]";
            // 
            // opSpecLabel
            // 
            this.opSpecLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.opSpecLabel.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opSpecLabel.Location = new System.Drawing.Point(4, 136);
            this.opSpecLabel.Name = "opSpecLabel";
            this.opSpecLabel.Size = new System.Drawing.Size(337, 31);
            this.opSpecLabel.TabIndex = 81;
            this.opSpecLabel.Text = "Specification";
            this.opSpecLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // channelTextBox
            // 
            this.channelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.channelTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.channelTextBox.Location = new System.Drawing.Point(144, 55);
            this.channelTextBox.Name = "channelTextBox";
            this.channelTextBox.Size = new System.Drawing.Size(147, 22);
            this.channelTextBox.TabIndex = 80;
            // 
            // opCondLabel
            // 
            this.opCondLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.opCondLabel.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opCondLabel.Location = new System.Drawing.Point(4, 9);
            this.opCondLabel.Name = "opCondLabel";
            this.opCondLabel.Size = new System.Drawing.Size(337, 31);
            this.opCondLabel.TabIndex = 79;
            this.opCondLabel.Text = "Condition";
            this.opCondLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(191, 321);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(113, 41);
            this.cancelButton.TabIndex = 78;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click_1);
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(39, 321);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(113, 41);
            this.okButton.TabIndex = 77;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // powerDiffTextBox
            // 
            this.powerDiffTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.powerDiffTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.powerDiffTextBox.Location = new System.Drawing.Point(144, 272);
            this.powerDiffTextBox.Name = "powerDiffTextBox";
            this.powerDiffTextBox.Size = new System.Drawing.Size(147, 22);
            this.powerDiffTextBox.TabIndex = 76;
            // 
            // avgPwrLowerTextBox
            // 
            this.avgPwrLowerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.avgPwrLowerTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.avgPwrLowerTextBox.Location = new System.Drawing.Point(144, 226);
            this.avgPwrLowerTextBox.Name = "avgPwrLowerTextBox";
            this.avgPwrLowerTextBox.Size = new System.Drawing.Size(147, 22);
            this.avgPwrLowerTextBox.TabIndex = 75;
            // 
            // avgPwrUpperTextBox
            // 
            this.avgPwrUpperTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.avgPwrUpperTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.avgPwrUpperTextBox.Location = new System.Drawing.Point(144, 184);
            this.avgPwrUpperTextBox.Name = "avgPwrUpperTextBox";
            this.avgPwrUpperTextBox.Size = new System.Drawing.Size(147, 22);
            this.avgPwrUpperTextBox.TabIndex = 71;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(8, 277);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(111, 18);
            this.label9.TabIndex = 74;
            this.label9.Text = "Ppeak - Pavg <=";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(8, 231);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(120, 18);
            this.label8.TabIndex = 73;
            this.label8.Text = "Avg Power Lower";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 189);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 18);
            this.label4.TabIndex = 72;
            this.label4.Text = "Avg Power Upper";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(4, 58);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 18);
            this.label7.TabIndex = 68;
            this.label7.Text = "Channel";
            // 
            // packetNumTextBox
            // 
            this.packetNumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packetNumTextBox.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetNumTextBox.Location = new System.Drawing.Point(144, 97);
            this.packetNumTextBox.Name = "packetNumTextBox";
            this.packetNumTextBox.Size = new System.Drawing.Size(147, 22);
            this.packetNumTextBox.TabIndex = 70;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(4, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 18);
            this.label6.TabIndex = 69;
            this.label6.Text = "Number of Packet";
            // 
            // OP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(353, 373);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.opSpecLabel);
            this.Controls.Add(this.channelTextBox);
            this.Controls.Add(this.opCondLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.powerDiffTextBox);
            this.Controls.Add(this.avgPwrLowerTextBox);
            this.Controls.Add(this.avgPwrUpperTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.packetNumTextBox);
            this.Controls.Add(this.label6);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "OP";
            this.Text = "Output Power 1M";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OP_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label opSpecLabel;
        private System.Windows.Forms.TextBox channelTextBox;
        private System.Windows.Forms.Label opCondLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox powerDiffTextBox;
        private System.Windows.Forms.TextBox avgPwrLowerTextBox;
        private System.Windows.Forms.TextBox avgPwrUpperTextBox;
        internal System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox packetNumTextBox;
        private System.Windows.Forms.Label label6;
    }
}