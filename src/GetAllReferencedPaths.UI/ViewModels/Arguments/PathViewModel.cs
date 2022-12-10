using IO = System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed record PathViewModel(
    bool IsValid,
    string Path
)
{
    public static PathViewModel FromFile(string path)
        => new(IO.File.Exists(path), IO.Path.GetFullPath(path));

    public static PathViewModel FromDirectory(string path)
        => new(IO.Directory.Exists(path), IO.Path.GetFullPath(path));
}