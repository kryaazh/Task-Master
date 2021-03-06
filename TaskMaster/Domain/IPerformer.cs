﻿using System.Collections.Generic;

namespace TaskMasterBot
{
    public interface IPerformer
    {
        long Id { get; }
        List<ITask> TakenTasks { get; }
        List<ITask> DoneTasks { get; }
        public void Take(ITask task)
        {
            if (task.TryTake(this))
                TakenTasks.Add(task);
        }

        public void Perform(ITask task)
        {
            if (!task.TryPerform(this)) return;
            TakenTasks.Remove(task);
            DoneTasks.Add(task);
        }
        public bool IsTaskTaken(ITask task) => TakenTasks.Contains(task);
    }
}