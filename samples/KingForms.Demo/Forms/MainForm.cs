namespace KingForms.Demo.Forms;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        uxTextBox.Text = "MAIN";
    }

    public void SetText(string value) => uxTextBox.Text = value;
}
