# Introduction
百度网盘**不限速**下载客户端，此项目源于[BaiduPanDownloadWinform](https://github.com/ResourceHunter/BaiduPanDownloadWinform)，旨在使用[WPF](https://msdn.microsoft.com/en-us/library/mt149842.aspx)技术，为其编写一个更为美观的界面，并且重新设计了软件的架构，使之在应对度娘多变的和谐策略的同时，为UI部分代码的复用提供了可能。此项目基于[MUI](https://github.com/firstfloorsoftware/mui)进行开发，并借助[Prism](https://github.com/PrismLibrary/Prism)框架实现了[界面](https://github.com/ResourceHunter/BaiduPanDownloadWpf/tree/master/BaiduPanDownloadWpf)与[业务逻辑](https://github.com/ResourceHunter/BaiduPanDownloadWpf/tree/master/BaiduPanDownloadWpf.Core)的分离。

![preview](/docs/images/preview.png)

# Notice
本项目前仍处于开发阶段，功能尚未更新完全，欢迎更多的贡献者参与进来，目前已完成的功能如下：
- 已注册并绑定百度账号的用户登入（暂不支持新用户注册）；
- 浏览网盘文件及删除操作；

P.S.: 不要笑，我知道以上这两个功能没啥卵用，只是为了给大家先看一个界面预览而已，完整功能的版本会在近期发布。

# Getting Started
1. 下载代码，并使用Visual Studio（2015+）打开；
2. 选中整个解决方案并右键，选择生成解决方案（build）；
3. 期间会自动还原NuGet packages，并可能占用一些时间，请耐心等待。
4. 运行项目后，老用户即可在Settings --> Login页面，用先前在老版本上注册过的账号登入，并回到file explorer/HOME页面浏览/删除文件。

![login illustration](/docs/images/login_page.png)

# Contribute
如果您对此项目有兴趣并想贡献自己的idea，欢迎通过以下方式参与：
- 加入**QQ群**：553683933或**联系**：1844812067，787673395；
- 在[Issues](https://github.com/ResourceHunter/BaiduPanDownloadWpf/issues)板块进行留言，分享你的建议或需求；
- Fork此项目，并通过pull request贡献你的代码。

# Screenshots

![configure page](/docs/images/configure_page.png)

![downloading page](/docs/images/downloading_page.png)
P.S.: 上图中下载速度为**demo演示**时的**随机数**，并非实际下载速度，不要引起恐慌。

![downloaded page](/docs/images/downloaded_page.png)
