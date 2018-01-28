using System;
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
using FirstFloor.ModernUI.Presentation;
using System.Linq;
using System.Windows.Media;
using System.Text.RegularExpressions;
using BaiduPanDownloadWpf.Infrastructure;

namespace BaiduPanDownloadWpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override ILoggerFacade CreateLogger()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Run records.log");
            if (File.Exists(filePath)) File.Delete(filePath);
            var file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(file, Encoding.UTF8) { AutoFlush = true };
            writer.WriteLine($"{UiStringResources.MWTitile} - {UiStringResources.Version}");
            writer.WriteLine($"{SystemInfo.Caption}: [{SystemInfo.Version}] {SystemInfo.OSArchitecture}");
            return new TextLogger(writer);
        }

        protected override DependencyObject CreateShell()
        {
            ServicePointManager.DefaultConnectionLimit = 99999;
            Application.Current.Exit += (sender, e) => OnExit();
            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledExceptionOccurred;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledExceptionOccurred;
            //return new SignWindow();
            return new MainWindow();
        }

        protected override void InitializeShell()
        {
            Initialize();
            Application.Current.MainWindow.Show();
        }

        protected override void InitializeModules()
        {
            Container.TryResolve<DownloadCoreModule>().Initialize();
            Logger.Log("Initialized DownloadCoreModule Module.", Category.Debug, Priority.Low);
        }


        private void Initialize()
        {
            if (!Directory.Exists(Common.UserDataSavePath)) Directory.CreateDirectory(Common.UserDataSavePath);
            Container.RegisterInstance(typeof(ILocalConfigInfo), LocalConfigInfo.Create());

            var tempTheme = StaticResources.Themes.FirstOrDefault(item => item.DisplayName == Container.Resolve<ILocalConfigInfo>().Theme)?.Source;
            if (tempTheme != null)
                AppearanceManager.Current.ThemeSource = tempTheme;

            var tempArr = Container.Resolve<ILocalConfigInfo>().Background?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (tempArr.IsNullOrEmpty() || tempArr.Length != 3) return;
            byte r, g, b;
            if (byte.TryParse(tempArr[0], out r) && byte.TryParse(tempArr[1], out g) && byte.TryParse(tempArr[2], out b))
                AppearanceManager.Current.AccentColor = Color.FromRgb(r, g, b);
        }
        private void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionEventArgs e)
        {
            CatchException(e.ExceptionObject as Exception);
        }
        private void OnDispatcherUnhandledExceptionOccurred(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            CatchException(e.Exception);
        }
        private void OnExit()
        {
            Container.TryResolve<ILocalConfigInfo>()?.Save();

            var mountUserRepository = Container.Resolve<IMountUserRepository>();
            var localDiskUser = mountUserRepository.FirstOrDefault();
            if (localDiskUser != null)
            {
                mountUserRepository.Save();
            }
            Logger.Log("The software has exited.", Category.Info, Priority.Medium);
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
                OnExit();
                _messageDialog = new MessageDialog(UiStringResources.MessageDialogTitle_Error, UiStringResources.MessageDialogContent_Crash);
                _messageDialog.ShowDialog();
            }
            Environment.Exit(-1);
        }
    }
}
