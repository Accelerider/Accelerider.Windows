using Newtonsoft.Json;

namespace Accelerider.Windows.Modules.NetDisk.Models
{
    internal class CloudTaskData
    {
    }

    internal class FileUpdateData
    {
    }

    internal class FileData : FileUpdateData
    {
    }

    internal abstract class NetDiskUserData
    {
    }

	public class RegisterData
	{
		[JsonProperty("name")]
		public string NickName { get; set; }

		[JsonProperty("password")]
		public string PasswordMd5 { get; set; }

		/// <summary>
		/// Phone code
		/// </summary>
		[JsonProperty("code")]
		public string Code { get; set; }

		[JsonProperty("phoneInfo")]
		public string PhoneInfo { get; set; }
	}
}
