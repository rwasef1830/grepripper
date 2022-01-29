using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace FunkyGrep.UI.Util;

public static class DirectoryUtil
{
    [SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
    public static bool? ExistsOrNullIfTimeout(string path, TimeSpan timeout)
    {
        var task = Task.Run(() => Directory.Exists(path));

        if (task.Wait(timeout))
        {
            return task.Result;
        }

        _ = task.ContinueWith(t => Debug.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        return null;
    }
}
