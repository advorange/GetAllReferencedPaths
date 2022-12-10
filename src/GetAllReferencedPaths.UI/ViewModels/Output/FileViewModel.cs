using ReactiveUI;

using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.Output;

public sealed class FileViewModel : FileSystemInfoViewModel<FileInfo>
{
	public string Relative { get; }

	public FileViewModel(DirectoryInfo baseDirectory, FileInfo file) : base(file)
	{
		Relative = Path.GetRelativePath(baseDirectory.FullName, file.FullName);
	}
}