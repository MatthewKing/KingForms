namespace KingForms.Demo.Forms;

public partial class MainForm2 : Form
{
    public MainForm2(DemoContext context)
    {
        InitializeComponent();

        uxTextBox.Text = context.Assets;
    }
}
