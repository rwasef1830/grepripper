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

        public FileSpecification(
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
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(p => new GlobExpression(p));
        }

        public IEnumerable<IDataSource> EnumerateFiles()
        {
            var enumerator =
                EnumerateFiles(
                    this._folderPath,
                    this._filePattern,
                    this._includeSubfolders
                        ? SearchOption.AllDirectories
                        : SearchOption.TopDirectoryOnly);

            if (this._fileExcludePatterns.Any())
            {
                enumerator = enumerator.Where(x => !this._fileExcludePatterns.All(p => p.IsMatch(x)));
            }

            return enumerator.Select(
                x =>
                {
                    try
                    {
                        return new FileDataSource(x);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        return null;
                    }
                }).Where(x => x != null);
        }

        static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            try
            {
                var dirFiles = Enumerable.Empty<string>();
                if (searchOption == SearchOption.AllDirectories)
                {
                    dirFiles = Directory.EnumerateDirectories(path)
                        .SelectMany(x => EnumerateFiles(x, searchPattern, searchOption));
                }
                return dirFiles.Concat(Directory.EnumerateFiles(path, searchPattern));
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}