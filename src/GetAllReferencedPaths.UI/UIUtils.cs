using Avalonia.Controls;
using Avalonia.Platform.Storage.FileIO;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetAllReferencedPaths.UI;

public static class UIUtils
{
	public static Task<string?> GetDirectoryAsync(
		this Window window,
		string? directory)
	{
		directory = Directory.Exists(directory) ? directory! : Environment.CurrentDirectory;
		return window.GetDirectoryAsync(directory, "Directory");
	}

	public static async Task<string?> GetDirectoryAsync(
		this Window window,
		string directory,
		string title)
	{
		var result = await window.StorageProvider.OpenFolderPickerAsync(new()
		{
			AllowMultiple = false,
			SuggestedStartLocation = new BclStorageFolder(directory),
			Title = title,
		}).ConfigureAwait(true);

		return result.SingleOrDefault()?.TryGetUri(out var uri) ?? false
			? uri.LocalPath
			: null;
	}
}