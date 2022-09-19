namespace Common
{
    public static class Config
    {
        public static string DatabasePath { get; } = "database.db";
        public static string DataSourcePath { get; } = "/Users/r/DLS/source";
        public static int NumberOfFoldersToIndex { get; } = 10; // Use 0 or less for indexing all folders
    }
}