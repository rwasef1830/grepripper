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
        readonly IEnumerable<GlobExpression> _fileExcludePatterns;

        internal FileSpecification(
            string folderPath,
            bool includeSubfolders,
            string filePattern,
            IEnumerable<string> fileExcludePatterns)
        {
            Ensure.That(() => folderPath).IsNotNullOrWhiteSpace();
            Ensure.That(() => filePattern).IsNotNullOrWhiteSpace();

            this._folderPath = folderPath;
            this._includeSubfolders = includeSubfolders;
            this._filePattern = filePattern;

            this._fileExcludePatterns = (fileExcludePatterns ?? new string[0])
                .Where(x => x != null)
                .Select(p => new GlobExpression(p));
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
                    .Where(x => !this._fileExcludePatterns.All(p => p.IsMatch(x)))
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