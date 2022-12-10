using DynamicData;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Output;

public sealed class DirectoryViewModel : FileSystemInfoViewModel<DirectoryInfo>
{
	private static readonly Comparer<IFileSystemInfoViewModel> _Comparer
		= Comparer<IFileSystemInfoViewModel>.Create(
			(x, y) => x.Info.Name.CompareTo(y.Info.Name)
		);

	public SortedObservableCollection<FileViewModel> Files { get; }
		= new(_Comparer);
	public IEnumerable<IFileSystemInfoViewModel> Items
		=> Subdirectories.Cast<IFileSystemInfoViewModel>().Concat(Files);
	public SortedObservableCollection<DirectoryViewModel> Subdirectories { get; }
		= new(_Comparer);

	public DirectoryViewModel(DirectoryInfo directory) : base(directory)
	{
		var filesChanged = Files.WhenAnyValue(x => x.Count);
		var dirsChanged = Subdirectories.WhenAnyValue(x => x.Count);
		filesChanged.CombineLatest(dirsChanged).Subscribe(_ =>
		{
			this.RaisePropertyChanged(nameof(Items));
		});
	}
}