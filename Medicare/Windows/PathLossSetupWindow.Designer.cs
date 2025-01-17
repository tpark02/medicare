namespace Medicare.PathLoss
{
    partial class PathLossSetupWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PathLossSetupWindow));
            this.pathLossGrid = new System.Windows.Forms.DataGridView();
            this.writeButton = new System.Windows.Forms.Button();
            this.readButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.CH = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Freq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RF1_TX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RF1_RX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RF2_TX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RF2_RX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.pathLossGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // pathLossGrid
            // 
            this.pathLossGrid.AllowUserToAddRows = false;
            this.pathLossGrid.AllowUserToDeleteRows = false;
            this.pathLossGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.pathLossGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.pathLossGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CH,
            this.Freq,
            this.RF1_TX,
            this.RF1_RX,
            this.RF2_TX,
            this.RF2_RX});
            this.pathLossGrid.Location = new System.Drawing.Point(15, 95);
            this.pathLossGrid.Margin = new System.Windows.Forms.Padding(5);
            this.pathLossGrid.Name = "pathLossGrid";
            this.pathLossGrid.RowTemplate.Height = 23;
            this.pathLossGrid.Size = new System.Drawing.Size(1067, 718);
            this.pathLossGrid.TabIndex = 5;
            this.pathLossGrid.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.editingControlShowing);
            // 
            // writeButton
            // 
            this.writeButton.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.writeButton.Location = new System.Drawing.Point(840, 15);
            this.writeButton.Margin = new System.Windows.Forms.Padding(5);
            this.writeButton.Name = "writeButton";
            this.writeButton.Size = new System.Drawing.Size(222, 70);
            this.writeButton.TabIndex = 9;
            this.writeButton.Text = "Write";
            this.writeButton.UseVisualStyleBackColor = true;
            this.writeButton.Click += new System.EventHandler(this.writeButton_Click);
            // 
            // readButton
            // 
            this.readButton.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.readButton.Location = new System.Drawing.Point(557, 13);
            this.readButton.Margin = new System.Windows.Forms.Padding(5);
            this.readButton.Name = "readButton";
            this.readButton.Size = new System.Drawing.Size(222, 70);
            this.readButton.TabIndex = 8;
            this.readButton.Text = "Read";
            this.readButton.UseVisualStyleBackColor = true;
            this.readButton.Click += new System.EventHandler(this.readButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadButton.Location = new System.Drawing.Point(291, 15);
            this.loadButton.Margin = new System.Windows.Forms.Padding(5);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(222, 70);
            this.loadButton.TabIndex = 7;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveButton.Location = new System.Drawing.Point(15, 15);
            this.saveButton.Margin = new System.Windows.Forms.Padding(5);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(222, 70);
            this.saveButton.TabIndex = 6;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // CH
            // 
            this.CH.HeaderText = "CH";
            this.CH.Name = "CH";
            this.CH.ReadOnly = true;
            this.CH.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Freq
            // 
            this.Freq.HeaderText = "Freq";
            this.Freq.Name = "Freq";
            this.Freq.ReadOnly = true;
            this.Freq.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // RF1_TX
            // 
            this.RF1_TX.HeaderText = "RF1_TX";
            this.RF1_TX.Name = "RF1_TX";
            this.RF1_TX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // RF1_RX
            // 
            this.RF1_RX.HeaderText = "RF1_RX";
            this.RF1_RX.Name = "RF1_RX";
            this.RF1_RX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // RF2_TX
            // 
            this.RF2_TX.HeaderText = "RF2_TX";
            this.RF2_TX.Name = "RF2_TX";
            this.RF2_TX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.RF2_TX.Visible = false;
            // 
            // RF2_RX
            // 
            this.RF2_RX.HeaderText = "RF2_RX";
            this.RF2_RX.Name = "RF2_RX";
            this.RF2_RX.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.RF2_RX.Visible = false;
            // 
            // PathLossSetupWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1096, 827);
            this.Controls.Add(this.pathLossGrid);
            this.Controls.Add(this.writeButton);
            this.Controls.Add(this.readButton);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.saveButton);
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PathLossSetupWindow";
            this.Text = "PathLoss";
            ((System.ComponentModel.ISupportInitialize)(this.pathLossGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView pathLossGrid;
        private System.Windows.Forms.Button writeButton;
        private System.Windows.Forms.Button readButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn CH;
        private System.Windows.Forms.DataGridViewTextBoxColumn Freq;
        private System.Windows.Forms.DataGridViewTextBoxColumn RF1_TX;
        private System.Windows.Forms.DataGridViewTextBoxColumn RF1_RX;
        private System.Windows.Forms.DataGridViewTextBoxColumn RF2_TX;
        private System.Windows.Forms.DataGridViewTextBoxColumn RF2_RX;
    }
}