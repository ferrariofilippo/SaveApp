using App.ViewModels.DataViewModels;
using System.Collections.ObjectModel;

namespace App.ViewModels
{
    public class StatsViewModel
	{
		public readonly ObservableCollection<StatisticsItemViewModel> Displays = new ObservableCollection<StatisticsItemViewModel>();
	}
}
