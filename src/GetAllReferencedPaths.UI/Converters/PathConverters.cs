using Avalonia.Data.Converters;

using System.Collections;
using System.IO;

namespace GetAllReferencedPaths.UI.Converters;

public static class PathConverters
{
	public static FuncMultiValueConverter<string, string> Join { get; } = new(input =>
	{
		var output = "";
		foreach (var segment in input)
		{
			if (segment is null)
			{
				continue;
			}

			output = Path.Combine(output, segment);
		}
		return Path.GetFullPath(output);
	});
}