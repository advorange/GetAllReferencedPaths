using ReactiveUI;

using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.Output;

public sealed class FileViewModel : FileSystemInfoViewModel<FileInfo>
{
	private bool _IsCopied;

	public bool IsCopied
	{
		get => _IsCopied;
		set => this.RaiseAndSetIfChanged(ref _IsCopied, value);
	}
	public string Relative { get; }

	public FileViewModel(DirectoryInfo baseDirectory, FileInfo file) : base(file)
	{
		Relative = Path.GetRelativePath(baseDirectory.FullName, file.FullName);
	}
}