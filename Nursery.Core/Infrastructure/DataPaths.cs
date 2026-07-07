namespace Nursery.Core.Infrastructure;

public static class DataPaths
{
    public static readonly string DataDirectory =
        Path.Combine(GetSolutionRoot(), "data");
    
    public static readonly string NurseryDbFile =
        Path.Combine(DataDirectory, "nursery.db");

    private static string GetSolutionRoot()
    {
        var dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (dir != null && !dir.GetFiles("*.sln").Any())
            dir = dir.Parent;
        return dir?.FullName ?? AppDomain.CurrentDomain.BaseDirectory;
    }
}