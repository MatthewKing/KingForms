using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KingForms;

public static class InstanceRestorationHelper
{
    public static void Restore(InstanceRestorationMethod method)
    {
        if (method is InstanceRestorationMethod.ShowMainWindow)
        {
            ActOnWindowHandle(hWnd =>
            {
                NativeMethods.ShowWindow(hWnd, 9); // SW_RESTORE
                NativeMethods.SetForegroundWindow(hWnd);
            });
        }
        else if (method is InstanceRestorationMethod.SendMessageToMainWindow)
        {
            ActOnWindowHandle(hWnd =>
            {
                MessageHelper.SendMessage(hWnd, Messages.RestoreInstance.Value);
            });
        }
    }

    private static void ActOnWindowHandle(Action<IntPtr> action)
    {
        try
        {
            var thisProcess = Process.GetCurrentProcess();
            var thisProcessId = thisProcess.Id;
            var thisProcessMainModuleFileName = thisProcess.MainModule.FileName;
            var existingProcess = Process.GetProcesses().FirstOrDefault(x =>
            {
                try
                {
                    return x.Id != thisProcessId
                        && x.MainModule.FileName == thisProcessMainModuleFileName;
                }
                catch
                {
                    return false;
                }
            });

            if (existingProcess != null && existingProcess.MainWindowHandle != default)
            {
                action.Invoke(existingProcess.MainWindowHandle);
            }
        }
        catch
        {

        }
    }

    private static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}
