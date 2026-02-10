using System;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class IssueResponse : QueueItemBase
    {
        public string DocumentURL { get; set; } = string.Empty;
    }
}