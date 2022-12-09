using ReactiveUI;

using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class DirectoryViewModel : PathViewModel
{
	public DirectoryViewModel(StringWrapper baseDirectory, string value) : base(value)
	{
		var dirChange = baseDirectory.WhenAnyValue(x => x.Value);
		var valChange = this.WhenAnyValue(x => x.Value);
		dirChange.CombineLatest(valChange).Select(x =>
		{
			var (dir, val) = x;
			return new[] { Path.GetFullPath(Path.Combine(dir, val)) };
		}).BindTo(this, x => x.Paths);
	}
}
