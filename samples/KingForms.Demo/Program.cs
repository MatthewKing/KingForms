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
            .WithInitializer<DemoInitializer>()
            .WithSplashForm<SplashForm>()
            .SingleMainForm<MainFormEmpty>()
            .Run();

        // Async initialization, a splash form, and multiple main forms:
        ApplicationContextBuilder.Create()
            .WithInitializer<DemoInitializer>()
            .WithSplashForm(new SplashForm(ProgressBarStyle.Continuous))
            .MultipleMainForms((DemoInitializationResult result) => new Form[] {
                new MainForm1(result),
                new MainForm2(result),
                new ComboBoxDemoForm(),
            })
            .Run();

        // As above, but also with DI/IoC:
        ApplicationContextBuilder.Create()
            .WithInitializer<DemoInitializerWithDI>()
            .WithSplashForm(() => new SplashForm(ProgressBarStyle.Continuous))
            .MultipleMainForms<IServiceProvider>(services => new Form[] {
                services.GetService<MainForm1>(),
                services.GetService<MainForm2>(),
                services.GetService<ComboBoxDemoForm>(),
            })
            .Run();

        // Advanced example, with forms being shown in order:
        ApplicationContextBuilder.Create()
            .WithInitializer<DemoInitializer>()
            .WithSplashForm<SplashForm>()
            .CustomRunAction<DemoInitializationResult>((initializationResult, context) =>
            {
                // Show forms in order: MainForm1, MainForm2, ComboBoxDemoForm:
                var mainForm1 = new MainForm1(initializationResult);
                context.AttachForm(mainForm1, true);
                mainForm1.FormClosed += (s, e) =>
                {
                    var mainForm2 = new MainForm2(initializationResult);
                    context.AttachForm(mainForm2, true);
                    mainForm2.FormClosed += (s, e) =>
                    {
                        var comboBoxDemoForm = new ComboBoxDemoForm();
                        context.AttachForm(comboBoxDemoForm, true);
                    };
                };
            })
            .Run();
    }
}
