using Avalonia.Controls;

using GetAllReferencedPaths.UI.ViewModels.Arguments;
using GetAllReferencedPaths.UI.ViewModels.Output;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
	private readonly Window _Window;

	private ResultsViewModel _Searching = new();

	public ArgumentsViewModel Args { get; }
	public ResultsViewModel Results
	{
		get => _Searching;
		set => this.RaiseAndSetIfChanged(ref _Searching, value);
	}

	#region Commands
	public ReactiveCommand<Unit, Unit> ClearResults { get; }
	public ReactiveCommand<Unit, Unit> CopyFiles { get; }
	public ReactiveCommand<Unit, Unit> GetPaths { get; }
	public ReactiveCommand<Unit, Unit> SelectBaseDirectory { get; }
	#endregion Commands

	public MainWindowViewModel(Window window, GetAllReferencedPaths.Arguments args)
	{
		_Window = window;
		Args = new(args);

		ClearResults = ReactiveCommand.CreateFromTask(ClearResultsAsync);
		CopyFiles = ReactiveCommand.CreateFromTask(CopyFilesAsync);
		GetPaths = ReactiveCommand.CreateFromTask(GetPathsAsync);
		SelectBaseDirectory = ReactiveCommand.CreateFromTask(SelectBaseDirectoryAsync);
	}

	private Task ClearResultsAsync()
	{
		Results = new();
		return Task.CompletedTask;
	}

	private Task CopyFilesAsync()
	{
		var baseDir = Args.BaseDirectory.Value!;
		var time = DateTime.Now.ToString("s").Replace(':', '.')!;

		void CopyFiles(DirectoryViewModel dirVM)
		{
			foreach (var file in dirVM.Files)
			{
				var destination = Path.Combine(
					baseDir,
					time,
					file.Relative
				);
				Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
				File.Copy(file.Info.FullName, destination);
				Debug.WriteLine($"Copied: {file.Relative} -> {destination}");
			}
			foreach (var dir in dirVM.Subdirectories)
			{
				CopyFiles(dir);
			}
		}

		CopyFiles(Results.Output!);
		return Task.CompletedTask;
	}

	private async Task GetPathsAsync()
	{
		void AddFileToTree(DirectoryInfo baseDirectory, FileInfo file)
		{
			var segments = new List<string>();
			var dir = file.Directory;
			while (baseDirectory.FullName != dir!.FullName)
			{
				segments.Add(dir.Name);
				dir = dir.Parent;
			}
			// reverse so the segments go in order from nearest to the base dir
			// to farthest
			segments.Reverse();

			var output = Results.Output!;
			foreach (var segment in segments)
			{
				var subDir = output.Subdirectories
					.FirstOrDefault(x => x.Info.Name == segment);
				if (subDir is null)
				{
					subDir = new(new(Path.Combine(output.Info.FullName, segment)));
					output.Subdirectories.Add(subDir);
				}
				output = subDir;
			}
			output.Files.Add(new(baseDirectory, file));
		}

		var args = RuntimeArguments.Create(Args.ToModel());
		Results = new()
		{
			Output = new(args.BaseDirectory),
		};

		var sw = Stopwatch.StartNew();
		var alreadyProcessed = new HashSet<string>();
		var filesToProcess = new Stack<FileInfo>(args.Sources);
		while (filesToProcess.TryPop(out var file))
		{
			Results.Remaining = filesToProcess.Count;
			if (!alreadyProcessed.Add(Path.GetFullPath(file.FullName)))
			{
				continue;
			}

			Results.CurrentFile = file.FullName;
			Results.Found = alreadyProcessed.Count;
			AddFileToTree(args.BaseDirectory, file);

			foreach (var gatherer in args.Gatherers)
			{
				var result = await gatherer.GetStringsAsync(file).ConfigureAwait(true);
				if (!result.IsSuccess)
				{
					continue;
				}

				foreach (var rootedFile in gatherer.RootFiles(file, result.Value))
				{
					filesToProcess.Push(rootedFile);
					Results.Remaining = filesToProcess.Count;
				}
			}
		}

		sw.Stop();
		Results.EllapsedTime = sw.Elapsed;
		Results.CurrentFile = null;
	}

	private async Task SelectBaseDirectoryAsync()
	{
		var path = await _Window.GetDirectoryAsync(Args.BaseDirectory.Value).ConfigureAwait(true);
		if (string.IsNullOrWhiteSpace(path))
		{
			return;
		}

		Args.BaseDirectory.Value = path;
	}
}