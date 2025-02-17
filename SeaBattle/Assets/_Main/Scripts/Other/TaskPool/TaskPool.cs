using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using UnityEngine;

// Вынести в отдельную папаку
namespace ExtensionClasses
{
    public class TaskPool
    {
        private List<Task> _tasksList;

        public object this[int index]
        { get => _tasksList[index]; }

        public int Count
        { get => _tasksList.Count; }

        public bool IsEmpty
        { get => _tasksList.Count == 0; }

        public bool AreAllTasksCompleted
        {
            get
            {
                if (IsEmpty) return true;
                return _tasksList[_tasksList.Count - 1].Status > TaskStatus.WaitingForChildrenToComplete;
            }
        }

        public async void Push(Task task)
        {
            try
            {
                Task lastTask;
                if (!IsEmpty)
                {
                    lastTask = _tasksList[_tasksList.Count - 1];
                    _tasksList.Add(task);
                    await lastTask;
                    task.Start();
                    await task;
                    _tasksList.Remove(task);
                }
                else
                {
                    _tasksList.Add(task);
                    task.Start();
                    await task;
                    _tasksList.Remove(task);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Tasks pool canceled with exception: {e.Message}");
            }


        }

        public bool Contains(Task task)
        {
            return _tasksList.Contains(task);
        }

        public int IndexOf(Task task)
        {
            return _tasksList.IndexOf(task);
        }

        public TaskPool()
        {
            _tasksList = new List<Task>();
        }
    }

}
