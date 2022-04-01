namespace KingForms.Demo.Forms;

partial class ComboBoxDemoForm
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
            this.uxComboBox = new System.Windows.Forms.ComboBox();
            this.uxLabel = new System.Windows.Forms.Label();
            this.uxResetButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // uxComboBox
            // 
            this.uxComboBox.FormattingEnabled = true;
            this.uxComboBox.Location = new System.Drawing.Point(20, 20);
            this.uxComboBox.Margin = new System.Windows.Forms.Padding(10);
            this.uxComboBox.Name = "uxComboBox";
            this.uxComboBox.Size = new System.Drawing.Size(201, 23);
            this.uxComboBox.TabIndex = 0;
            this.uxComboBox.SelectedValueChanged += new System.EventHandler(this.uxComboBox_SelectedValueChanged);
            // 
            // uxLabel
            // 
            this.uxLabel.AutoSize = true;
            this.uxLabel.Location = new System.Drawing.Point(241, 23);
            this.uxLabel.Margin = new System.Windows.Forms.Padding(10);
            this.uxLabel.Name = "uxLabel";
            this.uxLabel.Size = new System.Drawing.Size(40, 15);
            this.uxLabel.TabIndex = 1;
            this.uxLabel.Text = "LABEL";
            // 
            // uxResetButton
            // 
            this.uxResetButton.Location = new System.Drawing.Point(146, 56);
            this.uxResetButton.Name = "uxResetButton";
            this.uxResetButton.Size = new System.Drawing.Size(75, 23);
            this.uxResetButton.TabIndex = 2;
            this.uxResetButton.Text = "Reset";
            this.uxResetButton.UseVisualStyleBackColor = true;
            this.uxResetButton.Click += new System.EventHandler(this.uxResetButton_Click);
            // 
            // ComboBoxDemoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.uxResetButton);
            this.Controls.Add(this.uxLabel);
            this.Controls.Add(this.uxComboBox);
            this.Name = "ComboBoxDemoForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Text = "Combo Box Example - Demo Application";
            this.Load += new System.EventHandler(this.ComboBoxDemoForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private ComboBox uxComboBox;
    private Label uxLabel;
    private Button uxResetButton;
}
