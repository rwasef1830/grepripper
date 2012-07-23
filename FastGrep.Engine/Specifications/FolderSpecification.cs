using System;
using System.IO;
using EnsureThat;

namespace FastGrep.Engine.Specifications
{
    public class FolderSpecification
    {
        public string FolderPath { get; private set; }
        public bool IncludeSubfolders { get; private set; }

        public FolderSpecification(string folderPath, bool includeSubfolders)
        {
            Ensure.That(() => folderPath).IsNotNullOrWhiteSpace();

            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException();

            this.FolderPath = folderPath;
            this.IncludeSubfolders = includeSubfolders;
        }
    }
}