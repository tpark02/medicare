
partial class CTPReportWindow
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
        this.closeButton = new System.Windows.Forms.Button();
        this.reportTextBox = new System.Windows.Forms.RichTextBox();
        this.SuspendLayout();
        // 
        // closeButton
        // 
        this.closeButton.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.closeButton.Location = new System.Drawing.Point(963, 686);
        this.closeButton.Margin = new System.Windows.Forms.Padding(4);
        this.closeButton.Name = "closeButton";
        this.closeButton.Size = new System.Drawing.Size(161, 62);
        this.closeButton.TabIndex = 21;
        this.closeButton.Text = "Close";
        this.closeButton.UseVisualStyleBackColor = true;
        // 
        // reportTextBox
        // 
        this.reportTextBox.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.reportTextBox.Location = new System.Drawing.Point(18, 19);
        this.reportTextBox.Margin = new System.Windows.Forms.Padding(4);
        this.reportTextBox.Name = "reportTextBox";
        this.reportTextBox.Size = new System.Drawing.Size(1105, 656);
        this.reportTextBox.TabIndex = 20;
        this.reportTextBox.Text = "";
        // 
        // CTPReportWindow
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1143, 766);
        this.Controls.Add(this.closeButton);
        this.Controls.Add(this.reportTextBox);
        this.Name = "CTPReportWindow";
        this.Text = "CTPReportWindow";
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button closeButton;
    private System.Windows.Forms.RichTextBox reportTextBox;
}
