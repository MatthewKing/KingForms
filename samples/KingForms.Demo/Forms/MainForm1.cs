namespace KingForms.Demo.Forms;

public partial class MainForm1 : Form
{
    public MainForm1(DemoInitializationResult context)
    {
        InitializeComponent();

        uxTextBox.Text = context.Data;
    }
}
