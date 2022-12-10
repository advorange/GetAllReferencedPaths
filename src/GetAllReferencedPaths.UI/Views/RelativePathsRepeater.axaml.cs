using Avalonia.Controls;

using System.Collections;

namespace GetAllReferencedPaths.UI.Views;

public partial class RelativePathsRepeater : ItemsRepeater
{
	public RelativePathsRepeater()
	{
		InitializeComponent();
	}

	protected void RemoveItem(object arg)
		=> (Items as IList)?.Remove(arg);
}