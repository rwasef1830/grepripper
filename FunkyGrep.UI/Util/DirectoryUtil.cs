using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FunkyGrep.UI.Util
{
    public static class DirectoryUtil
    {
        public static bool? ExistsOrNullIfTimeout(string path, TimeSpan timeout)
        {
            var task = Task.Run(() => Directory.Exists(path));

            if (task.Wait(TimeSpan.FromSeconds(2)))
            {
                return task.Result;
            }

            _ = task.ContinueWith(t => Debug.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            return null;
        }
    }
}
