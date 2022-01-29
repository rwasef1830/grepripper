using System;

namespace FunkyGrep.Engine;

public class CompletedEventArgs
{
    public TimeSpan Duration { get; }

    public Exception? FailureReason { get; }

    public ProgressEventArgs FinalProgressUpdate { get; }

    public CompletedEventArgs(TimeSpan duration, ProgressEventArgs finalProgressUpdate, Exception? failureReason)
    {
        this.Duration = duration;
        this.FinalProgressUpdate = finalProgressUpdate;
        this.FailureReason = failureReason;
    }
}
