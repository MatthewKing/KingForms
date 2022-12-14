namespace KingForms.Demo.Forms;

partial class SplashFormWithoutProgress
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
            this.uxLoadingLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // uxLoadingLabel
            // 
            this.uxLoadingLabel.AutoSize = true;
            this.uxLoadingLabel.Location = new System.Drawing.Point(20, 20);
            this.uxLoadingLabel.Margin = new System.Windows.Forms.Padding(10);
            this.uxLoadingLabel.Name = "uxLoadingLabel";
            this.uxLoadingLabel.Size = new System.Drawing.Size(59, 15);
            this.uxLoadingLabel.TabIndex = 1;
            this.uxLoadingLabel.Text = "Loading...";
            // 
            // SplashFormWithoutProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 61);
            this.Controls.Add(this.uxLoadingLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 100);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 100);
            this.Name = "SplashFormWithoutProgress";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Splash Form - Demo Application";
            this.Load += new System.EventHandler(this.SplashFormWithoutProgress_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Label uxLoadingLabel;
}