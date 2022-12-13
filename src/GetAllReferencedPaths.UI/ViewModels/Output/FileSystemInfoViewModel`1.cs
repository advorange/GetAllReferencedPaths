using ReactiveUI;

using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.Output;

public abstract class FileSystemInfoViewModel<T> : ViewModelBase, IFileSystemInfoViewModel
	where T : FileSystemInfo
{
	private bool _IsCopied;

	public T Info { get; }
	public bool IsCopied
	{
		get => _IsCopied;
		set => this.RaiseAndSetIfChanged(ref _IsCopied, value);
	}

	FileSystemInfo IFileSystemInfoViewModel.Info => Info;

	protected FileSystemInfoViewModel(T info)
	{
		Info = info;
	}
}