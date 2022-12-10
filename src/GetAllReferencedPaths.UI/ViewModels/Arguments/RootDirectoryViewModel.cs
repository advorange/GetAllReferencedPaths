using ReactiveUI;

using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class RootDirectoryViewModel : PathCollectionViewModel
{
	public RootDirectoryViewModel(ArgumentsViewModel args, string value)
		: base(args.RootDirectories, value)
	{
		var dirChange = args.BaseDirectory.WhenAnyValue(x => x.Value);
		BindToPaths(dirChange, (dir, val) =>
		{
			return new[] { PathViewModel.FromDirectory(Path.Combine(dir, val)) };
		});
	}
}