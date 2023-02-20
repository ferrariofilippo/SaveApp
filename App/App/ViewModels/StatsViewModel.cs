using App.ViewModels.DataViewModels;
using System.Collections.ObjectModel;

namespace App.ViewModels
{
    public sealed class StatsViewModel
	{
		public ObservableCollection<StatisticsItemViewModel> Displays { get; } = new ObservableCollection<StatisticsItemViewModel>();
	}
}
