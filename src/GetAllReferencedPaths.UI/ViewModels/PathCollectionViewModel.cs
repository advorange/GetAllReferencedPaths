using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels;

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
		Func<T, string, IReadOnlyList<PathViewModel>> selector)
	{
		var valueChanged = this.WhenAnyValue(x => x.Value);
		return observable.CombineLatest(valueChanged).Select(tuple =>
		{
			var (dir, val) = tuple;
			return selector(dir, val);
		}).Subscribe(x => Paths = x);
	}
}