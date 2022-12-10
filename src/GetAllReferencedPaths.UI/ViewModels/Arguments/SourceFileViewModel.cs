using DynamicData;
using DynamicData.Binding;

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class SourceFileViewModel : PathCollectionViewModel
{
	public SourceFileViewModel(
		StringWrapper baseDirectory,
		ObservableCollection<RootDirectoryViewModel> rootDirectories,
		string value)
		: base(value)
	{
		var rootsChange = rootDirectories
			.ToObservableChangeSet()
			.AutoRefresh(x => x.Paths)
			.ToCollection();
		BindToPaths(rootsChange, (roots, val) =>
		{
			return roots
				.SelectMany(r => r.Paths.Select(p => new DirectoryInfo(p.Path)))
				.Prepend(new(baseDirectory.Value))
				.RootFile(val, existingFilesOnly: false)
				.ConvertAll(f => PathViewModel.FromFile(f.FullName));
		});
	}
}