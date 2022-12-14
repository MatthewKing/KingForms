namespace KingForms.Demo.Forms;

partial class SplashFormWithProgress
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
            this.uxProgressLabel = new System.Windows.Forms.Label();
            this.uxProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // uxProgressLabel
            // 
            this.uxProgressLabel.AutoSize = true;
            this.uxProgressLabel.Location = new System.Drawing.Point(20, 20);
            this.uxProgressLabel.Margin = new System.Windows.Forms.Padding(10);
            this.uxProgressLabel.Name = "uxProgressLabel";
            this.uxProgressLabel.Size = new System.Drawing.Size(93, 15);
            this.uxProgressLabel.TabIndex = 0;
            this.uxProgressLabel.Text = "PROGRESS_TEXT";
            // 
            // uxProgressBar
            // 
            this.uxProgressBar.Location = new System.Drawing.Point(20, 55);
            this.uxProgressBar.Margin = new System.Windows.Forms.Padding(10);
            this.uxProgressBar.Name = "uxProgressBar";
            this.uxProgressBar.Size = new System.Drawing.Size(344, 23);
            this.uxProgressBar.TabIndex = 1;
            // 
            // SplashForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 111);
            this.Controls.Add(this.uxProgressLabel);
            this.Controls.Add(this.uxProgressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 150);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 150);
            this.Name = "SplashForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Splash Form - Demo Application";
            this.Load += new System.EventHandler(this.SplashForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Label uxProgressLabel;
    private ProgressBar uxProgressBar;
}
