namespace GrepRipper.Engine;

public class ProgressEventArgs
{
    public long SearchedCount { get; }
    public long TotalCount { get; }
    public long FailedCount { get; }
    public long SkippedCount { get; }

    public ProgressEventArgs(long searchedCount, long totalCount, long failedCount, long skippedCount)
    {
        this.SearchedCount = searchedCount;
        this.TotalCount = totalCount;
        this.FailedCount = failedCount;
        this.SkippedCount = skippedCount;
    }
}
