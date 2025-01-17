using System;

namespace Medicare.BLE
{
    partial class RS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RS));
            this.dirtyComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.rsLevelTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.perTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rsSpecLabel = new System.Windows.Forms.Label();
            this.channelTextBox = new System.Windows.Forms.TextBox();
            this.rsCondLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.packetNumTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dirtyComboBox
            // 
            this.dirtyComboBox.FormattingEnabled = true;
            this.dirtyComboBox.Location = new System.Drawing.Point(244, 149);
            this.dirtyComboBox.Margin = new System.Windows.Forms.Padding(5);
            this.dirtyComboBox.Name = "dirtyComboBox";
            this.dirtyComboBox.Size = new System.Drawing.Size(237, 24);
            this.dirtyComboBox.TabIndex = 99;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(497, 189);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 18);
            this.label8.TabIndex = 98;
            this.label8.Text = "dBm";
            // 
            // rsLevelTextBox
            // 
            this.rsLevelTextBox.Font = new System.Drawing.Font("Verdana", 9F);
            this.rsLevelTextBox.Location = new System.Drawing.Point(246, 189);
            this.rsLevelTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.rsLevelTextBox.Name = "rsLevelTextBox";
            this.rsLevelTextBox.Size = new System.Drawing.Size(237, 22);
            this.rsLevelTextBox.TabIndex = 97;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(18, 196);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(63, 18);
            this.label5.TabIndex = 96;
            this.label5.Text = "RX Level";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 149);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 18);
            this.label4.TabIndex = 95;
            this.label4.Text = "Dirty";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(497, 303);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 18);
            this.label3.TabIndex = 94;
            this.label3.Text = "%";
            // 
            // perTextBox
            // 
            this.perTextBox.Location = new System.Drawing.Point(246, 297);
            this.perTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.perTextBox.Name = "perTextBox";
            this.perTextBox.Size = new System.Drawing.Size(237, 22);
            this.perTextBox.TabIndex = 93;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(18, 303);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 16);
            this.label2.TabIndex = 92;
            this.label2.Text = "PER";
            // 
            // rsSpecLabel
            // 
            this.rsSpecLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.rsSpecLabel.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rsSpecLabel.Location = new System.Drawing.Point(15, 239);
            this.rsSpecLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.rsSpecLabel.Name = "rsSpecLabel";
            this.rsSpecLabel.Size = new System.Drawing.Size(530, 35);
            this.rsSpecLabel.TabIndex = 91;
            this.rsSpecLabel.Text = "Specification";
            this.rsSpecLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // channelTextBox
            // 
            this.channelTextBox.Location = new System.Drawing.Point(245, 67);
            this.channelTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.channelTextBox.Name = "channelTextBox";
            this.channelTextBox.Size = new System.Drawing.Size(237, 22);
            this.channelTextBox.TabIndex = 90;
            // 
            // rsCondLabel
            // 
            this.rsCondLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.rsCondLabel.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rsCondLabel.Location = new System.Drawing.Point(15, 12);
            this.rsCondLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.rsCondLabel.Name = "rsCondLabel";
            this.rsCondLabel.Size = new System.Drawing.Size(530, 33);
            this.rsCondLabel.TabIndex = 89;
            this.rsCondLabel.Text = "Condition";
            this.rsCondLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(317, 368);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(5);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(184, 83);
            this.cancelButton.TabIndex = 88;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(69, 368);
            this.okButton.Margin = new System.Windows.Forms.Padding(5);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(184, 83);
            this.okButton.TabIndex = 87;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(17, 73);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 18);
            this.label7.TabIndex = 84;
            this.label7.Text = "Channel";
            // 
            // packetNumTextBox
            // 
            this.packetNumTextBox.Location = new System.Drawing.Point(244, 106);
            this.packetNumTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.packetNumTextBox.Name = "packetNumTextBox";
            this.packetNumTextBox.Size = new System.Drawing.Size(237, 22);
            this.packetNumTextBox.TabIndex = 86;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(16, 111);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 18);
            this.label6.TabIndex = 85;
            this.label6.Text = "Number of Packet";
            // 
            // RS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(557, 469);
            this.Controls.Add(this.dirtyComboBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.rsLevelTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.perTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rsSpecLabel);
            this.Controls.Add(this.channelTextBox);
            this.Controls.Add(this.rsCondLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.packetNumTextBox);
            this.Controls.Add(this.label6);
            this.Font = new System.Drawing.Font("Montserrat", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "RS";
            this.Text = "Receiver Sensitivity 1M";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox dirtyComboBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox rsLevelTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox perTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label rsSpecLabel;
        private System.Windows.Forms.TextBox channelTextBox;
        private System.Windows.Forms.Label rsCondLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox packetNumTextBox;
        private System.Windows.Forms.Label label6;
    }
}
