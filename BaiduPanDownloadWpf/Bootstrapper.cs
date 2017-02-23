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
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new TextLogger(/*TextWriter*/);
        }

        private void OnUnhandledExceptionOccurred(object sender, UnhandledExceptionEventArgs ex)
        {
            var e = (Exception) ex.ExceptionObject;
            var log = new StringBuilder();
            log.AppendLine("程序在运行时遇到不可预料的错误");
            log.AppendLine("=======追踪开始=======");
            log.AppendLine();
            log.AppendLine("Time: " + DateTime.Now);
            log.AppendLine("Type: " + e.GetType().Name);
            log.AppendLine("Message: " + e.Message);
            log.AppendLine("Version: 0.0.10.59");
            log.AppendLine("StackTrace: ");
            log.AppendLine(e.StackTrace);
            log.AppendLine();
            log.AppendLine("=======追踪结束=======");
            log.AppendLine("请将以上信息提供给开发者以供参考");
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Error.log")))
            {
                try
                {
                    File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Error.log"));
                }
                catch
                {
                    throw e;
                }
            }
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Error.log"), log.ToString());
            throw e;
        }

        private void OnDispatcherUnhandledExceptionOccurred(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var log = new StringBuilder();
            log.AppendLine("程序在运行时遇到不可预料的错误");
            log.AppendLine("=======追踪开始=======");
            log.AppendLine();
            log.AppendLine("Time: " + DateTime.Now);
            log.AppendLine("Type: " + e.GetType().Name);
            log.AppendLine("Version: 0.0.10.59");
            log.AppendLine("Message: " + e.Exception==null?"无信息":e.Exception.Message);
            log.AppendLine("StackTrace: ");
            log.AppendLine(e.Exception==null?"无信息":e.Exception.StackTrace);
            log.AppendLine();
            log.AppendLine("=======追踪结束=======");
            log.AppendLine("请将以上信息提供给开发者以供参考");
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Error.log")))
            {
                try
                {
                    File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Error.log"));
                }
                catch
                {
                    throw e.Exception;
                }
            }
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Error.log"), log.ToString());
            throw e.Exception;
        }
    }
}
