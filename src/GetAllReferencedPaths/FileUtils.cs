namespace GetAllReferencedPaths;

public static class FileUtils
{
	public static async Task CopyFileAsync(string sourceFile, string destinationFile)
	{
		const int BUFFER_SIZE = 81920;
		const FileOptions FILE_OPTIONS = 0
			| FileOptions.Asynchronous
			| FileOptions.SequentialScan;

		using var sourceStream = new FileStream(
			sourceFile,
			FileMode.Open,
			FileAccess.Read,
			FileShare.Read,
			BUFFER_SIZE,
			FILE_OPTIONS
		);
		using var destinationStream = new FileStream(
			destinationFile,
			FileMode.CreateNew,
			FileAccess.Write,
			FileShare.None,
			BUFFER_SIZE,
			FILE_OPTIONS
		);

		await sourceStream.CopyToAsync(destinationStream);
	}

	public static HashSet<string> InterchangeFileTypes(
		this IEnumerable<IReadOnlySet<string>> fileTypes,
		string @string)
	{
		var sources = new HashSet<string>()
		{
			@string
		};

		var fileExtension = Path.GetExtension(@string);
		foreach (var extensions in fileTypes)
		{
			if (extensions.Contains(fileExtension))
			{
				foreach (var extension in extensions)
				{
					sources.Add(Path.ChangeExtension(@string, extension));
				}
			}
		}

		return sources;
	}

	public static List<FileInfo> RootFile(
		this IEnumerable<DirectoryInfo> roots,
		string @string,
		bool existingFilesOnly = true)
	{
		var output = new HashSet<string>();
		foreach (var root in roots)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(@string))
				{
					foreach (var file in Directory.EnumerateFiles(root.FullName, @string))
					{
						output.Add(Path.GetFullPath(file));
					}
				}
			}
			catch
			{
			}

			try
			{
				var file = Path.Join(root.FullName, @string);
				if (!existingFilesOnly || File.Exists(file))
				{
					output.Add(Path.GetFullPath(file));
				}
			}
			catch
			{
			}
		}
		return output.Select(x => new FileInfo(x)).ToList();
	}
}