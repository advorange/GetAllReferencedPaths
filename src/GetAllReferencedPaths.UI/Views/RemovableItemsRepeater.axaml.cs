using Avalonia.Controls;

using System.Collections;

namespace GetAllReferencedPaths.UI.Views;

public partial class RemovableItemsRepeater : ItemsRepeater
{
	public RemovableItemsRepeater()
	{
		InitializeComponent();
	}

	protected void RemoveItem(object arg)
		=> (Items as IList)?.Remove(arg);
}