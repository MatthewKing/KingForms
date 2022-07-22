using KingForms.Demo.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace KingForms.Demo;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Async initialization and a splash form:
        Application.Run(new ApplicationContextBuilder()
            .WithInitializer<DemoInitializer>()
            .WithSplashForm<SplashForm>()
            .SingleMainForm<MainFormEmpty>());

        // Async initialization, a splash form, and multiple main forms:
        Application.Run(new ApplicationContextBuilder()
            .WithInitializer<DemoInitializer>()
            .WithSplashForm(new SplashForm(ProgressBarStyle.Continuous))
            .MultipleMainForms((DemoContext context) => new Form[] {
                new MainForm1(context),
                new MainForm2(context),
                new ComboBoxDemoForm(),
            }));

        // As above, but also with DI/IoC:
        Application.Run(new ApplicationContextBuilder()
            .WithInitializer<DemoInitializerWithDI>()
            .WithSplashForm(() => new SplashForm(ProgressBarStyle.Continuous))
            .MultipleMainForms<IServiceProvider>(services => new Form[] {
                services.GetService<MainForm1>(),
                services.GetService<MainForm2>(),
                services.GetService<ComboBoxDemoForm>(),
            }));

        // Lifecycle events:
        Application.Run(new ApplicationContextBuilder()
            .OnStarting(() => MessageBox.Show("Starting app"))
            .OnStopping(() => MessageBox.Show("Stopping app"))
            .WithInitializer<DemoInitializer>()
            .WithSplashForm<SplashForm>()
            .SingleMainForm<MainFormEmpty>());

        // Custom form logic:
        Application.Run(new ApplicationContextBuilder()
            .WithInitializer<DemoInitializer>()
            .WithSplashForm<SplashForm>()
            .CustomMainForms<DemoContext>((context, launcher) =>
            {
                var hiddenForm = new MainForm1(context);
                launcher.Launch(hiddenForm, false);
                var visibleForm = new MainForm1(context);
                visibleForm.FormClosed += (s, e) => hiddenForm.Close();
                launcher.Launch(visibleForm, true);
            }));
    }
}
