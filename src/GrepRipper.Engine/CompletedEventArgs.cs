using System;

namespace GrepRipper.Engine;

public class CompletedEventArgs(TimeSpan duration, ProgressEventArgs finalProgressUpdate, Exception? failureReason)
{
    public TimeSpan Duration { get; } = duration;

    public Exception? FailureReason { get; } = failureReason;

    public ProgressEventArgs FinalProgressUpdate { get; } = finalProgressUpdate;
}
