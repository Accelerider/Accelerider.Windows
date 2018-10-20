using System;
using Newtonsoft.Json;

namespace Accelerider.Windows.TransferService
{
    public class TransferContextBase
    {
        [JsonProperty]
        public Guid Id { get; protected set; }

        [JsonProperty]
        public long TotalSize { get; internal set; }

        [JsonProperty]
        public virtual string LocalPath { get; internal set; }
    }
}
