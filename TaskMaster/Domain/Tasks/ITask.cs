﻿using System;

namespace TaskMasterBot
{
    public interface ITask
    {
        int Id { get; }
        IOwner Owner { get; }
        IPerformer Performer { get; set; }
        string Topic { get; set; }
        string Description { get; set; }
        TaskState State { get; set; }
        DateTime? Start { get; set; }
        DateTime? Finish { get; set; }
        DateTime DeadLine { get; set; }
        bool TryPerform(IPerformer performer);
        bool TryTake(IPerformer performer);
    }
}
