using ExtensionClasses;
using Mirror.BouncyCastle.Security;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Scripts.LogService
{
    public class LogMaster
    {
        #region [ Реализация синглтона ]
        static private LogMaster _instance;
        static public LogMaster Instance
        {
            get
            {
                if (_instance == null) _instance = new LogMaster();
                return _instance;
            }   
        }

        #endregion

        private string SavePath { get; }

        private Queue<Task> TaskPool;

        public void Log(string message, bool saveLogs = false)
        {
            Task.Run(() =>
            {
                List<Task> TaskPool1 = new List<Task>();
                try
                {
                    Debug.Log(TaskPool1[0].Status >=  (TaskStatus.RanToCompletion));
                    if (saveLogs)
                    {
                        string directoryName = "\\Logs";
                        string logPathDirectory = SavePath + directoryName;
                        if (Directory.Exists(logPathDirectory))
                        {
                            Directory.CreateDirectory(SavePath + directoryName);
                        }

                        string tmpLogFileNmae = GenerateLogFileName();
                        string logsPathFile = logPathDirectory + tmpLogFileNmae;
                        using (FileStream fs = File.Create(logsPathFile))
                        {
                            File.AppendAllText(logsPathFile, message);
                        }

                    }

                    {
                        Debug.Log(message);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally { }
            });
        }

        public async void Test() 
        {
            Task task1 = new Task(() => {
                Debug.Log(Thread.GetCurrentProcessorId());
                Debug.Log("task1 - Start");
                Thread.Sleep(1000);
                Debug.Log("task1 - End");
            });
            Task task2 = new Task(() => {
                Debug.Log(Thread.GetCurrentProcessorId());
                Debug.Log("task2 - Start");
                Thread.Sleep(1000);
                Debug.Log("task2 - End");
            });
            Task task3 = new Task(() => {
                Debug.Log(Thread.GetCurrentProcessorId());
                Debug.Log("task3 - Start");
                Thread.Sleep(1000);
                Debug.Log("task3 - End");
            });
            Task task4 = new Task(() => {
                Debug.Log(Thread.GetCurrentProcessorId());
                Debug.Log("task4 - Start");
                Thread.Sleep(1000);
                Debug.Log("task4 - End");
            });
            Task task5 = new Task(() => {
                Debug.Log(Thread.GetCurrentProcessorId());
                Debug.Log("task5 - Start");
                Thread.Sleep(1000);
                Debug.Log("task5 - End");
            });

            TaskPool taskPool = new TaskPool();
            taskPool.Push(task1);

            taskPool.Push(task2);
            taskPool.Push(task3);
            taskPool.Push(task4);
            taskPool.Push(task5);
            //taskPool.RunSequentially();
            //Debug.Log(taskPool.Count);
        }


        private string GenerateLogFileName()
        {
            string result = string.Empty;

            return result;
        } 

        private string FormatMessage(string message)
        {
            string result = string.Empty;

            return result;
        }

        public LogMaster()
        {
            SavePath = Application.persistentDataPath;
        }
    }
}

