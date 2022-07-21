using KingForms.Demo.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace KingForms.Demo;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Simple example:
        var applicationContext1 = new ApplicationContextBuilder()
            .WithSplashForm(() => new SplashForm(), new ApplicationInitializerSimple())
            .MultipleMainForms((DemoContext context) => new Form[] {
                new MainForm1(context),
                new MainForm2(context),
                new ComboBoxDemoForm(),
            })
            .Build();

        Application.Run(applicationContext1);

        // Example using DI / IoC:
        var applicationContext2 = new ApplicationContextBuilder()
            .WithSplashForm(() => new SplashForm(), new ApplicationInitializerUsingDependencyInjectionContainer())
            .MultipleMainForms<IServiceProvider>(services => new Form[] {
                services.GetService<MainForm1>(),
                services.GetService<MainForm2>(),
                services.GetService<ComboBoxDemoForm>(),
            })
            .Build();

        Application.Run(applicationContext2);
    }
}
