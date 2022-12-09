using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using GetAllReferencedPaths.UI.ViewModels;
using GetAllReferencedPaths.UI.Views;

namespace GetAllReferencedPaths.UI;

public sealed class App : Application
{
	public override void Initialize()
		=> AvaloniaXamlLoader.Load(this);

	public override void OnFrameworkInitializationCompleted()
	{
		var args = new Arguments(
			BaseDirectory: @"C:\Users\User\Source\Repos\GetAllReferencedPaths\src\GetAllReferencedPaths",
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
				"./sim",
				"./game"
			},
			SourceFiles: new()
			{
				"./input.jsim"
			}
		);

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var window = new MainWindow();
			window.DataContext = new MainWindowViewModel(window, args);
			desktop.MainWindow = window;
		}

		base.OnFrameworkInitializationCompleted();
	}
}