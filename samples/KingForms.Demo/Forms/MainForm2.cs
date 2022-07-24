namespace KingForms.Demo.Forms;

public partial class MainForm2 : Form
{
    public MainForm2(DemoInitializationResult context)
    {
        InitializeComponent();

        uxTextBox.Text = context.Assets;
    }
}
