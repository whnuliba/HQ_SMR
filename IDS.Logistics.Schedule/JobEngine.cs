using IDS.Ioc;
using log4net;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace IDS.Logistics.Schedule
{
    public class JobEngine : IDisposable
    {
        public readonly TimeSpan mInterval;
        public ConcurrentDictionary<string?, ITask> mJobDic;
        public ConcurrentQueue<ITask> mJobQueue;
        public static JobEngine mInstance = null;
        public static readonly object InstanceLock = new object();
        public virtual ILog mLogger { set; get; }

        public JobEngine()
        {

            mLogger = LogManager.GetLogger(typeof(JobEngine));//(ILogger<JobEngine>?)ContainerUtils.AutofacServiceProvider.GetService(typeof(Logger<JobEngine>));
             mJobDic = new ConcurrentDictionary<string?, ITask>();
            mInterval = TimeSpan.FromMilliseconds(20);
            mJobQueue = new ConcurrentQueue<ITask>();
            IsRunning = false;
        }

        public static JobEngine Instance
        {
            get
            {
                if (mInstance != null)
                    return mInstance;
                lock (InstanceLock)
                {
                    return mInstance ?? (mInstance = new JobEngine());
                }
            }
        }

        public void Dispose()
        {
            Stop();
            mJobDic.Clear();
            mJobDic = null;
            while (!mJobQueue.IsEmpty)
                mJobQueue.TryDequeue(out var _);
            mJobQueue = null;
        }

        public bool IsRunning { get;  set; }

        public bool AddJob(ITask job)
        {
            if (job == null) return false;
            return mJobDic.AddOrUpdate(job.JobId, (k) => job, (k, v) => job) != null;
        }

        public bool ExecuteJob(ITask job)
        {
            try
            {
                job.Executing = true;
                job.Execute();
                job.LastExecuted = DateTime.Now;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                job.Executing = false;
            }
        }

        public bool ExecuteJob(string? jobId)
        {
            if (mJobDic.TryRemove(jobId, out var job))
                return job != null && ExecuteJob(job);
            return false;
        }

        public bool RemoveJob(string? jobId)
        {
            return mJobDic.TryRemove(jobId, out var _);
        }

        public CancellationTokenSource TokenSource;

        public void Start()
        {

            if (TokenSource != null && !TokenSource.IsCancellationRequested) return;

            TokenSource = new CancellationTokenSource();
            Task.Factory.StartNew((f) => CheckJob(f), TokenSource.Token, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew((f) => ExecuteQueue(f), TokenSource.Token, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            TokenSource?.Cancel();
            int i = 0;
            while (i < 10)
            {
                if (mJobDic.All(f => !f.Value.Executing))
                    break;
                i++;
                Thread.Sleep(100);
            }



        }

        public void CheckJob(object obj)
        {
            var token = (CancellationToken)obj;
            var lockJob = new List<ITask>();
            while (!token.IsCancellationRequested)
                try
                {
                    var listJobs = mJobDic?.Values?.ToList();
                    if (listJobs == null) return;

                    var listQueue = mJobQueue?.ToArray()?.ToList();
                    if (listQueue == null) return;
                    var list = listJobs.Where(f => f.Executing).Union(listQueue).ToList();
                    var listLock = list.Where(f => !string.IsNullOrWhiteSpace(f.LockName) && f.LockName != "#")
                        .Select(f => f.LockName).Distinct().ToList();
                    var candoList = listJobs.Except(list).OrderBy(f => f.LastExecuted).ToList();
                    foreach (var job in candoList)
                    {
                        if (token.IsCancellationRequested) return;
                        if (!DoJob(job)) continue;
                        if (string.IsNullOrWhiteSpace(job.LockName) || job.LockName == "#")
                        {
                            mJobQueue.Enqueue(job);
                            continue;
                        }

                        if (listLock.Contains(job.LockName)) continue;
                        listLock.Add(job.LockName);
                        mJobQueue.Enqueue(job);
                    }
                }
                finally
                {
                    Thread.Sleep(mInterval);
                }
        }

        public bool DoJob(ITask job)
        {
            if (job == null) return false;
            if (job.Executing) return false;
            if (job.MonthDay > 0)
                return DoMonthJob(job);
            else if (job.WeekDay != -1)
                return DoWeekJob(job);
            else if (job.AtTime != TimeSpan.Zero)
                return DoDayJob(job);
            else if (job.Interval != TimeSpan.Zero)
                return DoIntervalJob(job);
            return false;
        }

        public bool DoIntervalJob(ITask job)
        {
            var currentDate = DateTime.Now;
            var ts = currentDate.Subtract(job.LastExecuted);
            return ts >= job.Interval;
            //  if (ts >= job.Interval)
            // RequestExecuteJob(job);
        }

        public bool DoDayJob(ITask job)
        {
            var currentDate = DateTime.Now;
            if (job.LastExecuted.Year < currentDate.Year)
            {
                return currentDate.TimeOfDay >= job.AtTime;
                //if (currentDate.TimeOfDay >= job.AtTime)
                //    RequestExecuteJob(job);
            }
            else if (job.LastExecuted.Year == currentDate.Year)
            {
                if (job.LastExecuted.Month < currentDate.Month)
                    return currentDate.TimeOfDay >= job.AtTime;
                //if (currentDate.TimeOfDay >= job.AtTime)
                //    RequestExecuteJob(job);
                else if (job.LastExecuted.Month == currentDate.Month)
                    return job.LastExecuted.Day < currentDate.Day && currentDate.TimeOfDay >= job.AtTime;
                //if (job.LastExecuted.Day >= currentDate.Day) return ;
                //if (currentDate.TimeOfDay >= job.AtTime)
                //    RequestExecuteJob(job);
            }

            return false;
        }

        public bool DoWeekJob(ITask job)
        {
            var currentDate = DateTime.Now;
            return job.WeekDay == (int)currentDate.DayOfWeek
                   && job.LastExecuted.DayOfYear < currentDate.DayOfYear
                   && (job.AtTime == TimeSpan.Zero || currentDate.TimeOfDay >= job.AtTime);


            //var currentDate = DateTime.Now;
            //if (job.WeekDay != (int)currentDate.DayOfWeek ||
            //    job.LastExecuted.DayOfYear >= currentDate.DayOfYear) return;
            //if (job.AtTime != TimeSpan.Zero)
            //{
            //    if (currentDate.TimeOfDay >= job.AtTime)
            //        RequestExecuteJob(job);
            //}
            //else
            //{
            //    RequestExecuteJob(job);
            //}
        }

        public bool DoMonthJob(ITask job)
        {
            var currentDate = DateTime.Now;
            return job.MonthDay == (int)currentDate.Day
                   && job.LastExecuted.Month < currentDate.Month
                   && (job.AtTime == TimeSpan.Zero || currentDate.TimeOfDay >= job.AtTime);

            //var currentDate = DateTime.Now;
            //if (job.MonthDay != currentDate.Day || job.LastExecuted.Month >= currentDate.Month) return;
            //if (job.AtTime != TimeSpan.Zero)
            //{
            //    if (currentDate.TimeOfDay >= job.AtTime)
            //        RequestExecuteJob(job);
            //}
            //else
            //{
            //    RequestExecuteJob(job);
            //}
        }

        //public void RequestExecuteJob(IJob job)
        //{

        //    lock (QueueLock)
        //    {
        //        //if (mJobQueue == null) return;
        //        //if (mJobQueue.Any(f => f.JobId == job.JobId)) return;
        //        //if (job.Executing) return;
        //        mJobQueue.Enqueue(job);
        //    }


        //    // Task.Run(ExecuteJob);
        //    //var thread = new Thread(ExecuteJob) { IsBackground = true, Priority = ThreadPriority.AboveNormal };
        //    //thread.Start();

        //    //ExecuteJob();
        //}

        public void ExecuteQueue(object obj)
        {
            var token = (CancellationToken)obj;
            while (!token.IsCancellationRequested)
                try
                {
                    if (mJobQueue == null) break;
                    while (!mJobQueue.IsEmpty)
                    {
                        if (token.IsCancellationRequested) return;
                        if (!mJobQueue.TryDequeue(out var job)) continue;
                        if (job == null || job.Executing) continue;
                        job.Executing = true;
                        job.LastExecuted = DateTime.Now;
                        Task.Run(() => Execute(job), token);
                        Thread.Sleep(0);
                    }

                    Thread.Sleep(5);
                }
                catch (Exception e)
                {
                    mLogger?.Error($"执行定时器工作发生异常:{e.Message}", e);
                }
        }

        public void Execute(ITask job)
        {
            try
            {
                job.Execute();
            }
            catch (Exception e)
            {
                mLogger?.Error($"执行定时器工作发生异常:{e.Message}", e);
            }
            finally
            {
                job.Executing = false;
            }
        }
    }
}
