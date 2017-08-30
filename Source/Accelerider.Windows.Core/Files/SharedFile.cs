using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.Files
{
    public class SharedFile : ISharedFile
    {
        public string Title { get; set; }
        
        public DateTime SharedTime { get; set; }

        public Uri ShareLink { get; set; }

        public string AccessCode { get; set; }

        public FileTypeEnum FileType { get; set; }
        public Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
