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

        // File type filters
        public List<string> FileFilters { get; set; } = new();

        // Static folder checkboxes from UI
        public List<string> IncludedFolders { get; set; } = new();
    }

    public static class DirectoryScanner
    {
        // Required by MainForm
        public static string BuildTree(DirectoryScanOptions opt) => GenerateTree(opt);

        // =====================================================
        //                 ASCII TREE OUTPUT
        // =====================================================
        public static string GenerateTree(DirectoryScanOptions opt)
        {
            var sb = new StringBuilder();

            string rootName = Path.GetFileName(opt.RootPath.TrimEnd('\\', '/'));
            if (string.IsNullOrEmpty(rootName))
                rootName = opt.RootPath;

            sb.AppendLine($"[{rootName}]");

            ScanTree(opt.RootPath, sb, "", opt);

            return sb.ToString();
        }

        private static void ScanTree(string path, StringBuilder sb, string indent, DirectoryScanOptions opt)
        {
            var dirs = Directory.GetDirectories(path)
                .Where(d => !IsStaticFolderExcluded(Path.GetFileName(d), opt))
                .OrderBy(d => d)
                .ToList();

            var files = Directory.GetFiles(path)
                .Where(f => MatchesFilter(f, opt.FileFilters))
                .OrderBy(f => f)
                .ToList();

            int total = dirs.Count + files.Count;
            int index = 0;

            // Files
            foreach (var file in files)
            {
                index++;
                bool last = index == total;
                sb.AppendLine($"{indent}{(last ? "└── " : "├── ")}{Path.GetFileName(file)}");
            }

            // Folders
            foreach (var dir in dirs)
            {
                index++;
                bool last = index == total;
                string folderName = Path.GetFileName(dir);

                sb.AppendLine($"{indent}{(last ? "└── " : "├── ")}[{folderName}]");

                string childIndent = indent + (last ? "    " : "│   ");
                ScanTree(dir, sb, childIndent, opt);
            }
        }

        // =====================================================
        //                 MARKDOWN TREE OUTPUT
        // =====================================================
        public static string GenerateMarkdownTree(DirectoryScanOptions opt)
        {
            var sb = new StringBuilder();

            string rootName = Path.GetFileName(opt.RootPath.TrimEnd('\\', '/'));
            if (string.IsNullOrEmpty(rootName))
                rootName = opt.RootPath;

            sb.AppendLine($"# {rootName} file tree\n");

            WriteMarkdownFolder(opt.RootPath, sb, 0, opt);

            return sb.ToString();
        }

        private static void WriteMarkdownFolder(string path, StringBuilder sb, int level, DirectoryScanOptions opt)
        {
            string indent = new string(' ', level * 2);
            string folderName = Path.GetFileName(path);

            sb.AppendLine($"{indent}- **{folderName}/**");

            var dirs = Directory.GetDirectories(path)
                .Where(d => !IsStaticFolderExcluded(Path.GetFileName(d), opt))
                .OrderBy(d => d)
                .ToList();

            var files = Directory.GetFiles(path)
                .Where(f => MatchesFilter(f, opt.FileFilters))
                .OrderBy(f => f)
                .ToList();

            // Files
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                string relPath = Path.GetRelativePath(opt.RootPath, file).Replace("\\", "/");
                string name = Path.GetFileName(file);

                sb.AppendLine(
                    $"{indent}  - [{name}]({relPath})  ({FormatSize(info.Length)}, {info.LastWriteTime:yyyy-MM-dd HH:mm})"
                );
            }

            // Folders
            foreach (var dir in dirs)
            {
                WriteMarkdownFolder(dir, sb, level + 1, opt);
            }
        }

        // =====================================================
        // HELPERS
        // =====================================================
        private static bool MatchesFilter(string file, List<string> filters)
        {
            if (filters.Count == 0)
                return true;

            string ext = Path.GetExtension(file).ToLowerInvariant();
            return filters.Any(f => f.StartsWith("*.") && ext == f.Substring(1).ToLowerInvariant());
        }

        private static bool IsStaticFolderExcluded(string folderName, DirectoryScanOptions opt)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                return false;

            string lower = folderName.ToLowerInvariant();

            // These are the ONLY static folders bound to checkboxes
            string[] staticFolders =
            {
                ".git", ".vs", "bin", "obj", "properties", "resources"
            };

            if (!staticFolders.Contains(lower))
                return false;    // always include normal folders (Core, UI, etc.)

            return !opt.IncludedFolders.Any(f =>
                string.Equals(f, folderName, StringComparison.OrdinalIgnoreCase)
            );
        }

        private static string FormatSize(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unit = 0;

            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }

            return $"{size:0.##} {units[unit]}";
        }
    }
}
