using System.Runtime.InteropServices;

namespace KingForms.Demo.Forms;

public partial class MainFormCustomRestoration : Form
{
    public MainFormCustomRestoration()
    {
        InitializeComponent();
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Messages.RestoreInstance.Value)
        {
            uxTextBox.Text = $"Handled restore message at {DateTimeOffset.Now:O}";

            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
            }

            NativeMethods.SetForegroundWindow(Handle);
        }

        base.WndProc(ref m);
    }

    private static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
