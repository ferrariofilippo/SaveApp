using App.ViewModels.DataViewModels;
using System.Collections.ObjectModel;

namespace App.ViewModels
{
    public class StatsViewModel
	{
		public ObservableCollection<StatisticsItemViewModel> Displays { get; } = new ObservableCollection<StatisticsItemViewModel>();
	}
}
