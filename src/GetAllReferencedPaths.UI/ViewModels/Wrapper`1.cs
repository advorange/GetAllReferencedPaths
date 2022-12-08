using ReactiveUI;

namespace GetAllReferencedPaths.UI.ViewModels;

public class Wrapper<T> : ReactiveObject
{
	private T? _Value;

	public T? Value
	{
		get => _Value;
		set => this.RaiseAndSetIfChanged(ref _Value, value);
	}
}