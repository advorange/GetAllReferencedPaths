using GetAllReferencedPaths.UI.ViewModels.Output;

using ReactiveUI;

using System;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class ResultsViewModel : ViewModelBase
{
	private string? _CurrentlyProcessingFile;
	private int _Found;
	private TimeSpan _LastSearchTime;
	private DirectoryViewModel? _Output;
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
	public int Found
	{
		get => _Found;
		set => this.RaiseAndSetIfChanged(ref _Found, value);
	}
	public DirectoryViewModel? Output
	{
		get => _Output;
		set => this.RaiseAndSetIfChanged(ref _Output, value);
	}
	public int Remaining
	{
		get => _Remaining;
		set => this.RaiseAndSetIfChanged(ref _Remaining, value);
	}
}