using Avalonia.Controls;
using Avalonia.Platform.Storage.FileIO;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GetAllReferencedPaths.UI;

public static class UIUtils
{
	public static async Task CopyFileAsync(string sourceFile, string destinationFile)
	{
		const int BUFFER_SIZE = 81920;
		const FileOptions FILE_OPTIONS = 0
			| FileOptions.Asynchronous
			| FileOptions.SequentialScan;

		using var sourceStream = new FileStream(
			sourceFile,
			FileMode.Open,
			FileAccess.Read,
			FileShare.Read,
			BUFFER_SIZE,
			FILE_OPTIONS
		);
		using var destinationStream = new FileStream(
			destinationFile,
			FileMode.CreateNew,
			FileAccess.Write,
			FileShare.None,
			BUFFER_SIZE,
			FILE_OPTIONS
		);

		await sourceStream.CopyToAsync(destinationStream);
	}

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