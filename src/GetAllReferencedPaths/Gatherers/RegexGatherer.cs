using System.Text.RegularExpressions;

namespace GetAllReferencedPaths.Gatherers;

public sealed class RegexGatherer : GathererBase
{
	private static readonly Regex _StringRegex = new(
		"([\"'])(?:\\\\.|[^\\\\])*?\\1",
		RegexOptions.Compiled
	);

	public RegexGatherer(IEnumerable<DirectoryInfo> roots) : base(roots)
	{
	}

	public override Task<Result<List<string>>> GetStringsAsync(
		FileInfo source,
		CancellationToken cancellationToken = default)
		=> Task.FromResult(GetStrings(source));

	private static Result<List<string>> GetStrings(FileInfo source)
	{
		try
		{
			var strings = new List<string>();
			foreach (var line in File.ReadLines(source.FullName))
			{
				var matches = _StringRegex.Matches(line);
				for (var i = 0; i < matches.Count; ++i)
				{
					// quotes are included in the regex match, skip them
					strings.Add(matches[i].Value[1..^1]);
				}
			}
			return new(strings);
		}
		catch
		{
			return new("Unable to read lines.");
		}
	}
}