using System;

namespace Medicare.BLE
{
    partial class RS2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RS2));
            this.dirtyComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.rsLevelTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.perTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.rs2SpecLabel = new System.Windows.Forms.Label();
            this.channelTextBox = new System.Windows.Forms.TextBox();
            this.rs2CondLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.packetNumTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // dirtyComboBox
            // 
            this.dirtyComboBox.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dirtyComboBox.FormattingEnabled = true;
            this.dirtyComboBox.Location = new System.Drawing.Point(243, 142);
            this.dirtyComboBox.Margin = new System.Windows.Forms.Padding(5);
            this.dirtyComboBox.Name = "dirtyComboBox";
            this.dirtyComboBox.Size = new System.Drawing.Size(237, 47);
            this.dirtyComboBox.TabIndex = 115;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(494, 186);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 32);
            this.label8.TabIndex = 114;
            this.label8.Text = "dBm";
            // 
            // rsLevelTextBox
            // 
            this.rsLevelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rsLevelTextBox.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rsLevelTextBox.Location = new System.Drawing.Point(243, 186);
            this.rsLevelTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.rsLevelTextBox.Name = "rsLevelTextBox";
            this.rsLevelTextBox.Size = new System.Drawing.Size(237, 42);
            this.rsLevelTextBox.TabIndex = 113;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(15, 193);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 32);
            this.label5.TabIndex = 112;
            this.label5.Text = "RX Level";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(15, 142);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 32);
            this.label4.TabIndex = 111;
            this.label4.Text = "Dirty";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(495, 296);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 32);
            this.label3.TabIndex = 110;
            this.label3.Text = "%";
            // 
            // perTextBox
            // 
            this.perTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.perTextBox.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.perTextBox.Location = new System.Drawing.Point(244, 291);
            this.perTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.perTextBox.Name = "perTextBox";
            this.perTextBox.Size = new System.Drawing.Size(237, 42);
            this.perTextBox.TabIndex = 109;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 296);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 32);
            this.label2.TabIndex = 108;
            this.label2.Text = "PER";
            // 
            // rs2SpecLabel
            // 
            this.rs2SpecLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.rs2SpecLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rs2SpecLabel.Location = new System.Drawing.Point(15, 244);
            this.rs2SpecLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.rs2SpecLabel.Name = "rs2SpecLabel";
            this.rs2SpecLabel.Size = new System.Drawing.Size(528, 27);
            this.rs2SpecLabel.TabIndex = 107;
            this.rs2SpecLabel.Text = "Specification";
            this.rs2SpecLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // channelTextBox
            // 
            this.channelTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.channelTextBox.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.channelTextBox.Location = new System.Drawing.Point(243, 55);
            this.channelTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.channelTextBox.Name = "channelTextBox";
            this.channelTextBox.Size = new System.Drawing.Size(237, 42);
            this.channelTextBox.TabIndex = 106;
            // 
            // rs2CondLabel
            // 
            this.rs2CondLabel.BackColor = System.Drawing.Color.DarkSeaGreen;
            this.rs2CondLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rs2CondLabel.Location = new System.Drawing.Point(15, 17);
            this.rs2CondLabel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.rs2CondLabel.Name = "rs2CondLabel";
            this.rs2CondLabel.Size = new System.Drawing.Size(528, 23);
            this.rs2CondLabel.TabIndex = 105;
            this.rs2CondLabel.Text = "Condition";
            this.rs2CondLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cancelButton
            // 
            this.cancelButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(307, 350);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(5);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(184, 72);
            this.cancelButton.TabIndex = 104;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Font = new System.Drawing.Font("Montserrat Medium", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(59, 350);
            this.okButton.Margin = new System.Windows.Forms.Padding(5);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(184, 72);
            this.okButton.TabIndex = 103;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(15, 59);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(114, 32);
            this.label7.TabIndex = 100;
            this.label7.Text = "Channel";
            // 
            // packetNumTextBox
            // 
            this.packetNumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.packetNumTextBox.Font = new System.Drawing.Font("Montserrat", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetNumTextBox.Location = new System.Drawing.Point(243, 97);
            this.packetNumTextBox.Margin = new System.Windows.Forms.Padding(5);
            this.packetNumTextBox.Name = "packetNumTextBox";
            this.packetNumTextBox.Size = new System.Drawing.Size(237, 42);
            this.packetNumTextBox.TabIndex = 102;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Montserrat", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(15, 102);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(227, 32);
            this.label6.TabIndex = 101;
            this.label6.Text = "Number of Packet";
            // 
            // RS2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 446);
            this.Controls.Add(this.dirtyComboBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.rsLevelTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.perTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rs2SpecLabel);
            this.Controls.Add(this.channelTextBox);
            this.Controls.Add(this.rs2CondLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.packetNumTextBox);
            this.Controls.Add(this.label6);
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RS2";
            this.Text = "Receiver Sensitivity 2M";
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
        private System.Windows.Forms.Label rs2SpecLabel;
        private System.Windows.Forms.TextBox channelTextBox;
        private System.Windows.Forms.Label rs2CondLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox packetNumTextBox;
        private System.Windows.Forms.Label label6;
    }
}