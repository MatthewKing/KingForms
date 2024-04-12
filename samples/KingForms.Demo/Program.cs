using KingForms;
using KingForms.Demo;
using KingForms.Demo.Forms;
using Microsoft.Extensions.DependencyInjection;

static class Program
{
    [STAThread]
    public static void Main()
    {
        // The simplest possible example: An app with a single form.
        ApplicationContextBuilder.Create()
            .WithMainForm<MainForm>()
            .Run();

        // Another very simple example. An app with a splash form and then a main form.
        ApplicationContextBuilder.Create()
            .WithSplashForm<SplashFormWithoutProgress>()
            .WithMainForm<MainForm>()
            .Run();

        // As above, but the splash form now performs some background initialization. This doesn't impact the main form, though.
        ApplicationContextBuilder.Create()
            .WithSplashForm<SplashFormWithProgress>(DemoInitializer.RunAsync)
            .WithMainForm<MainForm>()
            .Run();

        // As above, but the splash form now passes its initialized state to the main form.
        ApplicationContextBuilder.Create()
            .WithSplashForm<SplashFormWithProgress>(DemoInitializer.RunAsync)
            .WithMainForm((DemoInitializationResult result) => new MainForm1(result))
            .Run();

        // As above, but the splash form is also hidden.
        ApplicationContextBuilder.Create()
            .WithSplashForm<SplashFormWithProgress>(DemoInitializer.RunAsync, false)
            .WithMainForm((DemoInitializationResult result) => new MainForm1(result))
            .Run();

        // Multiple main forms:
        ApplicationContextBuilder.Create()
            .WithMainForms(() => [new MainForm(), new ComboBoxDemoForm()])
            .Run();

        // Putting it all together: Async initialization, a splash form, and multiple main forms.
        ApplicationContextBuilder.Create()
            .WithSplashForm(() => new SplashFormWithProgress(ProgressBarStyle.Continuous), DemoInitializer.RunAsync)
            .WithMainForms((DemoInitializationResult result) => [new MainForm1(result), new MainForm2(result)])
            .Run();

        // As above, but also with DI/IoC:
        ApplicationContextBuilder.Create()
            .WithSplashForm(() => new SplashFormWithProgress(ProgressBarStyle.Continuous), DemoInitializerWithDI.RunAsync)
            .WithMainForms<IServiceProvider>(services =>
            [
                services.GetService<MainForm1>(),
                services.GetService<MainForm2>(),
                services.GetService<ComboBoxDemoForm>(),
            ])
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

        // Advanced example: Controlling the "stage" manually.
        ApplicationContextBuilder.Create()
            .AddStage(stage =>
            {
                // This one is visible:
                var visibleForm = new MainForm();
                stage.AddForm(visibleForm, true);

                // This one is hidden:
                var hiddenForm = new MainForm();
                stage.AddForm(hiddenForm, false); // This one is hidden.

                // Wire up the hidden one to close when the visible one is closed:
                visibleForm.FormClosed += (s, e) => hiddenForm.Close();
            })
            .Run();

        // Advanced example: Forms being created and shown in order.
        ApplicationContextBuilder.Create()
            .WithSplashForm<SplashFormWithProgress>(DemoInitializerWithDI.RunAsync)
            .AddStage<IServiceProvider>((stage, services) =>
            {
                // Show forms in order: MainForm1, MainForm2, ComboBoxDemoForm:
                var mainForm1 = services.GetService<MainForm1>();
                stage.AddForm(mainForm1);
                mainForm1.FormClosed += (s, e) =>
                {
                    var mainForm2 = services.GetService<MainForm2>();
                    stage.AddForm(mainForm2);
                    mainForm2.FormClosed += (s, e) =>
                    {
                        var comboBoxDemoForm = services.GetService<ComboBoxDemoForm>();
                        stage.AddForm(comboBoxDemoForm);
                    };
                };
            })
            .Run();

        // Advance example: Multiple stages, passing state between them.
        ApplicationContextBuilder.Create()
            .AddStage(stage =>
            {
                var comboBoxDemoForm = new ComboBoxDemoForm();
                comboBoxDemoForm.FormClosed += (s, e) =>
                {
                    stage.SetCompletedState(new DemoStateDTO() { SelectedDate = comboBoxDemoForm.SelectedDate });
                };

                stage.AddForm(comboBoxDemoForm);
            })
            .AddStage<DemoStateDTO>((stage, state) =>
            {
                var mainForm = new MainForm();
                mainForm.SetText($"In the previous stage, you selected: {state.SelectedDate?.ToString() ?? "NOTHING"}");
                stage.AddForm(mainForm);
            })
            .Run();
    }
}
