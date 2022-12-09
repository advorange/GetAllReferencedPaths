using Avalonia.Controls.Shapes;

using DynamicData;
using DynamicData.Binding;

using GetAllReferencedPaths.Gatherers;

using ReactiveUI;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class SourceFileViewModel : PathCollectionViewModel
{
	public SourceFileViewModel(ObservableCollection<RootDirectoryViewModel> roots, string value) : base(value)
	{
		var rootChange = roots
			.ToObservableChangeSet()
			.AutoRefresh(x => x.Paths)
			.ToCollection();
		var valChange = this.WhenAnyValue(x => x.Value);
		rootChange.CombineLatest(valChange).Select(tuple =>
		{
			var (roots, val) = tuple;
			var output = new List<PathViewModel>();
			foreach (var root in roots)
			{
				var dirs = root.Paths.Select(x => new DirectoryInfo(x.Path));
				var files = GathererBase.RootFile(dirs, val, existingFilesOnly: false)
					.Select(x => PathViewModel.SourceFile(x.FullName));
				output.AddRange(files);
			}
			return output;
		}).BindTo(this, x => x.Paths);
	}
}