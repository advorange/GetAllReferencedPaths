using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

using ReactiveUI;

using System.IO;
using System.Reactive;
using System.Threading.Tasks;

namespace GetAllReferencedPaths.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public ArgumentsViewModel Args { get; }

	#region Commands
	public ReactiveCommand<Unit, Unit> GetPaths { get; }
	#endregion Commands

	public MainWindowViewModel()
	{
		GetPaths = ReactiveCommand.CreateFromTask(GetPathsAsync);

		Args = new(new Arguments(
			BaseDirectory: Directory.GetCurrentDirectory(),
			InterchangeableFileTypes: new()
			{
				new()
				{
					".jsim", ".jresource"
				},
				new()
				{
					".xyz", ".xyz2"
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
		));
	}

	private Task GetPathsAsync()
	{
		return Task.CompletedTask;
	}
}