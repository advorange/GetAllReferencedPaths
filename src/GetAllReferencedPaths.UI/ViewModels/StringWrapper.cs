using ReactiveUI;

namespace GetAllReferencedPaths.UI.ViewModels;

public class StringWrapper : ViewModelBase
{
	private string _Value;

	public string Value
	{
		get => _Value;
		set => this.RaiseAndSetIfChanged(ref _Value, value);
	}

	public StringWrapper(string value)
	{
		_Value = value;
	}
}