using DynamicData;
using DynamicData.Binding;

using System;
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
		ObservableCollection<FileTypeGroupViewModel> fileTypes,
		string value)
		: base(value)
	{
		var rootsChanged = rootDirectories
			.ToObservableChangeSet()
			.AutoRefresh(x => x.Paths)
			.ToCollection()
			.Select(x =>
			{
				return x
					.SelectMany(r => r.Paths.Select(p => new DirectoryInfo(p.Path)))
					.Prepend(new(baseDirectory.Value))
					.ToList();
			});
		var fileTypesChanged = fileTypes
			.ToObservableChangeSet()
			.AutoRefresh(x => x.Display)
			.ToCollection()
			.Select(x =>
			{
				return x
					.Select(g => g.Select(e => e.Value).ToHashSet())
					.ToList();
			});
		var otherChanged = rootsChanged.CombineLatest(fileTypesChanged);
		BindToPaths(otherChanged, (val, tuple) =>
		{
			var (roots, fileTypes) = tuple;
			return fileTypes
				.InterchangeFileTypes(val)
				.SelectMany(x => roots.RootFile(x, existingFilesOnly: false))
				.OrderBy(x => x.DirectoryName)
				.ThenBy(x => x.Name)
				.Select(f => PathViewModel.FromFile(f.FullName))
				.ToList();
		});
	}
}