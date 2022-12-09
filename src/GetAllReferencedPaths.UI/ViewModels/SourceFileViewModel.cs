using DynamicData;
using DynamicData.Binding;

using ReactiveUI;

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class SourceFileViewModel : PathViewModel
{
	public SourceFileViewModel(ObservableCollection<DirectoryViewModel> roots, string value) : base(value)
	{
		var rootChange = roots
			.ToObservableChangeSet()
			.AutoRefresh(x => x.Paths)
			.ToCollection();
		var valChange = this.WhenAnyValue(x => x.Value);
		rootChange.CombineLatest(valChange).Select(x =>
		{
			var (roots, val) = x;
			return roots
				.SelectMany(x => x.Paths)
				.Select(x => Path.GetFullPath(Path.Combine(x, val)))
				.ToList();
		}).BindTo(this, x => x.Paths);
	}
}
