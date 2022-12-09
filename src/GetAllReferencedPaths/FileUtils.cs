namespace GetAllReferencedPaths;

public static class FileUtils
{
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