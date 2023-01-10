using App.Data;
using App.ViewModels.DataViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace App.ViewModels
{
    public class BudgetBrowserViewModel : ObservableObject
	{
		private readonly AppDatabase _database = DependencyService.Get<AppDatabase>();
		
		public ObservableCollection<BudgetItemViewModel> Budgets = new ObservableCollection<BudgetItemViewModel>();

		public bool ShowEmptyLabel => Budgets.Count == 0;

		public async Task LoadBudgets()
		{
			Budgets.Clear();

			(await _database.GetBudgetsAsync())
				.OrderBy(b => b.EndingDate)
				.ForEach(x => Budgets.Add(new BudgetItemViewModel(x)));

			OnPropertyChanged(nameof(ShowEmptyLabel));
		}

		public Task DeleteBudget(BudgetItemViewModel b) => _database.DeleteBudgetAsync(b.Budget);
	}
}
