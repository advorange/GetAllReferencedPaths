using DynamicData;
using DynamicData.Binding;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace GetAllReferencedPaths.UI.ViewModels.Arguments;

public sealed class FileTypeGroupViewModel : ObservableCollection<StringWrapper>
{
	public string Display { get; private set; } = "";

	public FileTypeGroupViewModel()
	{
		this.ToObservableChangeSet()
			.AutoRefresh(x => x.Value)
			.ToCollection()
			.Select(extensions =>
			{
				var values = extensions
					.Select(x => x.Value)
					.Where(x => !string.IsNullOrWhiteSpace(x));
				return string.Join(", ", values);
			})
			.Subscribe(x =>
			{
				Display = x;
				OnPropertyChanged(new(nameof(Display)));
			});
	}
}