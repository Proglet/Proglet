using System;

namespace DockerSlaveManager.Services
{
    public class QueueItem
    {
        public string Id { get; set; }
        public Status status { get; set; } = Status.Queued;
        public DateTime queueTime { get; set; } = DateTime.Now;
        public DateTime runTime { get; set; } = DateTime.MinValue;
        public DateTime finishTime { get; set; } = DateTime.MinValue;
    }
}
