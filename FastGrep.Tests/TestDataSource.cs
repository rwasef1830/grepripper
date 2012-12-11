using System.IO;
using System.Text;
using FastGrep.Engine;

namespace FastGrep.Tests
{
    class TestDataSource : IDataSource
    {
        readonly long _length;
        readonly string _content;

        public string Identifier { get; private set; }

        public long GetLength()
        {
            return this._length;
        }

        public TextReader OpenReader()
        {
            return new StringReader(this._content);
        }

        public TestDataSource(string identifier, string content, Encoding encoding)
        {
            this.Identifier = identifier;
            this._content = content;
            this._length = encoding.GetByteCount(content);
        }
    }
}