using System;

namespace BaiduPanDownloadWpf.Infrastructure
{
    /// <summary>
    /// An enum is used to mark the file type.
    /// </summary>
    public enum FileTypeEnum
    {
        /// <summary>
        /// Unknown type.
        /// </summary>
        OtherType,

        /// <summary>
        /// 
        /// </summary>
        ApkType,
        DocType,
        ExeType,
        FolderType,
        ImgType,
        MixFileType,
        MusicType,
        PdfType,
        PptType,
        RarType,
        TorrentType,
        TxtType,
        VideoType,
        XlsType,
    }

    /// <summary>
    /// An enum is used to indicate the status of the download task.
    /// </summary>
    [Flags]
    public enum DownloadStateEnum
    {
        /// <summary>
        /// The task has been created. Start state. Can be converted to <see cref="Waiting"/>.
        /// </summary>
        Created = 0,

        /// <summary>
        /// The task created, waiting to start. Can be converted to <see cref="Downloading"/> or <see cref="Canceled"/> or <see cref="Paused"/>.
        /// </summary>
        Waiting = 1,

        /// <summary>
        /// The task is being downloaded. Can be converted to <see cref="Paused"/> or <see cref="Completed"/> or <see cref="Canceled"/> or <see cref="Faulted"/>.
        /// </summary>
        Downloading = 2,

        /// <summary>
        /// The task is Paused. Can be converted to <see cref="Downloading"/> or <see cref="Canceled"/> or <see cref="Waiting"/>.
        /// </summary>
        Paused = 4,

        /// <summary>
        /// The task has been completed. End state.
        /// </summary>
        Completed = 8,

        /// <summary>
        /// The task has been canceled. End state.
        /// </summary>
        Canceled = 16,

        /// <summary>
        /// The task failed. End state.
        /// </summary>
        Faulted = 32
    }

    /// <summary>
    /// The unit of size.
    /// </summary>
    public enum SizeUnitEnum
    {
        /// <summary>
        /// Byte.
        /// </summary>
        B,

        /// <summary>
        /// K byte.
        /// </summary>
        K,

        /// <summary>
        /// M byte.
        /// </summary>
        M,

        /// <summary>
        /// G byte.
        /// </summary>
        G,

        /// <summary>
        /// T byte.
        /// </summary>
        T,

        /// <summary>
        /// P byte.
        /// </summary>
        P,
    }

    /// <summary>
    /// The result of the login.
    /// </summary>
    public enum ClientLoginStateEnum
    {
        /// <summary>
        /// 登录完成
        /// </summary>
        Success = 0,
        /// <summary>
        /// 不存在的用户
        /// </summary>
        NonUser = 1,
        /// <summary>
        /// 密码错误
        /// </summary>
        PasswordError = 2,
        /// <summary>
        /// 被封禁
        /// </summary>
        Baned = 5,
        /// <summary>
        /// 未知错误
        /// </summary>
        OtherError = 4
    }

    /// <summary>
    /// 下载方法
    /// </summary>
    public enum DownloadMethodEnum
    {
        /// <summary>
        /// 直接下载
        /// </summary>
        DirectDownload = 1,
        /// <summary>
        /// 中转下载
        /// </summary>
        JumpDownload = 2,
        /// <summary>
        /// APPID下载
        /// </summary>
        AppidDownload = 3
    }

    /// <summary>
    /// 登录状态
    /// </summary>
    public enum LoginStateEnum
    {
        /// <summary>
        /// 登录完成,无错误
        /// </summary>
        Success = 0,
        /// <summary>
        /// 系统错误
        /// </summary>
        SystemError = -1,
        /// <summary>
        /// 账号格式不正确
        /// </summary>
        UserNameError = 1,
        /// <summary>
        /// 账号不存在
        /// </summary>
        UserNotExists = 2,
        /// <summary>
        /// 验证码不存在或已过期
        /// </summary>
        VerifyCodeNotExistsOrTimeout = 3,
        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        UserNameOrPasswordError = 4,
        /// <summary>
        /// 验证码错误
        /// </summary>
        VerifyCodeError = 6,
        /// <summary>
        /// 密码错误
        /// </summary>
        PasswordError = 7,
        /// <summary>
        /// 帐号因安全问题已被限制登录
        /// </summary>
        SafeError = 16,
        /// <summary>
        /// 未输入验证码
        /// </summary>
        NoInputVerifyCode = 257,
        /// <summary>
        /// 系统正在升级,无法登陆
        /// </summary>
        SystemUpdate = 100027,
        /// <summary>
        /// 没有权限登录
        /// </summary>
        NoPermission = 21,

    }

    /// <summary>
    /// The result of the register.
    /// </summary>
    public enum RegisterStateEnum
    {
        /// <summary>
        /// 注册完成
        /// </summary>
        Completed,
        /// <summary>
        /// 已有账户
        /// </summary>
        ExistingAccount,
        /// <summary>
        /// 未知错误
        /// </summary>
        OtherError,
        /// <summary>
        /// 最大注册人数
        /// </summary>
        Maximum
    }

    /// <summary>
    /// The result of the share.
    /// </summary>
    public enum ShareStateEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 文件被删除
        /// </summary>
        Deleted = 9,
        /// <summary>
        /// 分享失败
        /// </summary>
        Failure = 1,
        /// <summary>
        /// 审核未通过
        /// </summary>
        Notpassed = 4
    }

    public enum LanguageEnum
    {
        Chinese,
        English,
        Japanese
    }
}
