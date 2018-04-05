using Accelerider.Windows.Infrastructure;
using Accelerider.Windows.TransportEngine.Implements;
using Microsoft.Practices.Unity;

namespace Accelerider.Windows.TransportEngine
{
    public class TransportEngineModule : ModuleBase
    {
        public TransportEngineModule(IUnityContainer container) : base(container)
        {
        }

        public override void Initialize()
        {
            RegisterTypeIfMissing<IDownloader, Downloader>(true);
            RegisterTypeIfMissing<IUploader, Uploader>(true);
        }
    }
}
