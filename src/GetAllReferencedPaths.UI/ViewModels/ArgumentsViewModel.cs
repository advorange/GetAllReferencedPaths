using System.Collections.ObjectModel;
using System.Linq;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class ArgumentsViewModel : ViewModelBase
{
	public StringWrapper BaseDirectory { get; set; }
	public ObservableCollection<ObservableCollection<StringWrapper>> InterchangeableFileTypes { get; set; } = new();
	public RootDirectoryViewModel OutputDirectory { get; set; }
	public ObservableCollection<RootDirectoryViewModel> RootDirectories { get; set; } = new();
	public ObservableCollection<SourceFileViewModel> SourceFiles { get; set; } = new();

	public ArgumentsViewModel(Arguments args)
	{
		BaseDirectory = new(args.BaseDirectory);
		OutputDirectory = new(BaseDirectory, args.OutputDirectory);

		foreach (var item in args.RootDirectories)
		{
			RootDirectories.Add(new(BaseDirectory, item));
		}
		foreach (var item in args.SourceFiles)
		{
			SourceFiles.Add(new(RootDirectories, item));
		}

		foreach (var set in args.InterchangeableFileTypes)
		{
			var list = new ObservableCollection<StringWrapper>();
			foreach (var item in set)
			{
				list.Add(new(item));
			}
			InterchangeableFileTypes.Add(list);
		}
	}

	public void AddRootDirectory()
		=> RootDirectories.Add(new(BaseDirectory, ""));

	public void AddSourceFile()
		=> SourceFiles.Add(new(RootDirectories, ""));

	public Arguments ToModel()
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