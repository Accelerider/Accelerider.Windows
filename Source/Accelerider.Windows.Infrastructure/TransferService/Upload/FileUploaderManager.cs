using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerider.Windows.Infrastructure.TransferService
{
    internal class FileUploaderManager : ITransporterManager<IUploader>
    {
        public IEnumerable<IUploader> Transporters { get; }

        public int MaxConcurrent { get; set; }

        public string ToJson()
        {
            throw new NotImplementedException();
        }

        public ITransporterManager<IUploader> FromJson(string json)
        {
            throw new NotImplementedException();
        }

        public bool Add(IUploader downloader)
        {
            throw new NotImplementedException();
        }

        public void AsNext(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Ready(Guid id)
        {
            throw new NotImplementedException();
        }

        public void StartAll()
        {
            throw new NotImplementedException();
        }

        public void SuspendAll()
        {
            throw new NotImplementedException();
        }
    }
}
