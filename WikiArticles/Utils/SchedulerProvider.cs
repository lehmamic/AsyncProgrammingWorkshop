using System;
using System.Reactive.Concurrency;

namespace WikiArticles.Utils;

public class SchedulerProvider : ISchedulerProvider
{
    public SchedulerProvider()
        : this(DefaultScheduler.Instance)
    {
    }

    public SchedulerProvider(IScheduler scheduler)
    {
        Scheduler = scheduler;
    }

    public IScheduler Scheduler { get; }

    public TimeSpan CreateTime(TimeSpan time)
    {
        return time;
    }
}
