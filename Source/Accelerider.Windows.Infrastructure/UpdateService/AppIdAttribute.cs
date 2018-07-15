using System;

namespace Accelerider.Windows.Infrastructure.UpdateService
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AppIdAttribute : Attribute
    {
        public AppIdAttribute(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
