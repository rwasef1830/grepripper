using System.IO;

namespace FastGrep.Engine
{
    public interface IDataSource
    {
        string Identifier { get; }
        long GetLength();
        TextReader OpenReader();
    }
}