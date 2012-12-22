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
        readonly IEnumerable<GlobExpression> _filePatterns;
        readonly IEnumerable<GlobExpression> _fileExcludePatterns;

        public FileSpecification(
            string folderPath,
            bool includeSubfolders,
            IEnumerable<string> filePatterns,
            IEnumerable<string> fileExcludePatterns)
        {
            Ensure.That(() => folderPath).IsNotNullOrWhiteSpace();

            this._folderPath = folderPath;
            this._includeSubfolders = includeSubfolders;

            if (filePatterns == null)
            {
                filePatterns = new[] { "*.*" };
            }
            else
            {
                filePatterns = filePatterns.ToList();
            }

            this._filePatterns = filePatterns
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(p => new GlobExpression(p));

            this._fileExcludePatterns = (fileExcludePatterns ?? new string[0])
                .Where(x => !String.IsNullOrWhiteSpace(x))
                .Select(p => new GlobExpression(p));
        }

        public IEnumerable<IDataSource> EnumerateFiles()
        {
            var searchOption = this._includeSubfolders
                                   ? SearchOption.AllDirectories
                                   : SearchOption.TopDirectoryOnly;

            var enumerator =
                EnumerateFiles(this._folderPath, searchOption)
                    .Where(x => this._filePatterns.Any(p => p.IsMatch(Path.GetFileName(x))));

            if (this._fileExcludePatterns.Any())
            {
                enumerator = enumerator.Where(
                    x => !this._fileExcludePatterns.Any(p => p.IsMatch(x)));
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

        static IEnumerable<string> EnumerateFiles(string path, SearchOption searchOption)
        {
            try
            {
                var dirFiles = Enumerable.Empty<string>();
                if (searchOption == SearchOption.AllDirectories)
                {
                    dirFiles = Directory
                        .EnumerateDirectories(path)
                        .SelectMany(x => EnumerateFiles(x, searchOption));
                }
                return dirFiles.Concat(Directory.EnumerateFiles(path));
            }
            catch (UnauthorizedAccessException)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}