namespace Pancake.Editor.Finder
{
    using System.IO;

    internal static class FileTools
    {
        public static void DeleteFile(string path)
        {
            if (!File.Exists(path)) return;
            RemoveReadOnlyAttribute(path);
            File.Delete(path);
        }

        private static void RemoveReadOnlyAttribute(string filePath)
        {
            var attributes = File.GetAttributes(filePath);
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                File.SetAttributes(filePath, attributes & ~FileAttributes.ReadOnly);
        }
    }
}