using KingForms;
using KingForms.Demo;
using KingForms.Demo.Forms;
using Microsoft.Extensions.DependencyInjection;

static class Program
{
    [STAThread]
    public static void Main()
    {
        // Simple example with async initialization and a splash form:
        ApplicationContextBuilder.Create()
            .WithSplashForm<SplashFormWithProgress>(DemoInitializer.RunAsync)
            .WithMainForm<MainForm>()
            .Run();

        // As above, but with a very simple splash form without progress indicators.
        ApplicationContextBuilder.Create()
            .WithSplashForm<SplashFormWithoutProgress>(DemoInitializer.RunAsync)
            .WithMainForm<MainForm>()
            .Run();

        // Simple example with both initialization and cleanup:
        ApplicationContextBuilder.Create()
            .WithSplashForm<SplashFormWithProgress>(DemoInitializer.RunAsync)
            .WithMainForm<MainForm>()
            .WithCleanupForm<SplashFormWithProgress>(DemoCleanup.RunAsync)
            .Run();

        // Async initialization, a splash form, and multiple main forms:
        ApplicationContextBuilder.Create()
            .WithSplashForm(() => new SplashFormWithProgress(ProgressBarStyle.Continuous), DemoInitializer.RunAsync)
            .WithMainForms((DemoInitializationResult result) => new Form[] {
                new MainForm1(result),
                new MainForm2(result),
                new ComboBoxDemoForm(),
            })
            .Run();

        // As above, but also with DI/IoC:
        ApplicationContextBuilder.Create()
            .WithSplashForm(() => new SplashFormWithProgress(ProgressBarStyle.Continuous), DemoInitializer.RunAsync)
            .WithMainForms<IServiceProvider>(services => new Form[] {
                services.GetService<MainForm1>(),
                services.GetService<MainForm2>(),
                services.GetService<ComboBoxDemoForm>(),
            })
            .Run();

        // A single-instance app:
        ApplicationContextBuilder.Create()
            .WithMainForm<MainForm>()
            .RestrictToSingleInstance("example-mutex-name")
            .Run();

        // A single-instance app that shows a message if a second instance is opened:
        ApplicationContextBuilder.Create()
            .WithMainForm<MainForm>()
            .RestrictToSingleInstance("example-mutex-name", () => MessageBox.Show("App is already running"))
            .Run();

        // A single-instance app that tries to restore the first instance's main window if a second instance is opened:
        ApplicationContextBuilder.Create()
            .WithMainForm<MainForm>()
            .RestrictToSingleInstance("example-mutex-name", InstanceRestorationMethod.ShowMainWindow)
            .Run();

        // A single-instance app that sends a custom restoration message to the first instance if a second instance is opened:
        ApplicationContextBuilder.Create()
            .WithMainForm<MainFormCustomRestoration>()
            .RestrictToSingleInstance("example-mutex-name", InstanceRestorationMethod.SendMessageToMainWindow)
            .Run();

        // Advanced example: Application lifecycle events:
        ApplicationContextBuilder.Create()
            .WithSplashForm(() => new SplashFormWithProgress(ProgressBarStyle.Continuous), DemoInitializer.RunAsync)
            .OnRun<DemoInitializationResult>(DemoLifetime.Run)
            .OnPreRun<DemoInitializationResult>(DemoLifetime.PreRun)
            .OnPostRun<DemoInitializationResult>(DemoLifetime.PostRun)
            .Run();

        // Advanced example: A hidden form.
        ApplicationContextBuilder.Create()
            .OnRun(context =>
            {
                // This one is visible:
                var visibleForm = new MainForm();
                context.AddForm(visibleForm, true);

                // This one is hidden:
                var hiddenForm = new MainForm();
                context.AddForm(hiddenForm, false); // This one is hidden.

                // Wire up the hidden one to close when the visible one is closed:
                visibleForm.FormClosed += (s, e) => hiddenForm.Close();
            })
            .Run();

        // Advanced example: Forms being created and shown in order.
        ApplicationContextBuilder.Create()
            .WithSplashForm<SplashFormWithProgress>(DemoInitializerWithDI.RunAsync)
            .OnRun<IServiceProvider>((services, context) =>
            {
                // Show forms in order: MainForm1, MainForm2, ComboBoxDemoForm:
                var mainForm1 = services.GetService<MainForm1>();
                context.AddForm(mainForm1);
                mainForm1.FormClosed += (s, e) =>
                {
                    var mainForm2 = services.GetService<MainForm2>();
                    context.AddForm(mainForm2);
                    mainForm2.FormClosed += (s, e) =>
                    {
                        var comboBoxDemoForm = services.GetService<ComboBoxDemoForm>();
                        context.AddForm(comboBoxDemoForm);
                    };
                };
            })
            .Run();
    }
}
