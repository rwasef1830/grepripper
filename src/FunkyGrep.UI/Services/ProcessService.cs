using System.Diagnostics;

namespace FunkyGrep.UI.Services;

class ProcessService : IProcessService
{
    public Process? Start(ProcessStartInfo processStartInfo)
    {
        return Process.Start(processStartInfo);
    }
}
