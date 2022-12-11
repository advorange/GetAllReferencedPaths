using System;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed record PathViewModel(
	IObservable<bool> PathExists,
	string Path
)
{
	public static PathViewModel FromFile(string path)
		=> Create(path, System.IO.File.Exists);

	public static PathViewModel FromDirectory(string path)
		=> Create(path, System.IO.Directory.Exists);

	private static PathViewModel Create(string path, Func<string?, bool> existence)
	{
		var fullPath = System.IO.Path.GetFullPath(path);
		// poll for existence of path
		var observable = Observable
			.Interval(TimeSpan.FromSeconds(1))
			.Select(_ => existence(fullPath))
			.Prepend(existence(fullPath));
		return new(PathExists: observable, Path: fullPath);
	}
}