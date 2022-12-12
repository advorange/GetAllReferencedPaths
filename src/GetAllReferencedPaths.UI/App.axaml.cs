using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using GetAllReferencedPaths.UI.ViewModels;
using GetAllReferencedPaths.UI.ViewModels.Arguments;
using GetAllReferencedPaths.UI.Views;

using System;

namespace GetAllReferencedPaths.UI;

public sealed class App : Application
{
	public override void Initialize()
		=> AvaloniaXamlLoader.Load(this);

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var argsPath = System.IO.Path.Combine(
				Environment.CurrentDirectory,
				"settings.json"
			);
			var args = ArgumentsViewModel.Load(argsPath);

			var window = new MainWindow();
			window.DataContext = new MainWindowViewModel(window, args);
			desktop.MainWindow = window;

			desktop.ShutdownRequested += (_, _) =>
			{
				args.Save(argsPath);
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
}