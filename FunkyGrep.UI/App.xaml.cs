using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FunkyGrep.UI.Services;
using FunkyGrep.UI.ViewModels;
using FunkyGrep.UI.Views;
using Mono.Options;
using MvvmDialogs;
using Prism.Ioc;

namespace FunkyGrep.UI
{
    public partial class App
    {
        StartupEventArgs _startupArgs;

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

                string initialDirectory = unprocessed.FirstOrDefault();

                if (this.MainWindow != null && this.MainWindow.DataContext is MainWindowViewModel viewModel)
                {
                    viewModel.Directory = initialDirectory;
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
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }
    }
}
