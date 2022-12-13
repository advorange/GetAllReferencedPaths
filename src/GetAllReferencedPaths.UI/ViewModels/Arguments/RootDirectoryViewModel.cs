using System;
using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class RootDirectoryViewModel : PathCollectionViewModel
{
	public RootDirectoryViewModel(IObservable<string> baseDirectory, string value)
		: base(value)
	{
		BindToPaths(baseDirectory, (value, directory) =>
		{
			return new[]
			{
				PathViewModel.FromDirectory(Path.Combine(directory, value))
			};
		});
	}
}