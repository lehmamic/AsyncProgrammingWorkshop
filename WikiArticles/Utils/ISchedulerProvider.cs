using System;
using System.Reactive.Concurrency;

namespace WikiArticles.Utils;

public interface ISchedulerProvider
{
    TimeSpan CreateTime(TimeSpan time);

    IScheduler Scheduler { get; }
}
