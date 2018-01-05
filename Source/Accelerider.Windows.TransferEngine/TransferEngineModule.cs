using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.TransferEngine.Implements;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.TransferEngine
{
    public class TransferEngineModule : ModuleBase
    {
        public TransferEngineModule(IUnityContainer container) : base(container)
        {
        }

        public override void Initialize()
        {
            RegisterTypeIfMissing<IDownloader, Downloader>(true);
            RegisterTypeIfMissing<IUploader, Uploader>(true);
        }
    }
}
