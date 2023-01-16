using KingForms.Demo.Forms;

namespace KingForms.Demo;

internal class DemoLifetime
{
    public static Task<object> Initialize(IProgress<ApplicationProgress> progress)
    {
        return DemoInitializer.RunAsync(progress);
    }

    public static void PreRun(DemoInitializationResult context, ApplicationScope scope)
    {
        MessageBox.Show($"This occurs BEFORE the application runs, but still has access to the initialized context. As proof, it can access the result ID: {context.Id}.");
    }

    public static void Run(DemoInitializationResult context, ApplicationScope scope)
    {
        scope.AddForm(new MainForm1(context), visible: true);
        scope.AddForm(new MainForm2(context), visible: true);
    }

    public static void PostRun(DemoInitializationResult context, ApplicationScope scope)
    {
        MessageBox.Show($"This occurs AFTER the application runs, but still has access to the initialized context. As proof, it can access the result ID: {context.Id}.");
    }
}
