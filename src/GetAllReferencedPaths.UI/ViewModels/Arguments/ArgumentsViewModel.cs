using System.Collections.ObjectModel;
using System.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class ArgumentsViewModel : ViewModelBase
{
	public StringWrapper BaseDirectory { get; }
	public ObservableCollection<ObservableCollection<StringWrapper>> InterchangeableFileTypes { get; } = new();
	public RootDirectoryViewModel OutputDirectory { get; }
	public ObservableCollection<RootDirectoryViewModel> RootDirectories { get; } = new();
	public ObservableCollection<SourceFileViewModel> SourceFiles { get; } = new();

	public ArgumentsViewModel(GetAllReferencedPaths.Arguments args)
	{
		BaseDirectory = new(args.BaseDirectory);
		OutputDirectory = new(this, args.OutputDirectory);

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
			var list = new ObservableCollection<StringWrapper>();
			foreach (var item in set)
			{
				list.Add(new(item));
			}
			InterchangeableFileTypes.Add(list);
		}
	}

	public void AddRootDirectory(string value = "")
		=> RootDirectories.Add(new(this, value));

	public void AddSourceFile(string value = "")
		=> SourceFiles.Add(new(this, value));

	public GetAllReferencedPaths.Arguments ToModel()
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