using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public abstract class PathCollectionViewModel : StringWrapper
{
	private IReadOnlyList<PathViewModel> _Paths = Array.Empty<PathViewModel>();

	public IReadOnlyList<PathViewModel> Paths
	{
		get => _Paths;
		protected set => this.RaiseAndSetIfChanged(ref _Paths, value);
	}

	protected PathCollectionViewModel(string value) : base(value)
	{
	}

	protected IDisposable BindToPaths<T>(
		IObservable<T> observable,
		Func<string, T, IReadOnlyList<PathViewModel>> selector)
	{
		return this
			.WhenAnyValue(x => x.Value)
			.CombineLatest(observable, selector)
			.Subscribe(x => Paths = x);
	}
}