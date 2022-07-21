namespace KingForms.Demo.Forms;

partial class MainFormEmpty
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
            this.uxTextBox = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // uxTextBox
            // 
            this.uxTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uxTextBox.Location = new System.Drawing.Point(0, 0);
            this.uxTextBox.Name = "uxTextBox";
            this.uxTextBox.Size = new System.Drawing.Size(800, 450);
            this.uxTextBox.TabIndex = 1;
            this.uxTextBox.Text = "TEXT";
            this.uxTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainFormEmpty
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.uxTextBox);
            this.Name = "MainFormEmpty";
            this.Text = "Main Form - Demo Application";
            this.ResumeLayout(false);

    }

    #endregion

    private Label uxTextBox;
}