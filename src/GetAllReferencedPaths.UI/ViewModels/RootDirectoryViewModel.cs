using ReactiveUI;

using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class RootDirectoryViewModel : PathCollectionViewModel
{
	public RootDirectoryViewModel(StringWrapper baseDirectory, string value) : base(value)
	{
		var dirChange = baseDirectory.WhenAnyValue(x => x.Value);
		var valChange = this.WhenAnyValue(x => x.Value);
		dirChange.CombineLatest(valChange).Select(tuple =>
		{
			var (dir, val) = tuple;
			return new[] { PathViewModel.RootDirectory(Path.Combine(dir, val)) };
		}).BindTo(this, x => x.Paths);
	}
}