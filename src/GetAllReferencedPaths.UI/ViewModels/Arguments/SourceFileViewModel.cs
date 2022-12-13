using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class SourceFileViewModel : PathCollectionViewModel
{
	public SourceFileViewModel(
		IObservable<List<DirectoryInfo>> roots,
		IObservable<List<HashSet<string>>> fileTypes,
		string value)
		: base(value)
	{
		BindToPaths(roots.CombineLatest(fileTypes), (value, tuple) =>
		{
			var (roots, fileTypes) = tuple;
			return fileTypes
				.InterchangeFileTypes(value)
				.SelectMany(x => roots.RootFile(x, existingFilesOnly: false))
				.OrderBy(x => x.DirectoryName)
				.ThenBy(x => x.Name)
				.Select(f => PathViewModel.FromFile(f.FullName))
				.ToList();
		});
	}
}