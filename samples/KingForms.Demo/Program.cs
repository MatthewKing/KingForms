using KingForms.Demo.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace KingForms.Demo;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Splash form:
        Application.Run(new ApplicationContextBuilder()
            .WithSplashForm(new SplashForm(ProgressBarStyle.Marquee), TimeSpan.FromSeconds(5), "Loading...")
            .SingleMainForm(new MainFormEmpty()));

        // Full example, with simple initializer:
        Application.Run(new ApplicationContextBuilder()
            .WithInitializer(new ApplicationInitializerSimple())
            .WithSplashForm(() => new SplashForm(ProgressBarStyle.Continuous))
            .MultipleMainForms((DemoContext context) => new Form[] {
                new MainForm1(context),
                new MainForm2(context),
                new ComboBoxDemoForm(),
            }));

        // Full example, with DI / IoC:
        Application.Run(new ApplicationContextBuilder()
            .WithInitializer(new ApplicationInitializerUsingDependencyInjectionContainer())
            .WithSplashForm(() => new SplashForm(ProgressBarStyle.Continuous))
            .MultipleMainForms<IServiceProvider>(services => new Form[] {
                services.GetService<MainForm1>(),
                services.GetService<MainForm2>(),
                services.GetService<ComboBoxDemoForm>(),
            }));
    }
}
