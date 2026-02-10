using System;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    /// <summary>
    /// Base class for queue items used in requests and responses.
    /// </summary>
    public abstract class QueueItemBase
    {
        /// <summary>
        /// Identification of the queue used for processing.
        /// </summary>
        public Guid ftQueueID { get; set; }
        /// <summary>
        /// Identification of the item within a specific queue used for processing.
        /// </summary>
        public Guid ftQueueItemID { get; set; }

        /// <summary>
        /// row in which the item is stored within a specific queue used for processing
        /// </summary>
        public UInt64? ftQueueRow { get; set; }
    }
}