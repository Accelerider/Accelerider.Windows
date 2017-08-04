using Accelerider.Windows.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accelerider.Windows.Infrastructure;

namespace Accelerider.Windows.Core
{
    public class SharedFile : ISharedFile
    {
        public DateTime SharedTime => throw new NotImplementedException();

        public Uri ShareLink => throw new NotImplementedException();

        public long DownloadedNumber => throw new NotImplementedException();

        public long SavedNumber => throw new NotImplementedException();

        public long VisitedNumber => throw new NotImplementedException();

        public string AccessCode => throw new NotImplementedException();

        public long FileId => throw new NotImplementedException();

        public FileTypeEnum FileType => throw new NotImplementedException();
    }
}
