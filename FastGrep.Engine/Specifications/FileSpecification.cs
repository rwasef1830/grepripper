using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnsureThat;

namespace FastGrep.Engine.Specifications
{
    public class FileSpecification
    {
        readonly string _folderPath;
        readonly bool _includeSubfolders;
        readonly string _filePattern;

        public FileSpecification(
            string folderPath,
            bool includeSubfolders,
            string filePattern)
        {
            Ensure.That(() => filePattern).IsNotNullOrWhiteSpace();
            Ensure.That(() => folderPath).IsNotNullOrWhiteSpace();

            this._filePattern = filePattern;
            this._folderPath = folderPath;
            this._includeSubfolders = includeSubfolders;
        }

        public IEnumerable<DataSource> EnumerateFiles()
        {
            return
                Directory.EnumerateFiles(
                    this._folderPath,
                    this._filePattern,
                    this._includeSubfolders
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly)
                    .Select(
                        x =>
                        {
                            try
                            {
                                StreamReader reader = File.OpenText(x);
                                return new DataSource(x, reader, reader.BaseStream.Length);
                            }
                            catch (IOException)
                            {
                                return null;
                            }
                        })
                    .Where(x => x != null);
        }
    }
}