namespace GetAllReferencedPaths;

public static class Program
{
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

		var alreadyProcessed = new HashSet<string>();
		var filesToProcess = new Stack<FileInfo>(args.Sources);
		while (filesToProcess.TryPop(out var file))
		{
			if (!alreadyProcessed.Add(Path.GetFullPath(file.FullName)))
			{
				continue;
			}

			foreach (var gatherer in args.Gatherers)
			{
				var result = await gatherer.GetStringsAsync(file).ConfigureAwait(false);
				if (!result.IsSuccess)
				{
					continue;
				}

				foreach (var rootedFile in gatherer.RootFiles(file, result.Value))
				{
					filesToProcess.Push(rootedFile);
				}
			}
		}

		var files = alreadyProcessed
			.OrderBy(x => x)
			.Select(x => new FileInfo(x))
			.ToList();

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