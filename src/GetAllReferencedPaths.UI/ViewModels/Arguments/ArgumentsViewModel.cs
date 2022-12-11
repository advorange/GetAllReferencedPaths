using ReactiveUI;

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
		OutputDirectory = new(BaseDirectory, args.OutputDirectory);

		foreach (var item in args.RootDirectories)
		{
			AddRootDirectory(item);
		}
		foreach (var item in args.SourceFiles)
		{
			AddSourceFile(item);
		}

		foreach (var set in args.InterchangeableFileTypes)
		{
			var list = new FileTypeGroupViewModel();
			foreach (var item in set)
			{
				list.Add(new(item));
			}
			InterchangeableFileTypes.Add(list);
		}

		NewRootDirectory = ReactiveCommand.Create(() => AddRootDirectory());
		NewSourceFile = ReactiveCommand.Create(() => AddSourceFile());
		NewFileTypeGroup = ReactiveCommand.Create(() =>
		{
			InterchangeableFileTypes.Add(new());
		});
		NewFileType = ReactiveCommand.Create<FileTypeGroupViewModel>(collection =>
		{
			collection.Add(new(""));
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
		=> RootDirectories.Add(new(BaseDirectory, value));

	public void AddSourceFile(string value = "")
		=> SourceFiles.Add(new(BaseDirectory, RootDirectories, value));

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