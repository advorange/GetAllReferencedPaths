using GetAllReferencedPaths.UI.ViewModels.FileSystem;

using ReactiveUI;

using System;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class SearchingViewModel : ViewModelBase
{
	private string? _CurrentlyProcessingFile;
	private DirectoryViewModel? _FileSystem;
	private int _Found;
	private TimeSpan _LastSearchTime;
	private int _Remaining;

	public string? CurrentFile
	{
		get => _CurrentlyProcessingFile;
		set => this.RaiseAndSetIfChanged(ref _CurrentlyProcessingFile, value);
	}
	public TimeSpan EllapsedTime
	{
		get => _LastSearchTime;
		set => this.RaiseAndSetIfChanged(ref _LastSearchTime, value);
	}
	public DirectoryViewModel? FileSystem
	{
		get => _FileSystem;
		set => this.RaiseAndSetIfChanged(ref _FileSystem, value);
	}
	public int Found
	{
		get => _Found;
		set => this.RaiseAndSetIfChanged(ref _Found, value);
	}
	public int Remaining
	{
		get => _Remaining;
		set => this.RaiseAndSetIfChanged(ref _Remaining, value);
	}
}