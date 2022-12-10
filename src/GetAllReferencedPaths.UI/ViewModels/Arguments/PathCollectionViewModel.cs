using ReactiveUI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public abstract class PathCollectionViewModel : StringWrapper, ICanDetach
{
	private readonly IList? _Owner;

	private IReadOnlyList<PathViewModel> _Paths = Array.Empty<PathViewModel>();

	public IReadOnlyList<PathViewModel> Paths
	{
		get => _Paths;
		protected set => this.RaiseAndSetIfChanged(ref _Paths, value);
	}

	protected PathCollectionViewModel(IList? owner, string value) : base(value)
	{
		_Owner = owner;
	}

	void ICanDetach.RemoveFromOwner()
		=> _Owner?.Remove(this);

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