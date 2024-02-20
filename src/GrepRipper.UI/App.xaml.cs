using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using GrepRipper.UI.Services;
using GrepRipper.UI.ViewModels;
using GrepRipper.UI.Views;
using Mono.Options;
using MvvmDialogs;
using Prism.Ioc;

namespace GrepRipper.UI;

public partial class App
{
    public static readonly string Version = Assembly.GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion ?? "<unknown version>";

    StartupEventArgs _startupArgs = null!;

    void HandleStartup(object sender, StartupEventArgs e)
    {
        this._startupArgs = e;
    }

    protected override void OnInitialized()
    {
        try
        {
            var optionSet = new OptionSet();
            List<string> unprocessed = optionSet.Parse(this._startupArgs.Args);

            string? initialDirectory = unprocessed.FirstOrDefault();

            if (this.MainWindow is { DataContext: MainWindowViewModel viewModel } &&
                !string.IsNullOrWhiteSpace(initialDirectory))
            {
                viewModel.Search.Directory = initialDirectory;
            }
        }
        catch (OptionException ex)
        {
            // ReSharper disable LocalizableElement
            MessageBox.Show(
                "Invalid arguments passed via command line. Details: " + ex,
                "Fatal error",
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
            // ReSharper restore LocalizableElement
        }
        catch (Exception ex)
        {
            // ReSharper disable LocalizableElement
            MessageBox.Show(
                "An unhandled error occurred. Details: " + ex,
                "Fatal error",
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
            // ReSharper restore LocalizableElement
        }

        base.OnInitialized();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance<IDialogService>(new DialogService());
        containerRegistry.RegisterInstance<IClipboardService>(new ClipboardService());
        containerRegistry.RegisterInstance<IProcessService>(new ProcessService());
        containerRegistry.RegisterInstance<IAppSettingsService>(new AppSettingsService());
        containerRegistry.RegisterInstance<IEditorFinderService>(new EditorFinderService());
    }

    protected override Window CreateShell()
    {
        return this.Container.Resolve<MainWindow>();
    }
}
