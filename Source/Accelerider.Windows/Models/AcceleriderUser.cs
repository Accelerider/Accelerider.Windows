using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces2;
using MaterialDesignThemes.Wpf;

namespace Accelerider.Windows.Models
{
    internal class AcceleriderUser : IAcceleriderUser
    {
        private readonly ISnackbarMessageQueue _messageQueue;

        public AcceleriderUser(ISnackbarMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public string Token { get; private set; }

        public long Id { get; set; }

        public string Email { get; set; }

        public bool EmailVisibility { get; set; }

        public string Username { get; set; }

        public Uri AvatarUrl { get; set; }

        public string MultiOnline { get; set; }

        public IList<string> Roles { get; set; }

        public IList<long> ModuleIds { get; set; }

        public Task<bool> LoginAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SignUpAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UploadAvatarAsync(string imageFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
