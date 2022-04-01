namespace KingForms.Demo.Forms;

public partial class ComboBoxDemoForm : Form
{
    public ComboBoxDemoForm()
    {
        InitializeComponent();
    }

    private void ComboBoxDemoForm_Load(object sender, EventArgs e)
    {
        uxLabel.Text = null;
        uxComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        uxComboBox.SetDataSource(
            ComboBoxItem.Default("No value"),
            ComboBoxItem.ForValue(DateOnly.FromDateTime(DateTime.Now), "Today"),
            ComboBoxItem.ForValue(DateOnly.FromDateTime(DateTime.Now - TimeSpan.FromDays(7)), "One week ago"),
            ComboBoxItem.ForValue(DateOnly.FromDateTime(DateTime.Now - TimeSpan.FromDays(14)), "Two weeks ago"));
    }

    private void uxComboBox_SelectedValueChanged(object sender, EventArgs e)
    {
        uxLabel.Text = uxComboBox.GetSelectedValue<DateOnly?>()?.ToString();
    }

    private void uxResetButton_Click(object sender, EventArgs e)
    {
        uxComboBox.ResetSelectedValue();
    }
}
