using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using System.IO;
using BaiduPanDownloadWpf.Core.NetWork.Packets;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Exceptions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Core.NetWork;
using System;
using BaiduPanDownloadWpf.Core.Download;
using Microsoft.Practices.Unity;
using System.Diagnostics;

namespace BaiduPanDownloadWpf.Core
{
    public class LocalDiskUserRepository : ModelBase, ILocalDiskUserRepository
    {
        //..
        private readonly string _userDataSavePath = Directory.GetCurrentDirectory() + @"\Users\";
        private static readonly List<ILocalDiskUser> _netDiskUserList = new List<ILocalDiskUser>();

        public LocalDiskUserRepository(IUnityContainer container) : base(container)
        {
            if (!Directory.Exists(_userDataSavePath))
            {
                Directory.CreateDirectory(_userDataSavePath);
            }
        }

        public ILocalDiskUser FirstOrDefault()
        {
            return _netDiskUserList.FirstOrDefault();
        }

        public async Task<ILocalDiskUser> SignInAsync(string userName, string password)
        {
            var dataServer = Server.DefaultServer;
            var json = JObject.Parse(await dataServer.SendPacketAsync(new LoginPacket()
            {
                Name = userName,
                Password = password,
            }));
            switch ((int)json["error"])
            {
                default:
                    var result = new LocalDiskUser(Container)
                    {
                        Token = (string)json["token"],
                        BoundAccount = (bool)json["cookies"],
                        DataServer = dataServer,
                        Name=userName,
                        Password=password
                    };
                    if (!Directory.Exists(Path.Combine(_userDataSavePath, userName)))
                    {
                        Directory.CreateDirectory(Path.Combine(_userDataSavePath, userName));
                    }
                    try
                    {
                        TaskManager.GetTaskManagerByLocalDiskUser(Container, result);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                    _netDiskUserList.Add(result);
                    var userPath = Path.Combine(_userDataSavePath, userName);
                    if (Directory.Exists(userPath))
                    {
                        try
                        {
                            var info = JObject.Parse(File.ReadAllText(Path.Combine(userPath, "Account.json")));
                            result.DownloadDirectory = (string) info["DownloadDirectory"];
                            result.ParallelTaskNumber = (int) info["ParallelTaskNumber"];
                            result.DownloadThreadNumber = ((int) info["DwonloadTheradNumber"]) > 32
                                ? 32
                                : (int) info["DwonloadTheradNumber"];
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.ToString());
                        }
                    }
                    return result;
                case 1:
                    throw new LoginException("用户不存在", ClientLoginStateEnum.NonUser);
                case 2:
                    throw new LoginException("密码错误", ClientLoginStateEnum.PasswordError);
                case 3:
                    throw new LoginException("异地登录", ClientLoginStateEnum.OtherError);
                case 4:
                    throw new LoginException("服务端出现未知错误", ClientLoginStateEnum.OtherError);
                case 5:
                    throw new LoginException("账号被封禁", ClientLoginStateEnum.Baned);
            }
        }

        public Task SignOutAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task SignUpAsync(string userName, string password)
        {
            var dataServer = Server.DefaultServer;
            var json = JObject.Parse(await dataServer.SendPacketAsync(new RegisterPacket()
            {
                Name = userName,
                Password = password,
            }));
            switch ((int)json["error"])
            {
                case 1:
                    throw new RegisterException("用户名已存在", RegisterStateEnum.ExistingAccount);
                case 2:
                    throw new RegisterException("服务端出现未知错误", RegisterStateEnum.OtherError);
                case 3:
                    throw new RegisterException("注册人数达到服务端设置的上限", RegisterStateEnum.Maximum);
            }
        }

        public static LocalDiskUser GetLoginedUser()
        {
            return (LocalDiskUser)_netDiskUserList.FirstOrDefault();
        }

        public void Save(ILocalDiskUser entity)
        {
            var userPath = Path.Combine(_userDataSavePath,((LocalDiskUser)entity).Name);
            if (!Directory.Exists(userPath))
                Directory.CreateDirectory(userPath);
            File.WriteAllText(Path.Combine(userPath,"Account.json"),entity.ToString());
            ((LocalDiskUser)entity).GetTaskManger().StopAndSave();
        }
    }
}
