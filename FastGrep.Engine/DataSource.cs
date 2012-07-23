using System;
using System.IO;
using EnsureThat;

namespace FastGrep.Engine
{
    public class DataSource
    {
        public string Identifier { get; private set; }
        public long Length { get; private set; }
        public TextReader Reader { get; private set; }

        public DataSource(string identifier, TextReader reader, long length)
        {
            Ensure.That(() => identifier).IsNotNullOrWhiteSpace();
            Ensure.That(() => reader).IsNotNull();

            this.Identifier = identifier;
            this.Length = length;
            this.Reader = reader;
        }
    }
}