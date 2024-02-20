using System.Diagnostics;
using JetBrains.Annotations;

namespace GrepRipper.UI.Services;

[PublicAPI]
public interface IProcessService
{
    Process? Start(ProcessStartInfo processStartInfo);
}
