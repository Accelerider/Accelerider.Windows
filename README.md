# Introduction ![version](https://img.shields.io/badge/BaiduPanDownloadWpf-v0.2.0.79-orange.svg) ![build state](https://img.shields.io/badge/build-passing-brightgreen.svg) 
百度网盘**不限速**下载客户端，此项目源于[BaiduPanDownloadWinform](https://github.com/ResourceHunter/BaiduPanDownloadWinform)，旨在使用[WPF](https://msdn.microsoft.com/en-us/library/mt149842.aspx)技术，为其编写一个更为美观的界面，并且重新设计了软件的架构，使之在应对度娘多变的和谐策略的同时，为UI部分代码的复用提供了可能。此项目基于[MUI](https://github.com/firstfloorsoftware/mui)进行开发，并借助[Prism](https://github.com/PrismLibrary/Prism)框架实现了[界面](https://github.com/ResourceHunter/BaiduPanDownloadWpf/tree/master/BaiduPanDownloadWpf)与[业务逻辑](https://github.com/ResourceHunter/BaiduPanDownloadWpf/tree/master/BaiduPanDownloadWpf.Core)的分离。

![preview](/docs/images/preview.png)

# Getting Started
1. 下载代码，并使用Visual Studio（2015+）打开；
0. 选中整个解决方案并右键，选择生成解决方案（build / rebulid）；
0. 期间会自动还原NuGet packages，并可能占用一些时间，请耐心等待；
0. **注意**：从版本 `v0.2.0.79` 开始，用户密码将被加密传输（RSA），但公钥并没有被提供在开源代码中，所以目前本项目的代码仅供学习交流使用，不可以登入服务器；
0. 若想启动软件主界面，可以更改`BaiduPanDownloadWpf/Bootstrapper.cs`文件中的`CreateShell()`函数为以下形式：
```
        protected override DependencyObject CreateShell()
        {
            //return new SignWindow();
            return new MainWindow();
        }
```

0. 更多关于此项目的信息，请参考[Contributing](#Contributing)

# Screenshots

![downloading page](/docs/images/downloading_page.jpg)

![configure page](/docs/images/configure_page.png)

![downloaded page](/docs/images/downloaded_page.png)

![login page](/docs/images/login_page.png)

<h1 id="Contributing">Contributing</h1>


如果您对此项目有兴趣并想贡献自己的idea，欢迎通过以下方式参与：
- 加入我们：**开发者**QQ群 `622811874`（已更新）或**联系作者** `1844812067` - [Mrs4s](https://github.com/Mrs4s)、`787673395` - [Laplace's Demon](https://github.com/DingpingZhang)
    - 加群时，验证信息请填写任意**可证明编程能力**的判据，**非程**~~序员~~**勿扰**，参考该回复[Issue#59 - `DingpingZhang`](https://github.com/ResourceHunter/BaiduPanDownloadWpf/issues/59)；
    - 加作者联系方式时，请注明 `参与Github项目开发`；如果你是用户，请通过[Issues](https://github.com/ResourceHunter/BaiduPanDownloadWpf/issues)与作者交流，感谢你的支持；
    - 不符合以上要求者，均不会通过验证。
- 在[Issues](https://github.com/ResourceHunter/BaiduPanDownloadWpf/issues)板块进行留言，分享你的建议或需求；
- Fork此项目，并通过[Pull requests](https://github.com/ResourceHunter/BaiduPanDownloadWpf/pulls)贡献你的代码。

# Acknowledgements

- Modern UI for WPF (MUI): [https://github.com/firstfloorsoftware/mui](https://github.com/firstfloorsoftware/mui) (Adapted)
- MahApps.Metro: [https://github.com/MahApps/MahApps.Metro](https://github.com/MahApps/MahApps.Metro)
- Unity Container: [https://github.com/unitycontainer/unity](https://github.com/unitycontainer/unity)
- Prism: [https://github.com/PrismLibrary/Prism](https://github.com/PrismLibrary/Prism)
- Source of software icon: [https://www.iconfinder.com/icons/314711/cloud_download_icon#size=128](https://www.iconfinder.com/icons/314711/cloud_download_icon#size=128)