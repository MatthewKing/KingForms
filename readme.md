# KingForms

This is an experimental project where I iterate on some ideas that will improve the WinForms developer experience.

This is not necessarily the final project name, and some ideas may get spun off to become their own project/package.

The best way to see the functionality is to have a look at the [samples](/samples/) directory.

## Quickstart

Get it on NuGet:

```
PM> Install-Package KingForms
```

## Improved application lifecycle / application context

One thing I'm trying to work on is the aesthetics of application lifecycle management and the ApplicationContext class.

I've added an `ApplicationContextBuilder` that allows some fancy application bootstrapping.

Here are some examples:

### An app that is restricted to a single instance:

```csharp
ApplicationContextBuilder.Create()
    .WithMainForm<MainForm>()
    .RestrictToSingleInstance("unique-mutex-name-for-your-app")
    .Run();
```

### An app that has some async initialization and a splash form:

```csharp
ApplicationContextBuilder.Create()
    .WithSplashForm<SplashForm, DemoInitializer>()
    .WithMainForm<MainForm>()
    .Run();
```

Note: SplashForm and DemoInitializer need to be implemented to suit your needs - have a look at the samples for examples.

### An app with a splash form, dependency injection, and multiple forms:

```csharp
ApplicationContextBuilder.Create()
    .WithSplashForm(() => new SplashForm(ProgressBarStyle.Continuous), new DemoInitializer())
    .WithMainForms<IServiceProvider>(services => new Form[] {
        services.GetService<MainForm1>(),
        services.GetService<MainForm2>(),
        services.GetService<MainForm3>(),
    })
    .Run();
```

## License and copyright

Copyright Matthew King.
Distributed under the [MIT License](http://opensource.org/licenses/MIT)
