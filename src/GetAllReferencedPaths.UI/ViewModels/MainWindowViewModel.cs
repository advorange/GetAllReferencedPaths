using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
	public ArgumentsViewModel Args { get; }
	public ObservableCollection<CopyFileViewModel> FilesToProcess { get; } = new();

	#region Commands
	public ReactiveCommand<Unit, Unit> AddRootDirectory { get; }
	public ReactiveCommand<Unit, Unit> AddSourceFile { get; }
	public ReactiveCommand<Unit, Unit> ClearPaths { get; }
	public ReactiveCommand<Unit, Unit> CopyFiles { get; }
	public ReactiveCommand<Unit, Unit> GetPaths { get; }
	#endregion Commands

	public MainWindowViewModel(Arguments args)
	{
		Args = new(args);

		AddRootDirectory = ReactiveCommand.CreateFromTask(AddRootDirectoryAsync);
		AddSourceFile = ReactiveCommand.CreateFromTask(AddSourceFileAsync);
		ClearPaths = ReactiveCommand.CreateFromTask(ClearPathsAsync);
		CopyFiles = ReactiveCommand.CreateFromTask(CopyFilesAsync);
		GetPaths = ReactiveCommand.CreateFromTask(GetPathsAsync);
	}

	private Task AddRootDirectoryAsync()
	{
		Args.AddRootDirectory();
		return Task.CompletedTask;
	}

	private Task AddSourceFileAsync()
	{
		Args.AddSourceFile();
		return Task.CompletedTask;
	}

	private Task ClearPathsAsync()
	{
		FilesToProcess.Clear();
		return Task.CompletedTask;
	}

	private async Task CopyFilesAsync()
	{
		foreach (var file in FilesToProcess)
		{
			var destination = Path.Combine(
				Args.OutputDirectory.Value!,
				file.Time,
				file.Relative
			);
			Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
			File.Copy(file.Source, destination);
			Debug.WriteLine($"Copied: {file.Relative} -> {destination}");
		}
	}

	private async Task GetPathsAsync()
	{
		FilesToProcess.Clear();

		var args = RuntimeArguments.Create(Args.ToModel());
		var alreadyProcessed = new HashSet<string>();
		var filesToProcess = new Stack<FileInfo>(args.Sources);
		while (filesToProcess.TryPop(out var file))
		{
			if (!alreadyProcessed.Add(Path.GetFullPath(file.FullName)))
			{
				continue;
			}

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
				}
			}
		}

		var time = DateTime.Now;
		foreach (var file in alreadyProcessed.OrderBy(x => x))
		{
			FilesToProcess.Add(new(args, time, new(file)));
		}
	}
}