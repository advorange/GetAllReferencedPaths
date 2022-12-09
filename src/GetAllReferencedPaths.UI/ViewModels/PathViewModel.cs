using ReactiveUI;

using System;
using System.Collections.Generic;

namespace GetAllReferencedPaths.UI.ViewModels;

public abstract class PathViewModel : StringWrapper
{
	private IEnumerable<string> _Paths = Array.Empty<string>();

	public IEnumerable<string> Paths
	{
		get => _Paths;
		set => this.RaiseAndSetIfChanged(ref _Paths, value);
	}

	protected PathViewModel(string value) : base(value)
	{
	}
}
