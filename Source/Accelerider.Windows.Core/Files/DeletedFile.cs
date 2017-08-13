using System;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.Core.Files
{
    public class DeletedFile : DiskFileBase, IDeletedFile
    {
        public DateTime DeletedTime { get; set; }
        public int LeftDays { get; set; }

        public Task<bool> RestoreAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
