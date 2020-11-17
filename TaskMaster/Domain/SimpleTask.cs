﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TaskMaster.Domain
{
    class SimpleTask : ITask
    {
        private string _topic;
        private string _description;
        private TaskState _state;
        private DateTime? _start;
        private DateTime? _finish;
        private DateTime? _deadLine;
        private IPerformer _performer;

        public SimpleTask(IOwner owner, string topic, string description) 
        {
            Owner = owner;
            _topic = topic;
            _description = description;
        }

        string ITask.Topic
        {
            get => _topic;
            set => _topic = value;
        }

        string ITask.Description
        {
            get => _description;
            set => _description = value;
        }

        TaskState ITask.State
        {
            get => _state;
            set => _state = value;
        }

        DateTime? ITask.Start
        {
            get => _start;
            set => _start = value;
        }

        DateTime? ITask.Finish
        {
            get => _finish;
            set => _finish = value;
        }

        DateTime? ITask.DeadLine
        {
            get => _deadLine;
            set => _deadLine = value;
        }

        IPerformer ITask.Performer
        {
            get => _performer;
            set => _performer = value;
        }

        public IOwner Owner { get;}
    }
}