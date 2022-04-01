namespace KingForms.Demo;

public partial class MainForm1 : Form
{
    public MainForm1(DemoContext context)
    {
        InitializeComponent();

        uxTextBox.Text = context.Data;
    }
}
