namespace GetAllReferencedPaths.Gatherers;

public abstract class GathererBase
{
	protected IEnumerable<DirectoryInfo> Roots { get; }

	protected GathererBase(IEnumerable<DirectoryInfo> roots)
	{
		Roots = roots;
	}

	public static IEnumerable<FileInfo> RootFile(
		IEnumerable<DirectoryInfo> roots,
		string @string)
	{
		foreach (var root in roots)
		{
			FileInfo file;
			try
			{
				var joined = Path.Join(root.FullName, @string);
				if (!File.Exists(joined))
				{
					continue;
				}

				file = new FileInfo(joined);
			}
			catch
			{
				continue;
			}

			yield return file;
		}
	}

	public abstract Task<List<string>> GetStringsAsync(
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