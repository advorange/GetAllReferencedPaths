using System;
using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels;

public sealed class CopyFileViewModel : ViewModelBase
{
	public string Relative { get; }
	public string Source { get; }
	public string Time { get; }

	public CopyFileViewModel(RuntimeArguments args, DateTime time, FileInfo file)
	{
		Source = file.FullName;
		Relative = Path.GetRelativePath(args.BaseDirectory.FullName, Source);
		Time = time.ToString("s").Replace(':', '.');
	}
}