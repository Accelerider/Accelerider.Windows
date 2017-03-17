using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaiduPanDownloadWpf.Core
{
    /// <summary>
    /// Reference https://github.com/DCjanus/DC_Note/blob/master/%E3%80%90%E6%96%B0%E6%89%8B%E5%90%91%E3%80%91%E7%99%BE%E5%BA%A6%E6%A8%A1%E6%8B%9F%E7%99%BB%E5%BD%95%E6%B5%81%E7%A8%8B%E3%80%902017.2.27%E6%97%A5%E6%9C%89%E6%95%88%E3%80%91.md
    /// </summary>
    public class BaiduServer : IDisposable
    {
        #region Basic data
        private const string GidFormat = "xxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx";

        private const string ConvertPemToXmlUrl = "https://superdry.apphb.com/RsaKeyConverter/PemToXml";
        private const string GetTokenUrl = "https://passport.baidu.com/v2/api/?getapi&tpl=mn&apiver=v3&tt={0}&class=login&gid={1}&logintype=dialogLogin&callback=bd__cbs__tesnqc";
        //private static readonly string _getUBIUrl = "https://passport.baidu.com/v2/api/?loginhistory&token={0}&tpl=mn&apiver=v3&tt={1}&gid={2}&callback=bd__cbs__splnc1";
        private const string GetVerificationCodeUrl = "https://passport.baidu.com/v2/api/?logincheck&token={0}&tpl=mn&apiver=v3&tt={1}&sub_source=leadsetpwd&username={2}&isphone={3}&dv={4}&callback=bd__cbs__sehp6m";
        private const string GetVerificationCodeImageUrl = "https://passport.baidu.com/cgi-bin/genimage?{0}";
        private const string GetPublicKeyUrl = "https://passport.baidu.com/v2/getpublickey?token={0}&tpl=mn&apiver=v3&tt={1}&gid={2}&callback=bd__cbs__9t0drq";
        private const string CheckVerificationCodeUrl = "https://passport.baidu.com/v2/?checkvcode&token={0}&tpl=mn&apiver=v3&tt={1}&verifycode={2}&codestring={3}&callback=bd__cbs__mrws8s";
        private const string LoginUrl = "https://passport.baidu.com/v2/api/?login";

        private static readonly Random Random = new Random();
        private static readonly Regex MatchBaiduId = new Regex("^BAIDUID=(.+?);", RegexOptions.Compiled);
        private static readonly Regex MatchToken = new Regex("\"token\" ?: ?\"(.+?)\",", RegexOptions.Compiled);
        private static readonly Regex MatchCodeString = new Regex("\"codeString\" : \"(.*?)\"", RegexOptions.Compiled);
        private static readonly Regex MatchPublicKeyAndKey = new Regex("\"pubkey\": ?'(.*?)', *\"key\": ?(.*?) *}", RegexOptions.Compiled);
        private static readonly Regex MatchCheckVCodeNo = new Regex("\"no\": ?\"(\\d+?)\"", RegexOptions.Compiled);

        private static string GenerateGid()
        {
            var result = new StringBuilder();
            foreach (var item in GidFormat)
            {
                switch (item)
                {
                    case 'x':
                        result.Append(Random.Next(0, 16).ToString("x"));
                        break;
                    case 'y':
                        var temp = Random.Next(0, 16);
                        result.Append((3 & temp | 8).ToString("x"));
                        break;
                    default:
                        result.Append(item);
                        break;
                }
            }
            return result.ToString();
        }
        private static long GenerateTimestamp()
        {
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var nowTime = DateTime.Now;
            return (long)Math.Round((nowTime - startTime).TotalMilliseconds);
        }
        #endregion

        #region Login Baidu Form
        private readonly Dictionary<string, string> _loginBaiduForm = new Dictionary<string, string>
        {
            { "staticpage", "https://www.baidu.com/cache/user/html/v3Jump.html" },
            { "charset", "UTF-8" },
            //{ "token", "" },//1
            { "tpl", "mn" },
            { "subpro", "" },
            { "apiver", "v3" },
            //{ "tt", "" },//2
            //{ "codestring", "" },//3
            { "safeflg", "0" },
            { "u", "https://www.baidu.com/" },
            { "isPhone", "" },
            { "detect", "1" },
            //{ "gid", "" },//4
            { "quick_user", "0" },
            { "logintype", "dialogLogin" },
            { "logLoginType", "pc_loginDialog" },
            { "idc", "" },
            { "loginmerge", "true" },
            { "splogin", "rate" },
            //{ "username", "" },//5
            //{ "password", "" },//6
            //{ "verifycode", "" },//7
            { "mem_pass", "on" },
            //{ "rsakey", "" },//8
            { "crypttype", "12" },
            { "ppui_logintime", "10797" },
            { "countrycode", "" },
            { "dv", "" },
            { "callback", "parent.bd__pcbs__r6aj37" }
        };

        #endregion

        private readonly HttpClient _httpClient;
        private readonly RSACryptoServiceProvider _rsa = new RSACryptoServiceProvider();
        private readonly CookieContainer _cookieContainer = new CookieContainer();

        private string _inputedVerificationCode;
        private string _rsaPassword;
        private string _rsaKey;
        private string _codeString;

        #region Public properties
        public string BaiduId { get; private set; }
        public string Gid { get; private set; }
        public string Token { get; private set; }

        public string Username { get; private set; }
        public bool HasVerificationCode => VerificationCodeImageUri != null;
        public Uri VerificationCodeImageUri { get; private set; }
        #endregion

        public BaiduServer()
        {
            var handler = new HttpClientHandler { UseCookies = true, CookieContainer = _cookieContainer };
            _httpClient = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, 8) };
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
        }

        public async Task SetUsernameAsync(string username)
        {
            if (!IsInitialized()) await InitializeAsync();

            Username = username;
            await UpdateVerificationCodeAsync();
        }
        public async Task SetPasswordAsync(string password)
        {
            if (string.IsNullOrEmpty(Username)) return;

            var keys = await GetPublicKeyAndKeyAsync(Token, GenerateTimestamp(), Gid);
            _rsaKey = keys.Item2;
            var response = await _httpClient.PostAsync(ConvertPemToXmlUrl, new StringContent(keys.Item1, Encoding.UTF8));
            var publicKeyXml = await response.Content.ReadAsStringAsync();
            _rsaPassword = RsaEncrypt(publicKeyXml, password);
        }
        public void SetVerificationCode(string verificationCode)
        {
            if (!HasVerificationCode) return;
            _inputedVerificationCode = verificationCode;
        }
        public async Task UpdateVerificationCodeAsync()
        {
            if (string.IsNullOrEmpty(Username)) return;
            var vCode = await GetVerificationCodeAsync(Token, GenerateTimestamp(), Username);
            if (string.IsNullOrEmpty(vCode)) return;
            _codeString = vCode;
            VerificationCodeImageUri = new Uri(string.Format(GetVerificationCodeImageUrl, vCode));
        }
        public async Task LoginAsync()
        {
            if (!IsInitialized() || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(_rsaPassword)) return;
            if (!await CheckVerificationCodeAsync(Token, GenerateTimestamp(), _inputedVerificationCode, _codeString))
                throw new ArgumentException("Incorrect verification code.");

            #region Fill the form
            _loginBaiduForm["token"] = Token;
            _loginBaiduForm["tt"] = GenerateTimestamp().ToString();
            _loginBaiduForm["codestring"] = _codeString;
            _loginBaiduForm["gid"] = Gid;
            _loginBaiduForm["username"] = Username;
            _loginBaiduForm["password"] = _rsaPassword;
            _loginBaiduForm["verifycode"] = _inputedVerificationCode;
            _loginBaiduForm["rsakey"] = _rsaKey;
            #endregion
            var stringBuilder = new StringBuilder();
            foreach (var item in _loginBaiduForm)
            {
                var value = item.Value == null ? "" : Uri.EscapeDataString(item.Value);
                stringBuilder.Append($"{item.Key}={value}&");
                //stringBuilder.Append($"{item.Key}={item.Value ?? ""}&");
            }
            var data = stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
            var response = await _httpClient.PostAsync(LoginUrl, new StringContent(data));

#if DEBUG
            var wwwCookies = _cookieContainer.GetCookies(new Uri("https://www.baidu.com"));
            var passportCookies = _cookieContainer.GetCookies(new Uri("https://passport.baidu.com"));
            Debug.WriteLine("-------------------------------------------------------------------------");
            Debug.WriteLine(await response.Content.ReadAsStringAsync());
            Debug.WriteLine("-------------------------------------------------------------------------");

            foreach (var item in wwwCookies)
            {
                Debug.WriteLine($"{DateTime.Now} : {item}");
            }
            foreach (var item in passportCookies)
            {
                Debug.WriteLine($"{DateTime.Now} : {item}");
            }
#endif
        }

        #region Private methods
        private bool IsInitialized()
        {
            return !(string.IsNullOrEmpty(BaiduId) ||
                   string.IsNullOrEmpty(Token) ||
                   string.IsNullOrEmpty(Gid));
        }
        private async Task InitializeAsync()
        {
            BaiduId = await GetBaiduIdAsync();
            _cookieContainer.SetCookies(new Uri("https://passport.baidu.com"), $"BAIDUID={BaiduId}");
            Gid = GenerateGid();
            Token = await GetTokenAsync(GenerateTimestamp(), Gid);
        }
        private async Task<string> GetBaiduIdAsync()
        {
            var html = await _httpClient.GetAsync("http://www.baidu.com");
            var setCookies = html.Headers.GetValues("Set-Cookie");
            foreach (var item in setCookies)
            {
                var temp = MatchBaiduId.Match(item).Groups[1].Value;
                if (!string.IsNullOrEmpty(temp)) return temp;
            }
            return string.Empty;
        }
        private async Task<string> GetTokenAsync(long timestamp, string gid)
        {
            var url = string.Format(GetTokenUrl, timestamp, gid);
            return (await ExtractResponeInfoAsync(url, MatchToken)).Groups[1].Value;
        }
        private async Task<string> GetVerificationCodeAsync(string token, long timestamp, string username, bool isPhone = false)
        {
            var url = string.Format(GetVerificationCodeUrl, token, timestamp, Uri.EscapeDataString(username), isPhone, string.Empty);
            return (await ExtractResponeInfoAsync(url, MatchCodeString)).Groups[1].Value;
        }
        private async Task<Tuple<string, string>> GetPublicKeyAndKeyAsync(string token, long timestamp, string gid)
        {
            var url = string.Format(GetPublicKeyUrl, token, timestamp, gid);
            var result = await ExtractResponeInfoAsync(url, MatchPublicKeyAndKey);
            return Tuple.Create(result.Groups[1].Value.Replace("\\n", Environment.NewLine).Replace("\\/", "/"), result.Groups[2].Value);
        }
        private string RsaEncrypt(string publicKeyXml, string text)
        {
            _rsa.FromXmlString(publicKeyXml);
            return Convert.ToBase64String(_rsa.Encrypt(Encoding.UTF8.GetBytes(text), false));
        }

        private async Task<bool> CheckVerificationCodeAsync(string token, long timestamp, string verificationCode, string codeString)
        {
            var url = string.Format(CheckVerificationCodeUrl, token, timestamp, Uri.EscapeDataString(verificationCode), codeString);
            return (await ExtractResponeInfoAsync(url, MatchCheckVCodeNo)).Groups[1].Value == "0";
        }
        private async Task<Match> ExtractResponeInfoAsync(string url, Regex regex)
        {
            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return regex.Match(result);
        }
        #endregion

        #region Implements IDisposable interface
        private bool _disposed;
        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                // Free any other managed objects here.
                BaiduId = null;
                Gid = null;
                Token = null;
                Username = null;
                VerificationCodeImageUri = null;
                _inputedVerificationCode = null;
                _rsaPassword = null;
                _rsaKey = null;
                _codeString = null;
            }
            // Free any unmanaged objects here.
            _httpClient.Dispose();
            _rsa.Dispose();
            _disposed = true;
        }
        #endregion
    }
}
