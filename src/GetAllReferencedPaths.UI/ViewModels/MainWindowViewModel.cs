using ReactiveUI;

using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;

namespace GetAllReferencedPaths.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public Arguments Args { get; }
	public ObservableCollection<string> Strings { get; } = new()
	{
		"123",
	};

	#region Commands
	public ReactiveCommand<Unit, Unit> GetPaths { get; }
	#endregion Commands

	public MainWindowViewModel()
	{
		GetPaths = ReactiveCommand.CreateFromTask(GetPathsAsync);

		Args = new Arguments(
			BaseDirectory: Directory.GetCurrentDirectory(),
			InterchangeableFileTypes: new()
			{
				new()
				{
					".jsim", ".jresource"
				},
			},
			OutputDirectory: "../ReferencedFilesOutput",
			RootDirectories: new()
			{
				"./sim"
			},
			SourceFiles: new()
			{
				"./input.jsim"
			}
		);
	}

	private Task GetPathsAsync()
	{
		return Task.CompletedTask;
	}
}