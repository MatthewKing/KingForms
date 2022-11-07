using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KingForms.SingleInstance;

internal static class SingleInstanceHelper
{
    public static void RestoreExistingInstance()
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
                NativeMethods.ShowWindow(existingProcess.MainWindowHandle, 9); // SW_RESTORE
                NativeMethods.SetForegroundWindow(existingProcess.MainWindowHandle);
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
