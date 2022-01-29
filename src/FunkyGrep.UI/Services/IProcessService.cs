using System.Diagnostics;
using JetBrains.Annotations;

namespace FunkyGrep.UI.Services;

[PublicAPI]
public interface IProcessService
{
    Process? Start(ProcessStartInfo processStartInfo);
}
