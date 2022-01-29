using System.IO;

namespace FunkyGrep.Engine;

public interface IDataSource
{
    string Identifier { get; }
    Stream OpenRead();
}
