using KingForms.Demo.Forms;

namespace KingForms.Demo;

internal class DemoLifetime
{
    public static Task<object> Initialize(IProgress<ApplicationProgress> progress)
    {
        return DemoInitializer.RunAsync(progress);
    }

    public static void PreRun(DemoInitializationResult state, ApplicationContextStage stage)
    {
        MessageBox.Show($"This occurs BEFORE the application runs, but still has access to the initialized context. As proof, it can access the result ID: {state.Id}.");
    }

    public static void Run(DemoInitializationResult state, ApplicationContextStage stage)
    {
        stage.AddForm(new MainForm1(state), visible: true);
        stage.AddForm(new MainForm2(state), visible: true);
    }

    public static void PostRun(DemoInitializationResult state, ApplicationContextStage stage)
    {
        MessageBox.Show($"This occurs AFTER the application runs, but still has access to the initialized context. As proof, it can access the result ID: {state.Id}.");
    }
}
