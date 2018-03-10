using System;

namespace Accelerider.Windows.Infrastructure.Interfaces2
{
    public interface IAcceleriderUser
    {
        string Token { get; }

        string Email { get; }

        string Username { get; }

        Uri AvatarUrl { get; }
    }
}
 