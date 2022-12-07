using GetAllReferencedPaths.Gatherers;

namespace GetAllReferencedPaths;

public static class Program
{
	public static async Task<List<FileInfo>> GetFilesAsync(
		this IEnumerable<GathererBase> gatherers,
		IEnumerable<FileInfo> sources,
		CancellationToken cancellationToken = default)
	{
		var alreadyProcessed = new HashSet<string>();
		var needsProcessing = new Stack<FileInfo>(sources);
		while (needsProcessing.TryPop(out var source))
		{
			if (!alreadyProcessed.Add(Path.GetFullPath(source.FullName)))
			{
				continue;
			}

			foreach (var gatherer in gatherers)
			{
				var strings = await gatherer.GetStringsAsync(
					source,
					cancellationToken
				).ConfigureAwait(false);
				foreach (var foundFile in gatherer.RootFiles(source, strings))
				{
					needsProcessing.Push(foundFile);
				}
			}
		}

		return alreadyProcessed
			.OrderBy(x => x)
			.Select(x => new FileInfo(x))
			.ToList();
	}

	public static DirectoryInfo GetLongestSharedDirectory(
		this IEnumerable<FileInfo> files)
	{
		var first = files.First().FullName;
		var i = 0;
		while (files.All(x => x.FullName.Length > i && x.FullName[i] == first[i]))
		{
			++i;
		}

		while (!Directory.Exists(first[..i]))
		{
			--i;
		}

		return new(first[..i]);
	}

	public static async Task Main()
	{
		var currentDirectory = Directory.GetCurrentDirectory();
		var root = Path.Combine(currentDirectory, "json");
		var args = new Arguments(
			Files: new FileInfo[]
			{
				new(Path.Combine(root, "input.json")),
			},
			Output: new(Path.Combine(currentDirectory, "Output")),
			Roots: new DirectoryInfo[]
			{
				new(root),
			}
		);
		var gatherers = new GathererBase[]
		{
			new JsonGatherer(args.Roots),
			new RegexGatherer(args.Roots),
		};

		var files = await gatherers.GetFilesAsync(args.Files).ConfigureAwait(false);
		var common = GetLongestSharedDirectory(files);
		var time = DateTime.Now.ToString("s").Replace(':', '.');
		foreach (var source in files)
		{
			var relative = Path.GetRelativePath(common.FullName, source.FullName);
			var destination = Path.Combine(args.Output.FullName, time, relative);

			Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
			source.CopyTo(destination);

			Console.WriteLine($"Copied: {relative} -> {destination}");
		}
	}
}

public record struct Arguments(
	IReadOnlyList<FileInfo> Files,
	DirectoryInfo Output,
	IReadOnlyList<DirectoryInfo> Roots
);