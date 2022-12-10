using DynamicData;
using DynamicData.Binding;

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class SourceFileViewModel : PathCollectionViewModel
{
	public SourceFileViewModel(ObservableCollection<RootDirectoryViewModel> roots, string value) : base(value)
	{
		var rootsChange = roots
			.ToObservableChangeSet()
			.AutoRefresh(x => x.Paths)
			.ToCollection();
		BindToPaths(rootsChange, (roots, val) =>
		{
			return roots.SelectMany(r =>
			{
				return r.Paths
					.Select(p => new DirectoryInfo(p.Path))
					.RootFile(val, existingFilesOnly: false)
					.Select(f => PathViewModel.FromFile(f.FullName));
			}).ToList();
		});
	}
}