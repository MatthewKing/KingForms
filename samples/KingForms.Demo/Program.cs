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
            .WithMainForm<MainForm>()
            .Run();

        // Async initialization, a splash form, and multiple main forms:
        ApplicationContextBuilder.Create()
            .WithInitializer<DemoInitializer>()
            .WithSplashForm(() => new SplashForm(ProgressBarStyle.Continuous))
            .WithMainForms((DemoInitializationResult result) => new Form[] {
                new MainForm1(result),
                new MainForm2(result),
                new ComboBoxDemoForm(),
            })
            .Run();

        // As above, but also with DI/IoC:
        ApplicationContextBuilder.Create()
            .WithInitializer<DemoInitializerWithDI>()
            .WithSplashForm(() => new SplashForm(ProgressBarStyle.Continuous))
            .WithMainForms<IServiceProvider>(services => new Form[] {
                services.GetService<MainForm1>(),
                services.GetService<MainForm2>(),
                services.GetService<ComboBoxDemoForm>(),
            })
            .Run();

        // Advanced example: A hidden form.
        ApplicationContextBuilder.Create()
            .OnStart(context =>
            {
                // This one is visible:
                var visibleForm = new MainForm();
                context.AttachForm(visibleForm, true);

                // This one is hidden:
                var hiddenForm = new MainForm();
                context.AttachForm(hiddenForm, false); // This one is hidden.

                // Wire up the hidden one to close when the visible one is closed:
                visibleForm.FormClosed += (s, e) => hiddenForm.Close();
            })
            .Run();

        // Advanced example: Forms being created and shown in order.
        ApplicationContextBuilder.Create()
            .WithInitializer<DemoInitializerWithDI>()
            .WithSplashForm<SplashForm>()
            .OnStart<IServiceProvider>((services, context) =>
            {
                // Show forms in order: MainForm1, MainForm2, ComboBoxDemoForm:
                var mainForm1 = services.GetService<MainForm1>();
                context.AttachForm(mainForm1);
                mainForm1.FormClosed += (s, e) =>
                {
                    var mainForm2 = services.GetService<MainForm2>();
                    context.AttachForm(mainForm2);
                    mainForm2.FormClosed += (s, e) =>
                    {
                        var comboBoxDemoForm = services.GetService<ComboBoxDemoForm>();
                        context.AttachForm(comboBoxDemoForm);
                    };
                };
            })
            .Run();
    }
}
