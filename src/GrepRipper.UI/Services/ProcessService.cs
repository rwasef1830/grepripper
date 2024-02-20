using System.Diagnostics;

namespace GrepRipper.UI.Services;

class ProcessService : IProcessService
{
    public Process? Start(ProcessStartInfo processStartInfo)
    {
        return Process.Start(processStartInfo);
    }
}
