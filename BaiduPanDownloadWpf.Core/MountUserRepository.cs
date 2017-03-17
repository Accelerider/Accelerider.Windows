using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaiduPanDownloadWpf.Core.NetWork;
using BaiduPanDownloadWpf.Core.NetWork.Packets;
using BaiduPanDownloadWpf.Infrastructure;
using BaiduPanDownloadWpf.Infrastructure.Exceptions;
using BaiduPanDownloadWpf.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using Newtonsoft.Json.Linq;

namespace BaiduPanDownloadWpf.Core
{
    public class MountUserRepository : ModelBase, IMountUserRepository
    {
        private readonly List<IMountUser> _netDiskUserList = new List<IMountUser>();


        public MountUserRepository(IUnityContainer container) : base(container)
        {
            if (Directory.Exists(Common.UserDataSavePath))
            {
                ReadLocalUserData();
            }
            else
            {
                Directory.CreateDirectory(Common.UserDataSavePath);
            }
        }


        public IMountUser FirstOrDefault()
        {
            return _netDiskUserList.FirstOrDefault(item => item.IsConnectedServer);
        }
        public IMountUser FindById(string username)
        {
            return _netDiskUserList.FirstOrDefault(item => item.Username == username);
        }
        public IEnumerable<IMountUser> GetAll()
        {
            return _netDiskUserList.AsReadOnly();
        }
        public bool Contains(string id)
        {
            return _netDiskUserList.Any(item => item.Username == id);
        }
        public void Remove(string id)
        {
            var temp = _netDiskUserList.FirstOrDefault(item => item.Username == id);
            if (temp != null) _netDiskUserList.Remove(temp);
        }
        public async Task<IMountUser> CreateAsync(string username, string password, bool force = false)
        {
            if (Contains(username))
            {
                // 1.在已有列表中查找；
                var existedObj = FindById(username);
                //if (!existedObj.IsRememberPassword)
                existedObj.SetAccountInfo(username, password);
                if (!existedObj.IsConnectedServer) await existedObj.SignInAsync();
                return existedObj;
            }
            // 2.如果force = true，则向服务器注册。
            if (force) await RegisterAsync(username, password);
            // 3.添加并连接至服务器；
            Add(username, password);
            var temp = FindById(username);
            await temp.SignInAsync();
            return temp;
        }
        public void Save()
        {
            foreach (var entity in _netDiskUserList)
            {
                var userPath = Path.Combine(Common.UserDataSavePath, ((MountUser)entity).Username);
                if (!Directory.Exists(userPath))
                    Directory.CreateDirectory(userPath);
                File.WriteAllText(Path.Combine(userPath, "Account.dat"), entity.ToString().EncryptByRijndael());
                ((MountUser)entity).GetTaskManger().StopAndSave();
            }
        }


        private void ReadLocalUserData()
        {
            var directories = Directory.GetDirectories(Common.UserDataSavePath);
            foreach (var item in directories)
            {
                var filePath = Path.Combine(item, "Account.dat");
                if (!File.Exists(filePath)) continue;
                var userInfo = File.ReadAllText(filePath).DecryptByRijndael();
                if (string.IsNullOrEmpty(userInfo)) continue;
                var json = JObject.Parse(userInfo);
                var username = json.Value<string>("Username");
                var password = json.Value<string>("Password");
                Add(username, password);
                var temp = FindById(username);
                temp.IsAutoSignIn = json.Value<bool>("IsAutoSignIn");
                temp.IsRememberPassword = true;
            }
        }
        private async Task RegisterAsync(string username, string password)
        {
            var dataServer = Server.DefaultServer;
            var json = JObject.Parse(await dataServer.SendPacketAsync(new RegisterPacket
            {
                Name = username,
                Password = password
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
        private void Add(string username, string password)
        {
            var result = Container.Resolve<MountUser>();
            result.SetAccountInfo(username, password);
            _netDiskUserList.Add(result);
        }
    }
}
