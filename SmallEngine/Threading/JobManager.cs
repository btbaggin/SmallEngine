using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SmallEngine.Threading
{
    public class JobManager
    {
        #region Job class
        private class Job
        {
            public Action<object> Work { get; set; }
            public object Parameters { get; set; }

            public Job(Action<object> pWork, object pParameters)
            {
                Work = pWork;
                Parameters = pParameters;
            }
        }
        #endregion

        static volatile bool _cancelThread;
        private List<Job> _jobs;
        private CountdownEvent _ce;

        #region Properties
        static JobManager _instance;
        public static JobManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new JobManager();
                }

                return _instance;
            }
        }
        #endregion

        #region Constructor
        public JobManager()
        {
            _instance = this;
            _ce = new CountdownEvent(0);
            _jobs = new List<Job>();
            var cores = System.Environment.ProcessorCount;
            ThreadPool.SetMaxThreads(cores, cores);
        }
        #endregion

        public void BeginBatch()
        {
            _ce = new CountdownEvent(1);
        }

        public void Queue(Action<object> pJob, object pParameters)
        {
            _ce.AddCount();
            _jobs.Add(new Job(pJob, pParameters));
        }

        public void ExecuteBatch()
        {
            foreach(var j in _jobs)
            {
                ThreadPool.QueueUserWorkItem((p) =>
                {
                    if(_cancelThread)
                    {
                        _ce.Signal();
                        return;
                    }

                    j.Work.Invoke(j.Parameters);
                    _ce.Signal();
                });
            }
            _jobs.Clear();

            _ce.Signal();
            _ce.Wait();
        }

        public void Dispose()
        {
            _cancelThread = true;
            _jobs.Clear();
        }
    }
}
