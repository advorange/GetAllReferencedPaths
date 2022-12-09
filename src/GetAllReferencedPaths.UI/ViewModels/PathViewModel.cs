using IO = System.IO;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed record PathViewModel(
	bool IsValid,
	string Path
)
{
	public static PathViewModel SourceFile(string path)
		=> new(IO.File.Exists(path), IO.Path.GetFullPath(path));

	public static PathViewModel RootDirectory(string path)
		=> new(IO.Directory.Exists(path), IO.Path.GetFullPath(path));
}