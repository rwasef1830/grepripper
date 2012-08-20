using System;

namespace FastGrep.Engine
{
    public class CompletedEventArgs : EventArgs
    {
        public TimeSpan Duration { get; private set; }

        public CompletedEventArgs(TimeSpan duration)
        {
            this.Duration = duration;
        }
    }
}
