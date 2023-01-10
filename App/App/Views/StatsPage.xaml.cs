using App.Data;
using App.Resx;
using App.ViewModels;
using App.ViewModels.DataViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StatsPage : ContentPage
	{
		private readonly StatisticsHolder _stats = DependencyService.Get<StatisticsHolder>();

		private readonly StatsViewModel _viewModel = new StatsViewModel();

		public StatsPage()
		{
			InitializeComponent();
			LoadGraphs();
			this.BindingContext = _viewModel;
			MainView.ItemsSource = _viewModel.Displays;
			_stats.PropertyChanged += UpdateGraphs;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		private void LoadGraphs()
		{
			_viewModel.Displays.Clear();

			_viewModel.Displays.Add(new StatisticsItemViewModel(
				AppResource.ExpenseType, 
				_stats.Statistics.ExpensesByType));

			_viewModel.Displays.Add(new StatisticsItemViewModel(
				AppResource.Monthly, 
				_stats.Statistics.ExpensesByMonth, isMonthly: true));
			
			_viewModel.Displays.Add(new StatisticsItemViewModel(
				AppResource.YearlyExpense,
				_stats.Statistics.ExpensesByYear));

			_viewModel.Displays.Add(new StatisticsItemViewModel(
				AppResource.YearlyIncome,
				_stats.Statistics.IncomeByYear,
				1.0m));
		}

		private void UpdateGraphs(string name)
		{
			_viewModel.Displays[0].UpdateGraph(_stats.Statistics.ExpensesByType);
			_viewModel.Displays[1].UpdateGraph(_stats.Statistics.ExpensesByMonth, isMonthly: true);
			_viewModel.Displays[2].UpdateGraph(_stats.Statistics.ExpensesByYear);
			_viewModel.Displays[3].UpdateGraph(_stats.Statistics.IncomeByYear, 1.0m);
		}
	}
}