using System;
using System.IO;
using System.Linq;
using System.Text;
using GrepRipper.Engine;

namespace GrepRipper.Tests;

class TestDataSource : IDataSource
{
    readonly byte[] _contentBytes;

    public TestDataSource(string content, Encoding encoding) : this(
        Guid.NewGuid().ToString(),
        content,
        encoding) { }

    public TestDataSource(string identifier, string content, Encoding encoding)
    {
        this.Identifier = identifier;
        this._contentBytes = encoding.GetPreamble().Concat(encoding.GetBytes(content)).ToArray();
    }

    #region IDataSource Members

    public string Identifier { get; }

    public Stream OpenRead()
    {
        return new MemoryStream(this._contentBytes, false);
    }

    #endregion
}
