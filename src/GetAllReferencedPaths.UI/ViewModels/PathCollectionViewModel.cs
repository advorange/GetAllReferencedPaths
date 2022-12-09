using ReactiveUI;

using System;
using System.Collections.Generic;

namespace GetAllReferencedPaths.UI.ViewModels;

public abstract class PathCollectionViewModel : StringWrapper
{
	private IReadOnlyList<PathViewModel> _Paths = Array.Empty<PathViewModel>();

	public IReadOnlyList<PathViewModel> Paths
	{
		get => _Paths;
		set => this.RaiseAndSetIfChanged(ref _Paths, value);
	}

	protected PathCollectionViewModel(string value) : base(value)
	{
	}
}