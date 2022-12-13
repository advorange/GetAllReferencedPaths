using System.IO;

namespace GetAllReferencedPaths.UI.ViewModels.Output;

public interface IFileSystemInfoViewModel
{
	FileSystemInfo Info { get; }
	bool IsCopied { get; set; }
}