using Avalonia.Data.Converters;
using Avalonia.Media;

namespace GetAllReferencedPaths.UI.Converters;

public static class BrushConverters
{
	public static FuncValueConverter<bool, IBrush> FileExists { get; } = new(input =>
	{
		return input ? Brushes.DarkGreen : Brushes.DarkRed;
	});
}