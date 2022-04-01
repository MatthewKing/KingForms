using KingForms.Demo.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace KingForms.Demo;

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        ApplicationConfiguration.Initialize();

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
