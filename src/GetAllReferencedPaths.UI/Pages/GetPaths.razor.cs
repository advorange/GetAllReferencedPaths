using System.Text.Json;

namespace GetAllReferencedPaths.UI.Pages;

public sealed partial class GetPaths
{
	private readonly JsonSerializerOptions _Options = new()
	{
		WriteIndented = true,
	};

	public Arguments? Args { get; set; }

	protected override async Task OnInitializedAsync()
	{
		var dir = Directory.GetCurrentDirectory();
		Args = await LoadArgumentsAsync().ConfigureAwait(false);
	}

	private Task<Arguments> LoadArgumentsAsync()
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
		return Task.FromResult(temp);
	}
}