using ReactiveUI;

using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class RootDirectoryViewModel : PathCollectionViewModel
{
	public RootDirectoryViewModel(StringWrapper baseDirectory, string value)
		: base(value)
	{
		var dirChange = baseDirectory.WhenAnyValue(x => x.Value);
		BindToPaths(dirChange, (val, dir) =>
		{
			return new[] { PathViewModel.FromDirectory(Path.Combine(dir, val)) };
		});
	}
}