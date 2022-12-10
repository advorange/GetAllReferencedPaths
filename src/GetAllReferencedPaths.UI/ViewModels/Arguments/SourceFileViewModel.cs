using DynamicData;
using DynamicData.Binding;

using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class SourceFileViewModel : PathCollectionViewModel
{
	public SourceFileViewModel(ArgumentsViewModel vm, string value)
		: base(vm.SourceFiles, value)
	{
		var rootsChange = vm.RootDirectories
			.ToObservableChangeSet()
			.AutoRefresh(x => x.Paths)
			.ToCollection();
		BindToPaths(rootsChange, (roots, val) =>
		{
			return roots
				.SelectMany(r => r.Paths.Select(p => new DirectoryInfo(p.Path)))
				.Prepend(new(vm.BaseDirectory.Value))
				.RootFile(val, existingFilesOnly: false)
				.ConvertAll(f => PathViewModel.FromFile(f.FullName));
		});
	}
}