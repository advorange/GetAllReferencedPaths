using GetAllReferencedPaths.Gatherers;

using System.Collections.Immutable;

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

public record Arguments(
	string BaseDirectory,
	List<HashSet<string>> InterchangeableFileTypes,
	string OutputDirectory,
	List<string> RootDirectories,
	List<string> SourceFiles
);

public record RuntimeArguments(
	DirectoryInfo BaseDirectory,
	IReadOnlyList<GathererBase> Gatherers,
	DirectoryInfo Output,
	IReadOnlyList<DirectoryInfo> Roots,
	IReadOnlyList<FileInfo> Sources
)
{
	public static RuntimeArguments Create(Arguments args)
	{
		var roots = args.RootDirectories
			.Select(x => Path.Combine(args.BaseDirectory, x))
			.Prepend(args.BaseDirectory)
			.Select(x => new DirectoryInfo(x))
			.ToImmutableArray();
		var sources = args.SourceFiles
			.SelectMany(x =>
			{
				var sources = new HashSet<string>()
				{
					x
				};

				var fileExtension = Path.GetExtension(x);
				foreach (var extensions in args.InterchangeableFileTypes)
				{
					if (extensions.Contains(fileExtension))
					{
						foreach (var extension in extensions)
						{
							sources.Add(Path.ChangeExtension(x, extension));
						}
					}
				}

				return sources;
			})
			.SelectMany(x => GathererBase.RootFile(roots, x))
			.ToImmutableArray();
		var baseDirectory = new DirectoryInfo(
			args.BaseDirectory
		);
		var output = new DirectoryInfo(
			Path.Combine(args.BaseDirectory, args.OutputDirectory)
		);
		var gatherers = new GathererBase[]
		{
			new JsonGatherer(roots),
			new RegexGatherer(roots),
		}.ToImmutableArray();

		return new(
			BaseDirectory: baseDirectory,
			Gatherers: gatherers,
			Output: output,
			Roots: roots,
			Sources: sources
		);
	}
}