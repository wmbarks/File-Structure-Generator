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

        // File type filters, e.g. ["*.cs", "*.md"]
        public List<string> FileFilters { get; set; } = new();

        // Folder behavior (legacy flags – left in place, not used in new logic)
        public bool ExcludeGit { get; set; }
        public bool ExcludeVs { get; set; }
        public bool ExcludeBinObj { get; set; }

        // If false, only the root folder is listed (no recursion)
        public bool IncludeAllSubfolders { get; set; } = true;

        // NEW: list of static folders that are CHECKED in the UI.
        // Only these static folders are included; unchecked ones are skipped.
        public List<string> IncludedFolders { get; set; } = new();
    }

    public static class DirectoryScanner
    {
        // =====================================================
        // ASCII TREE OUTPUT
        // =====================================================

        // Added to match MainForm: calls existing GenerateTree
        public static string BuildTree(DirectoryScanOptions opt)
        {
            return GenerateTree(opt);
        }

        public static string GenerateTree(DirectoryScanOptions opt)
        {
            var sb = new StringBuilder();

            string rootName = Path.GetFileName(
                opt.RootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.IsNullOrEmpty(rootName))
                rootName = opt.RootPath;

            sb.AppendLine($"[{rootName}]");

            ScanRoot(opt.RootPath, sb, opt);

            return sb.ToString();
        }

        private static void ScanRoot(string rootPath, StringBuilder sb, DirectoryScanOptions opt)
        {
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

            // FILES at root
            foreach (var file in files)
            {
                index++;
                bool last = index == total;
                string name = Path.GetFileName(file);
                sb.AppendLine($"{(last ? "└── " : "├── ")}{name}");
            }

            // FOLDERS at root
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

            if (!opt.IncludeAllSubfolders)
                return;

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

        // =====================================================
        // MARKDOWN OUTPUT (for copy/save as .md)
        // =====================================================
        public static string GenerateMarkdownTree(DirectoryScanOptions opt)
        {
            var sb = new StringBuilder();

            string rootName = Path.GetFileName(
                opt.RootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.IsNullOrEmpty(rootName))
                rootName = opt.RootPath;

            sb.AppendLine($"# {rootName} file tree");
            sb.AppendLine();

            MarkdownFolder(opt.RootPath, sb, 0, opt);

            return sb.ToString();
        }

        private static void MarkdownFolder(string path, StringBuilder sb, int level, DirectoryScanOptions opt)
        {
            string indent = new string(' ', level * 2);
            string folderName = Path.GetFileName(path);

            sb.Append(indent)
              .Append("- **")
              .Append(folderName)
              .Append("/**")
              .AppendLine();

            var dirs = Directory.GetDirectories(path)
                .Where(d => !FolderIsExcluded(Path.GetFileName(d), opt))
                .OrderBy(d => d)
                .ToList();

            var files = Directory.GetFiles(path)
                .Where(f => MatchesFilter(f, opt.FileFilters))
                .OrderBy(f => f)
                .ToList();

            // Files under this folder
            foreach (var file in files)
            {
                string relPath = Path.GetRelativePath(opt.RootPath, file).Replace("\\", "/");
                string name = Path.GetFileName(file);
                var info = new FileInfo(file);

                sb.Append(indent)
                  .Append("  - [")
                  .Append(name)
                  .Append("](")
                  .Append(relPath)
                  .Append(")  (")
                  .Append(FormatSize(info.Length))
                  .Append(", ")
                  .Append(info.LastWriteTime.ToString("yyyy-MM-dd HH:mm"))
                  .Append(")")
                  .AppendLine();
            }

            if (!opt.IncludeAllSubfolders)
                return;

            // Subfolders
            foreach (var dir in dirs)
            {
                MarkdownFolder(dir, sb, level + 1, opt);
            }
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

        // =====================================================
        // HELPERS
        // =====================================================
        private static bool FolderIsExcluded(string folderName, DirectoryScanOptions opt)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                return false;

            string lower = folderName.ToLowerInvariant();

            // These are the STATIC folders that correspond to your checkboxes
            bool isStatic =
                lower == ".git" ||
                lower == ".vs" ||
                lower == "bin" ||
                lower == "obj" ||
                lower == "properties" ||
                lower == "resources";

            // Non-static folders (Core, UI, etc.) are ALWAYS included
            if (!isStatic)
                return false;

            // For static folders: ONLY include if they are in IncludedFolders
            if (opt.IncludedFolders == null || opt.IncludedFolders.Count == 0)
                return true; // none checked => skip all static

            bool isIncluded = opt.IncludedFolders
                .Any(f => string.Equals(f, folderName, StringComparison.OrdinalIgnoreCase));

            return !isIncluded; // excluded if NOT in IncludedFolders
        }

        private static bool MatchesFilter(string file, List<string> filters)
        {
            if (filters == null || filters.Count == 0)
                return true;

            string ext = Path.GetExtension(file).ToLowerInvariant();

            return filters.Any(f =>
                f.StartsWith("*.") &&
                ext == f.Substring(1).ToLowerInvariant());
        }
    }
}
