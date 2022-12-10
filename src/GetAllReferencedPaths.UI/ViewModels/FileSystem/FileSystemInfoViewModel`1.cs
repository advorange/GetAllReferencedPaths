﻿using ReactiveUI;

using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.FileSystem;

public abstract class FileSystemInfoViewModel<T> : ViewModelBase, IFileSystemInfoViewModel
	where T : FileSystemInfo
{
	public T Info { get; }

	FileSystemInfo IFileSystemInfoViewModel.Info => Info;

	protected FileSystemInfoViewModel(T info)
	{
		Info = info;
	}
}