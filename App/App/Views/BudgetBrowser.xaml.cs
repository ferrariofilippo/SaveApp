using App.Extensions;
using App.Resx;
using App.ViewModels;
using App.ViewModels.DataViewModels;
using System;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BudgetBrowser : ContentPage
	{
		private readonly BudgetBrowserViewModel _viewModel = new BudgetBrowserViewModel();

		public BudgetBrowser()
		{
			InitializeComponent();
			this.BindingContext = _viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			BudgetListView.ItemsSource = _viewModel.Budgets;
			Refresh_Budgets(BudgetListView, EventArgs.Empty);
		}

		private async void Refresh_Budgets(object _, EventArgs e)
		{
			await _viewModel.LoadBudgets();
			_viewModel.IsRefreshing = false;
		}

		private async void SwipeItem_DeleteInvoked(object sender, EventArgs e)
		{
			if (!await DisplayAlert(AppResource.Warning, AppResource.DeleteItemMessage, AppResource.Delete, AppResource.Cancel))
				return;
			var toDelete = (BudgetItemViewModel)((SwipeItem)sender).Parent.BindingContext;
			await _viewModel.DeleteBudget(toDelete);

			_viewModel.IsRefreshing = true;
			Refresh_Budgets(null, null);
		}

		private void SwipeItem_InfoInvoked(object sender, EventArgs e)
		{
			var budgetgDisplay = (BudgetItemViewModel)((SwipeItem)sender).Parent.BindingContext;

			var message = new StringBuilder();
			message.AppendLine($"{AppResource.Description}: {budgetgDisplay.Budget.Name,30}");
			message.AppendLine($"{AppResource.Remaining}: {budgetgDisplay.RemainingString,30}");
			message.AppendLine($"{AppResource.Used}: {budgetgDisplay.UsedString,30}");
			message.AppendLine($"{AppResource.Total}: {budgetgDisplay.Budget.MaxAmount.ToCurrencyString(),30}");
			message.AppendLine($"{AppResource.InitialDate}: {budgetgDisplay.InitialString,30}");
			message.Append($"{AppResource.FinalDate}: {budgetgDisplay.EndingString,30}");

			DisplayAlert(AppResource.Budget, message.ToString(), "Ok");
		}

		private async void Add_Clicked(object sedenr, EventArgs e) 
			=> await Navigation.PushAsync(new AddBudgetPage());
	}
}