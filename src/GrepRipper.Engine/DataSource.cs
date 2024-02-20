using System;
using System.IO;

namespace GrepRipper.Engine;

public readonly record struct DataSource(string Identifier, Func<string, Stream> StreamFactory)
{
    public Stream OpenRead()
    {
        return this.StreamFactory(this.Identifier);
    }
}
