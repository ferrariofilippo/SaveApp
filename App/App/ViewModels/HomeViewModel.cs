using App.Data;
using App.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Xamarin.Forms;

namespace App.ViewModels
{
	public class HomeViewModel : ObservableObject
	{
		public readonly StatisticsManager Stats = DependencyService.Get<StatisticsManager>();

		private decimal _income = 0.0m;
		public decimal Income
		{
			get => _income;
			set
			{
				if (SetProperty(ref _income, value))
				{
					OnPropertyChanged(nameof(NetWorth));
					OnPropertyChanged(nameof(NetWorthColor));
					OnPropertyChanged(nameof(NetWorthString));
					OnPropertyChanged(nameof(IncomeString));
				}
			}
		}

		private decimal _expenses = 0.0m;
		public decimal Expenses
		{
			get => _expenses;
			set
			{
				if (SetProperty(ref _expenses, value))
				{
					OnPropertyChanged(nameof(NetWorth));
					OnPropertyChanged(nameof(NetWorthColor));
					OnPropertyChanged(nameof(NetWorthString));
					OnPropertyChanged(nameof(ExpensesString));
				}
			}
		}

		private bool _isRefreshing;
		public bool IsRefreshing
		{
			get => _isRefreshing;
			set => SetProperty(ref _isRefreshing, value);
		}

		public Color Green => (Color)App.Current.Resources["IncomeColor"];

		public Color Red => (Color)App.Current.Resources["ExpenseColor"];

		public decimal NetWorth => _income - _expenses;

		public Color NetWorthColor => NetWorth > 0 ? Green : Red;

		public string NetWorthString => NetWorth.ToCurrencyString();

		public string IncomeString => Income.ToCurrencyString();

		public string ExpensesString => (-Expenses).ToCurrencyString();

		public HomeViewModel()
		{
			UpdateData();
		}

		public void UpdateData()
		{
			Income = Stats.Statistics.TotalIncome;
			Expenses = Stats.Statistics.TotalExpenses;
		}
	}
}
