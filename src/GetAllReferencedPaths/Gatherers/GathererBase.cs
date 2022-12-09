namespace GetAllReferencedPaths.Gatherers;

public abstract class GathererBase
{
	protected IEnumerable<DirectoryInfo> Roots { get; }

	protected GathererBase(IEnumerable<DirectoryInfo> roots)
	{
		Roots = roots;
	}

	public static List<FileInfo> RootFile(
		IEnumerable<DirectoryInfo> roots,
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

	public abstract Task<Result<List<string>>> GetStringsAsync(
		FileInfo source,
		CancellationToken cancellationToken = default);

	public virtual IEnumerable<FileInfo> RootFiles(
		FileInfo source,
		IEnumerable<string> strings)
	{
		var roots = Roots.Append(source.Directory!);
		foreach (var @string in strings)
		{
			// only treat strings with a dot in it as a potential path
			if (!@string.Contains('.'))
			{
				continue;
			}

			foreach (var file in RootFile(roots, @string))
			{
				yield return file;
			}
		}
	}
}