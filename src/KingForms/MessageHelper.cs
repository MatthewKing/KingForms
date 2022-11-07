using System.Runtime.InteropServices;

namespace KingForms;

public static class MessageHelper
{
    public static void SendMessage(IntPtr hWnd, int messageId)
    {
        NativeMethods.PostMessage(hWnd, messageId, IntPtr.Zero, IntPtr.Zero);
    }

    public static void SendMessage(IntPtr hWnd, string message)
    {
        SendMessage(hWnd, GetMessageId(message));
    }

    public static int GetMessageId(string message)
    {
        return NativeMethods.RegisterWindowMessage(message);
    }

    private static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int RegisterWindowMessage(string lpString);
    }
}
