using System.IO;

namespace GrepRipper.Engine;

public interface IDataSource
{
    string Identifier { get; }
    Stream OpenRead();
}
