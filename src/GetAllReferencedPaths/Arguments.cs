using GetAllReferencedPaths.Gatherers;

using System.Collections.Immutable;

namespace GetAllReferencedPaths;

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
			.SelectMany(x => args.InterchangeableFileTypes.InterchangeFileTypes(x))
			.SelectMany(x => roots.RootFile(x))
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