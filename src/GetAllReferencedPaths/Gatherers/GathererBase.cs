namespace GetAllReferencedPaths.Gatherers;

public abstract class GathererBase
{
	protected IEnumerable<DirectoryInfo> Roots { get; }

	protected GathererBase(IEnumerable<DirectoryInfo> roots)
	{
		Roots = roots;
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

			foreach (var file in roots.RootFile(@string))
			{
				yield return file;
			}
		}
	}
}