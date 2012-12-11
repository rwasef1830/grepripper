using System;

namespace FastGrep.Engine
{
    public class ProgressEventArgs : EventArgs
    {
        public long NumberOfSearchedFiles { get; private set; }
        public long TotalNumberOfFiles { get; private set; }
        public long NumberOfFailedFiles { get; private set; }

        public ProgressEventArgs(long numberOfSearchedFiles, long totalNumberOfFiles, long numberOfFailedFiles)
        {
            this.NumberOfSearchedFiles = numberOfSearchedFiles;
            this.TotalNumberOfFiles = totalNumberOfFiles;
            this.NumberOfFailedFiles = numberOfFailedFiles;
        }
    }
}