using ReactiveUI;

using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class RootDirectoryViewModel : PathCollectionViewModel
{
	public RootDirectoryViewModel(StringWrapper baseDirectory, string value)
		: base(value)
	{
		var directoryChanged = baseDirectory
			.WhenAnyValue(x => x.Value);
		BindToPaths(directoryChanged, (val, dir) =>
		{
			return new[] { PathViewModel.FromDirectory(Path.Combine(dir, val)) };
		});
	}
}