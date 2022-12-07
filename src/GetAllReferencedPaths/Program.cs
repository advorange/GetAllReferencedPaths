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
		var filesToProcess = new Stack<FileInfo>(sources);
		while (filesToProcess.TryPop(out var file))
		{
			if (!alreadyProcessed.Add(Path.GetFullPath(file.FullName)))
			{
				continue;
			}

			foreach (var gatherer in gatherers)
			{
				var strings = await gatherer.GetStringsAsync(
					file,
					cancellationToken
				).ConfigureAwait(false);
				foreach (var rootedFile in gatherer.RootFiles(file, strings))
				{
					filesToProcess.Push(rootedFile);
				}
			}
		}

		return alreadyProcessed
			.OrderBy(x => x)
			.Select(x => new FileInfo(x))
			.ToList();
	}

	public static async Task Main()
	{
		var temp = new Arguments(
			BaseDirectory: Directory.GetCurrentDirectory(),
			InterchangeableFileTypes: new()
			{
				new()
				{
					".jsim", ".jresource"
				},
			},
			OutputDirectory: "../ReferencedFilesOutput",
			RootDirectories: new()
			{
				"./sim"
			},
			SourceFiles: new()
			{
				"./input.jsim"
			}
		);
		var args = RuntimeArguments.Create(temp);

		var files = await args.Gatherers.GetFilesAsync(args.Sources).ConfigureAwait(false);
		var time = DateTime.Now.ToString("s").Replace(':', '.');
		foreach (var source in files)
		{
			var relative = Path.GetRelativePath(args.BaseDirectory.FullName, source.FullName);
			var destination = Path.Combine(args.Output.FullName, time, relative);

			Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
			source.CopyTo(destination);

			Console.WriteLine($"Copied: {relative} -> {destination}");
		}
	}
}