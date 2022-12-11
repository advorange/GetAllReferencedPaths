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
using System.Reactive.Linq;
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

	#region IObservables
	public IObservable<bool> OutputActive { get; }
	#endregion IObservables

	#region Commands
	public ReactiveCommand<Unit, Unit> ClearResults { get; }
	public ReactiveCommand<Unit, Unit> CopyResults { get; }
	public ReactiveCommand<Unit, Unit> GetPaths { get; }
	public ReactiveCommand<Unit, Unit> SelectBaseDirectory { get; }
	#endregion Commands

	public MainWindowViewModel(Window window, ArgumentsViewModel args)
	{
		_Window = window;
		Args = args;

		OutputActive = this
			.WhenAnyValue(x => x.Results.Found)
			.Select(x => x != 0);

		ClearResults = ReactiveCommand.CreateFromTask(ClearResultsAsync, OutputActive);

		var outputInactive = OutputActive.Select(x => !x);
		GetPaths = ReactiveCommand.CreateFromTask(GetPathsAsync, outputInactive);
		SelectBaseDirectory = ReactiveCommand.CreateFromTask(SelectBaseDirectoryAsync, outputInactive);

		var outputCopyable = OutputActive
			.CombineLatest(GetPaths.IsExecuting, (act, exe) => act && !exe);
		CopyResults = ReactiveCommand.CreateFromTask(CopyResultsAsync, outputCopyable);
	}

	private Task ClearResultsAsync()
	{
		Results = new();
		return Task.CompletedTask;
	}

	private async Task CopyResultsAsync()
	{
		static IEnumerable<FileViewModel> EnumerateFiles(DirectoryViewModel dir)
		{
			foreach (var file in dir.Files)
			{
				yield return file;
			}
			foreach (var subdir in dir.Subdirectories)
			{
				foreach (var file in EnumerateFiles(subdir))
				{
					yield return file;
				}
			}
		}

		var baseDir = Args.BaseDirectory.Value!;
		var outDir = Args.OutputDirectory.Value!;
		var time = DateTime.Now.ToString("s").Replace(':', '.')!;
		var files = EnumerateFiles(Results.Output!).ToList();

		// reset each IsCopied to false in case the user clicks copy multiple times
		// otherwise if 1st copy is successful but subsequent ones arent the
		// failed copies will look successful
		foreach (var file in files)
		{
			file.IsCopied = false;
		}
		foreach (var file in files)
		{
			var destination = Path.Combine(
				baseDir,
				outDir,
				time,
				file.Relative
			);
			Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

			using var inputStream = file.Info.OpenRead();
			using var outputStream = File.Create(destination);
			await inputStream.CopyToAsync(outputStream).ConfigureAwait(true);

			file.IsCopied = true;
		}
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