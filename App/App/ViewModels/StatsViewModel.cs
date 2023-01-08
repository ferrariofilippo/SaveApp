using App.Models;
using System.Collections.ObjectModel;

namespace App.ViewModels
{
	public class StatsViewModel
	{
		public readonly ObservableCollection<StatisticsDisplay> Displays = new ObservableCollection<StatisticsDisplay>();
	}
}
