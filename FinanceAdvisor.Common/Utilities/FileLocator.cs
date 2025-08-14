using System.Reflection;

namespace FinanceAdvisor.Common.Utilities
{
    public static class FileLocator
    {
        /// <summary>
        /// Searches for a JSON file in the specified base directory (and optionally subdirectories)
        /// that ends with the given fileNamePattern (case-insensitive).
        /// </summary> 
        public static string? FindJsonFile(string fileNamePattern, bool searchSubDirectories = true)
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (assemblyLocation == null)
                return null;

            var searchOption = searchSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(assemblyLocation, "*.json", searchOption);

            return files.FirstOrDefault(f => f.EndsWith(fileNamePattern, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a file path relative to the solution's root directory.
        /// Useful for ensuring consistent paths in development and production.
        /// </summary>
        public static string GetFilePathFromRoot(params string[] relativeParts)
        {
            var root = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName;
            if (root == null) throw new InvalidOperationException("Solution root not found.");

            return Path.Combine(new[] { root }.Concat(relativeParts).ToArray());
        }
    }
}
