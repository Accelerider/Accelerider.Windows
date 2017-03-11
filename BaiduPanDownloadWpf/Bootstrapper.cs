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

namespace BaiduPanDownloadWpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return new MainWindow();
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
            var localDiskUserRepository = Container.Resolve<ILocalDiskUserRepository>();
            var localDiskUser = localDiskUserRepository.FirstOrDefault();
            if (localDiskUser != null)
            {
                localDiskUserRepository.Save(localDiskUser);
            }
        }

        protected override void InitializeModules()
        {
            Container.TryResolve<DownloadCoreModule>().Initialize();
            Logger.Log("Initialize DownloadCoreModule Module.", Category.Debug, Priority.Low);
        }

        protected override ILoggerFacade CreateLogger()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Run log.log");
            if (File.Exists(filePath)) File.Delete(filePath);
            var file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(file, Encoding.UTF8) { AutoFlush = true };
            writer.WriteLine(UiStringResources.MWTitile);
            return new TextLogger(writer);
            //return new TextLogger();
        }

        private void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionEventArgs e)
        {
            //var message = $"Message: {(e.ExceptionObject as Exception)?.Message}, StackTrace: {(e.ExceptionObject as Exception)?.StackTrace}";
            //Logger.Log(message, Category.Exception, Priority.High);
            // ------------------------------------------------------------------------------------------------------------------------------------
            this.CatchException(e.ExceptionObject as Exception);
        }

        private void OnDispatcherUnhandledExceptionOccurred(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var message = $"Message: {e.Exception.Message}, StackTrace: {Environment.NewLine}{e.Exception.StackTrace}{Environment.NewLine}";
            Logger.Log(message, Category.Exception, Priority.High);
            this.CatchException(e.Exception);
        }

        private void CatchException(Exception error)
        {
            if (error == null) return;

            var log = new StringBuilder();
            log.AppendLine("程序在运行时遇到不可预料的错误");
            log.AppendLine("=======追踪开始=======");
            log.AppendLine();
            log.AppendLine("Time: " + DateTime.Now);
            log.AppendLine("Type: " + error.GetType().Name);
            log.AppendLine("Version: 0.1.0.63");
            log.AppendLine("Message: " + error.Message);
            log.AppendLine("StackTrace: ");
            log.AppendLine(error.StackTrace);
            log.AppendLine();
            log.AppendLine("=======追踪结束=======");
            log.AppendLine("请将以上信息提供给开发者以供参考");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Error.log");
            try
            {
                File.WriteAllText(path, log.ToString());
            }
            catch (Exception e)
            {
                this.Logger.Log(e.ToString(), Category.Exception, Priority.High);
            }
            finally
            {
                Environment.Exit(-1);
            }
        }
    }
}
