using System.Text.Json;

namespace GetAllReferencedPaths;

public static class Program
{
	public static IEnumerable<FileInfo> GetFiles(
		IEnumerable<DirectoryInfo> roots,
		IEnumerable<string> strings)
	{
		foreach (var str in strings)
		{
			// only treat strings with a dot in it as a potential path
			if (!str.Contains('.'))
			{
				continue;
			}

			foreach (var root in roots)
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

	public static async Task<IEnumerable<FileInfo>> GetFilesAsync(
		IEnumerable<DirectoryInfo> roots,
		FileInfo file,
		CancellationToken cancellationToken = default)
	{
		var strings = await GetStringsAsync(file, cancellationToken).ConfigureAwait(false);
		return GetFiles(roots.Append(file.Directory!), strings);
	}

	public static DirectoryInfo GetLongestSharedDirectory(IEnumerable<FileInfo> files)
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

	public static IEnumerable<string> GetStrings(JsonElement element)
	{
		switch (element.ValueKind)
		{
			case JsonValueKind.Object:
				foreach (var property in element.EnumerateObject())
				{
					foreach (var e in GetStrings(property.Value))
					{
						yield return e;
					}
				}
				break;

			case JsonValueKind.Array:
				foreach (var item in element.EnumerateArray())
				{
					foreach (var e in GetStrings(item))
					{
						yield return e;
					}
				}
				break;

			case JsonValueKind.String:
				yield return element.GetString()!;
				break;

			case JsonValueKind.Undefined:
			case JsonValueKind.Number:
			case JsonValueKind.True:
			case JsonValueKind.False:
			case JsonValueKind.Null:
				break;
		}
	}

	public static async Task<IEnumerable<string>> GetStringsAsync(
		FileInfo file,
		CancellationToken cancellationToken = default)
	{
		try
		{
			using var fs = file.OpenRead();
			using var doc = await JsonDocument.ParseAsync(
				utf8Json: fs,
				cancellationToken: cancellationToken
			);

			return GetStrings(doc.RootElement).ToList();
		}
		catch
		{
			return Array.Empty<string>();
		}
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

		var alreadyProcessed = new HashSet<string>();
		var needsProcessing = new Stack<FileInfo>(args.Files);
		while (needsProcessing.TryPop(out var file))
		{
			if (!alreadyProcessed.Add(Path.GetFullPath(file.FullName)))
			{
				continue;
			}

			var foundFiles = await GetFilesAsync(args.Roots, file).ConfigureAwait(false);
			foreach (var foundFile in foundFiles)
			{
				needsProcessing.Push(foundFile);
			}
		}

		var files = alreadyProcessed
			.OrderBy(x => x)
			.Select(x => new FileInfo(x))
			.ToList();
		var directory = GetLongestSharedDirectory(files);
		foreach (var file in files)
		{
			var relative = Path.GetRelativePath(directory.FullName, file.FullName);
			var destination = Path.Combine(args.Output.FullName, relative);

			var dir = Path.GetDirectoryName(destination)!;
			Directory.CreateDirectory(dir);
			file.CopyTo(destFileName: destination, overwrite: true);
			Console.WriteLine($"Copied: {relative} -> {destination}");
		}
	}
}

public record Arguments(
	IReadOnlyList<FileInfo> Files,
	DirectoryInfo Output,
	IReadOnlyList<DirectoryInfo> Roots
);