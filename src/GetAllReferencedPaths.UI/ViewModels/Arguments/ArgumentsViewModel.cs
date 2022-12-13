using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;

using Args = GetAllReferencedPaths.Arguments;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class ArgumentsViewModel : ViewModelBase
{
	private static readonly JsonSerializerOptions _Options = new()
	{
		WriteIndented = true,
	};

	private readonly IObservable<string> _BaseDirectoryChanged;
	private readonly IObservable<List<HashSet<string>>> _FileTypesChanged;
	private readonly IObservable<List<DirectoryInfo>> _RootsChanged;

	public StringWrapper BaseDirectory { get; }
	public ObservableCollection<FileTypeGroupViewModel> InterchangeableFileTypes { get; } = new();
	public RootDirectoryViewModel OutputDirectory { get; }
	public ObservableCollection<RootDirectoryViewModel> RootDirectories { get; } = new();
	public ObservableCollection<SourceFileViewModel> SourceFiles { get; } = new();

	#region Commands
	public ReactiveCommand<FileTypeGroupViewModel, Unit> NewFileType { get; }
	public ReactiveCommand<Unit, Unit> NewFileTypeGroup { get; }
	public ReactiveCommand<Unit, Unit> NewRootDirectory { get; }
	public ReactiveCommand<Unit, Unit> NewSourceFile { get; }
	#endregion Commands

	public ArgumentsViewModel(Args args)
	{
		BaseDirectory = new(args.BaseDirectory);
		_BaseDirectoryChanged = BaseDirectory
			.WhenAnyValue(x => x.Value);

		OutputDirectory = new(_BaseDirectoryChanged, args.OutputDirectory);

		foreach (var set in args.InterchangeableFileTypes)
		{
			var list = new FileTypeGroupViewModel();
			foreach (var item in set)
			{
				list.Add(new(item));
			}
			InterchangeableFileTypes.Add(list);
		}
		_FileTypesChanged = InterchangeableFileTypes
			.ToObservableChangeSet()
			.AutoRefresh(x => x.Display)
			.ToCollection()
			.Select(x =>
			{
				return x
					.Select(g => g.Select(e => e.Value).ToHashSet())
					.ToList();
			});

		foreach (var item in args.RootDirectories)
		{
			AddRootDirectory(item);
		}
		_RootsChanged = RootDirectories
			.ToObservableChangeSet()
			.AutoRefresh(x => x.Paths)
			.ToCollection()
			.Select(x =>
			{
				return x
					.SelectMany(r => r.Paths.Select(p => new DirectoryInfo(p.Path)))
					.Prepend(new(BaseDirectory.Value))
					.ToList();
			});

		foreach (var item in args.SourceFiles)
		{
			AddSourceFile(item);
		}

		NewRootDirectory = ReactiveCommand.Create(() => AddRootDirectory());
		NewSourceFile = ReactiveCommand.Create(() => AddSourceFile());
		NewFileTypeGroup = ReactiveCommand.Create(() =>
		{
			InterchangeableFileTypes.Add(new()
			{
				new(""),
				new(""),
			});
		});
		NewFileType = ReactiveCommand.Create<FileTypeGroupViewModel>(group =>
		{
			group.Add(new(""));
		});
	}

	public static ArgumentsViewModel Load(string path)
	{
		Args? args = null;
		if (File.Exists(path))
		{
			using var fs = File.OpenRead(path);
			args = JsonSerializer.Deserialize<Args>(fs, _Options);
		}
		return new(args ?? new(
			BaseDirectory: Path.GetDirectoryName(path)!,
			InterchangeableFileTypes: new(),
			OutputDirectory: "../Output",
			RootDirectories: new(),
			SourceFiles: new()
		));
	}

	public void AddRootDirectory(string value = "")
		=> RootDirectories.Add(new(_BaseDirectoryChanged, value));

	public void AddSourceFile(string value = "")
		=> SourceFiles.Add(new(_RootsChanged, _FileTypesChanged, value));

	public void Save(string path)
	{
		var json = JsonSerializer.Serialize(ToModel(), _Options);
		File.WriteAllText(path, json);
	}

	public Args ToModel()
	{
		return new(
			BaseDirectory: BaseDirectory.Value!,
			InterchangeableFileTypes: InterchangeableFileTypes
				.Select(x => x.Select(x => x.Value!).ToHashSet()).ToList(),
			OutputDirectory: OutputDirectory.Value!,
			RootDirectories: RootDirectories.Select(x => x.Value!).ToList(),
			SourceFiles: SourceFiles.Select(x => x.Value!).ToList()
		);
	}
}