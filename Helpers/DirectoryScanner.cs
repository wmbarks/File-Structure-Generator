using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace File_Structure_Generator
{
    public class DirectoryScanOptions
    {
        public string RootPath { get; set; } = "";
        public bool UseRelativePaths { get; set; } = true;

        public List<string> FileFilters { get; set; } = new();
        public List<string> IncludedFolders { get; set; } = new();
    }

    public static class DirectoryScanner
    {
        // Folders that should only be included when their checkbox is checked.
        private static readonly HashSet<string> SpecialFolders = new(
            new[] { ".git", ".vs", "bin", "obj", "Properties", "Resources" },
            StringComparer.OrdinalIgnoreCase);

        public static string GenerateTree(DirectoryScanOptions opt)
        {
            var sb = new StringBuilder();

            string rootName = Path.GetFileName(opt.RootPath);
            if (string.IsNullOrEmpty(rootName))
                rootName = opt.RootPath;

            sb.AppendLine($"[{rootName}]");

            // NEW correct call:
            Scan(opt.RootPath, sb, opt);

            return sb.ToString();
        }

        private static void Scan(string rootPath, StringBuilder sb, DirectoryScanOptions opt)
        {
            // Get children of root WITHOUT printing the root again
            var dirs = Directory.GetDirectories(rootPath)
                .Where(d => !FolderIsExcluded(Path.GetFileName(d), opt))
                .OrderBy(d => d)
                .ToList();

            var files = Directory.GetFiles(rootPath)
                .Where(f => MatchesFilter(f, opt.FileFilters))
                .OrderBy(f => f)
                .ToList();

            int total = dirs.Count + files.Count;
            int index = 0;

            // FILES
            foreach (var file in files)
            {
                index++;
                bool last = index == total;
                string name = Path.GetFileName(file);

                sb.AppendLine($"{(last ? "└── " : "├── ")}{name}");
            }

            // FOLDERS
            foreach (var dir in dirs)
            {
                index++;
                bool last = index == total;

                ScanRecursive(dir, sb, "", last, opt);
            }
        }

        private static void ScanRecursive(string path, StringBuilder sb, string indent, bool isLast, DirectoryScanOptions opt)
        {
            string folderName = Path.GetFileName(path);
            sb.AppendLine($"{indent}{(isLast ? "└── " : "├── ")}[{folderName}]");

            string childIndent = indent + (isLast ? "    " : "│   ");

            var dirs = Directory.GetDirectories(path)
                .Where(d => !FolderIsExcluded(Path.GetFileName(d), opt))
                .OrderBy(d => d)
                .ToList();

            var files = Directory.GetFiles(path)
                .Where(f => MatchesFilter(f, opt.FileFilters))
                .OrderBy(f => f)
                .ToList();

            int total = dirs.Count + files.Count;
            int index = 0;

            // FILES
            foreach (var file in files)
            {
                index++;
                bool last = index == total;
                string name = Path.GetFileName(file);

                sb.AppendLine($"{childIndent}{(last ? "└── " : "├── ")}{name}");
            }

            // FOLDERS
            foreach (var dir in dirs)
            {
                index++;
                bool last = index == total;

                ScanRecursive(dir, sb, childIndent, last, opt);
            }
        }

        private static bool FolderIsExcluded(string folderName, DirectoryScanOptions opt)
        {
            if (!SpecialFolders.Contains(folderName))
                return false; // Normal project folder → always include

            return !opt.IncludedFolders.Any(f =>
                f.Equals(folderName, StringComparison.OrdinalIgnoreCase));
        }

        private static bool MatchesFilter(string file, List<string> filters)
        {
            if (filters.Count == 0)
                return true;

            string extension = Path.GetExtension(file);

            return filters.Any(filter =>
                filter.StartsWith("*.") &&
                extension.Equals(filter.Substring(1), StringComparison.OrdinalIgnoreCase));
        }
    }
}
