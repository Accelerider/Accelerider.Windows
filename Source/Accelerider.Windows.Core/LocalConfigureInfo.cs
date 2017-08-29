using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accelerider.Windows.Core
{
    internal class LocalConfigureInfo : ILocalConfigureInfo
    {
        public LocalConfigureInfo()
        {
            try
            {
                var configLocation = Path.Combine(Directory.GetCurrentDirectory(), "Config.json");
                if (File.Exists(configLocation))
                {
                    var config = JObject.Parse(File.ReadAllText(configLocation));
                    var type = typeof(LocalConfigureInfo);
                    foreach (var method in type.GetMethods())
                    {
                        if (method.Name.StartsWith("set_"))
                        {
                            this.GetType().GetMethod(method.Name).Invoke(this, new[]
                            {
                                config[method.Name.Replace("set_", string.Empty)]
                                    .ToObject(method.GetParameters().Last().ParameterType)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public string Theme { get; set; }
        public LanguageEnum Language { get; set; }


        public string Username { get; set; } = "name";
        public string PasswordEncrypted { get; set; } = "";
        public bool IsAutoSignIn { get; set; }


        public bool NotDisplayDownloadDialog { get; set; }
        public FileLocation DownloadDirectory { get; set; }
        public int ParallelTaskNumber { get; set; }
        public double SpeedLimit { get; set; }


        public void Save()
        {
            File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "Config.json"),
                JsonConvert.SerializeObject(this, Formatting.Indented));
        }
    }
}