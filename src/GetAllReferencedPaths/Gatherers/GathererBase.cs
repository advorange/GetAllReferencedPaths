namespace GetAllReferencedPaths.Gatherers;

public abstract class GathererBase
{
	protected IEnumerable<DirectoryInfo> Roots { get; }

	protected GathererBase(IEnumerable<DirectoryInfo> roots)
	{
		Roots = roots;
	}

	public abstract Task<List<string>> GetStringsAsync(
		FileInfo file,
		CancellationToken cancellationToken = default);

	public virtual IEnumerable<FileInfo> RootFiles(
		FileInfo source,
		IEnumerable<string> strings)
	{
		foreach (var str in strings)
		{
			// only treat strings with a dot in it as a potential path
			if (!str.Contains('.'))
			{
				continue;
			}

			foreach (var root in Roots.Append(source.Directory!))
			{
				FileInfo file;
				try
				{
					var joined = Path.Join(root.FullName, str);
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
	}
}