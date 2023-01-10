using App.Data;
using App.Extensions;
using App.ViewModels.DataViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App.ViewModels
{
    public class SubscriptionViewModel : ObservableObject
	{
		private readonly AppDatabase _database = DependencyService.Get<AppDatabase>();

		private readonly StatisticsHolder _stats = DependencyService.Get<StatisticsHolder>();

		public ObservableCollection<SubscriptionItemViewModel> Subscriptions = new ObservableCollection<SubscriptionItemViewModel>();

		public string ExpenseYTD => _stats.Statistics.SubscriptionPaidYTD.ToCurrencyString();

		public string MonthlyExpense => (_stats.Statistics.YearlySubscriptionExpense / 12.0m).ToCurrencyString();

		public string YearlyExpense => _stats.Statistics.YearlySubscriptionExpense.ToCurrencyString();

		public bool ShowEmptyLabel => Subscriptions.Count == 0;

		private bool _isRefreshing;
		public bool IsRefreshing
		{
			get => _isRefreshing;
			set => SetProperty(ref _isRefreshing, value);
		}

		public async Task UpdateLayout()
		{
			Subscriptions.Clear();
			(await _database.GetSubscriptionsAsync()).ForEach(x => Subscriptions.Add(new SubscriptionItemViewModel(x)));

			OnPropertyChanged(nameof(ExpenseYTD));
			OnPropertyChanged(nameof(MonthlyExpense));
			OnPropertyChanged(nameof(YearlyExpense));
			OnPropertyChanged(nameof(ShowEmptyLabel));
		}

		public Task DeleteSubscription(SubscriptionItemViewModel d) 
			=> Task.WhenAll(
				_database.DeleteSubscriptionAsync(d.Subscription),
				_stats.RemoveSubscription(d.Subscription)
			);
	}
}
