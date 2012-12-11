using System.IO;
using EnsureThat;

namespace FastGrep.Engine
{
    public class FileDataSource : IDataSource
    {
        readonly FileInfo _fileInfo;

        public string Identifier
        {
            get { return this._fileInfo.FullName; }
        }

        public long GetLength()
        {
            return this._fileInfo.Length;
        }

        public TextReader OpenReader()
        {
            return this._fileInfo.OpenText();
        }

        public FileDataSource(string filePath)
        {
            Ensure.That("filePath").IsNotNullOrWhiteSpace();
            this._fileInfo = new FileInfo(filePath);
        }
    }
}