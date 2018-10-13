using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WebApiApplication.AspInfrastructure.Crontab;

namespace WebApiApplication.AspInfrastructure.HostingServices
{
    public class AspTaskScheduler : AspHostedService
    {
        public event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        private readonly List<SchedulerTaskWrapper> _scheduledTasks = new List<SchedulerTaskWrapper>();
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger log;
        public AspTaskScheduler(
            IEnumerable<IScheduledTask> scheduledTasks,
            IServiceScopeFactory serviceScopeFactory,
            ILogger log
        )
        {
            this.log = log?.ForContext(GetType());
            this._serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

            var referenceTime = DateTime.UtcNow;

            foreach (var scheduledTask in scheduledTasks)
            {
                _scheduledTasks.Add(new SchedulerTaskWrapper
                {
                    Schedule = CrontabSchedule.Parse(scheduledTask.Schedule),
                    Task = scheduledTask,
                    NextRunTime = referenceTime
                });
            }
        }

        private int count = 1;
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await ExecuteOnceAsync(cancellationToken);
System.Diagnostics.Debug.WriteLine("Schedule #" + count++);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                //await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        private async Task ExecuteOnceAsync(CancellationToken cancellationToken)
        {
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var referenceTime = DateTime.UtcNow;

            var tasksThatShouldRun = _scheduledTasks.Where(t => t.ShouldRun(referenceTime)).ToList();

            foreach (var taskThatShouldRun in tasksThatShouldRun)
            {
                taskThatShouldRun.Increment();

                await taskFactory.StartNew(
                    async () => {
                        try
                        {
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var t = taskThatShouldRun.Task.GetType();
                                var method = t.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
                                var arguments = method.GetParameters()
                                    .Select(
                                        arg => arg.ParameterType == typeof(CancellationToken)
                                        ? cancellationToken
                                        : scope.ServiceProvider.GetService(arg.ParameterType)
                                    )
                                    .ToArray();
                                

                                if (method.ReturnType.Equals(typeof(Task)))
                                {
                                    await (Task)method.Invoke(taskThatShouldRun.Task, arguments);
                                }
                                else
                                {
                                    method.Invoke(taskThatShouldRun.Task, arguments);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            var args = new UnobservedTaskExceptionEventArgs(
                                ex as AggregateException ?? new AggregateException(ex));

                            UnobservedTaskException?.Invoke(this, args);

                            if (!args.Observed)
                            {
                                throw;
                            }
                        }
                    },
                    cancellationToken);
            }
        }
        
        private class SchedulerTaskWrapper
        {
            public CrontabSchedule Schedule { get; set; }
            public IScheduledTask Task { get; set; }

            public DateTime LastRunTime { get; set; }
            public DateTime NextRunTime { get; set; }

            public void Increment()
            {
                LastRunTime = NextRunTime;
                NextRunTime = Schedule.GetNextOccurrence(NextRunTime);
            }

            public bool ShouldRun(DateTime currentTime)
            {
                return NextRunTime < currentTime && LastRunTime != NextRunTime;
            }
        }
    }
}
