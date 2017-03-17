using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using BaiduPanDownloadWpf.Core;
using Prism.Unity;
using System.Windows;
using System.Windows.Threading;
using Prism.Logging;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using BaiduPanDownloadWpf.Assets;
using BaiduPanDownloadWpf.Views.Dialogs;

namespace BaiduPanDownloadWpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return new SignWindow();
            //return new MainWindow();
        }

        protected override void InitializeShell()
        {
            ServicePointManager.DefaultConnectionLimit = 99999;
            Application.Current.Exit += OnExit;
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledExceptionOccurred;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledExceptionOccurred;
            Application.Current.MainWindow.Show();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Container.Resolve<ILocalConfigInfo>().Save();

            var mountUserRepository = Container.Resolve<IMountUserRepository>();
            var localDiskUser = mountUserRepository.FirstOrDefault();
            if (localDiskUser != null)
            {
                mountUserRepository.Save();
            }
        }

        protected override void InitializeModules()
        {
            Container.TryResolve<DownloadCoreModule>().Initialize();
            Logger.Log("Initialize DownloadCoreModule Module.", Category.Debug, Priority.Low);
        }

        protected override ILoggerFacade CreateLogger()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Run records.log");
            if (File.Exists(filePath)) File.Delete(filePath);
            var file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(file, Encoding.UTF8) { AutoFlush = true };
            writer.WriteLine($"{UiStringResources.MWTitile} - {UiStringResources.Version}");
            return new TextLogger(writer);
        }

        private void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionEventArgs e)
        {
            CatchException(e.ExceptionObject as Exception);
        }

        private void OnDispatcherUnhandledExceptionOccurred(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CatchException(e.Exception);
        }

        private MessageDialog _messageDialog;
        private void CatchException(Exception error)
        {
            if (error == null) return;
            var message = $"Exception: {error.GetType().Name}, Message: {error.Message}, StackTrace: {Environment.NewLine}{error.StackTrace}{Environment.NewLine}";
            Logger.Log(message, Category.Exception, Priority.High);
            Logger.Log("The software has crashed.", Category.Info, Priority.High);
            if (_messageDialog == null)
            {
                _messageDialog = new MessageDialog(UiStringResources.MessageDialogTitle_Error, UiStringResources.MessageDialogContent_Crash);
                _messageDialog.ShowDialog();
            }
            Environment.Exit(-1);
        }
    }
}
