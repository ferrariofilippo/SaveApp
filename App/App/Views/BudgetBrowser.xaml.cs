using App.Extensions;
using App.Models;
using App.Resx;
using App.ViewModels;
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

		private async void Refresh_Budgets(object sender, EventArgs e)
		{
			await _viewModel.LoadBudgets();
			BudgetListView.IsRefreshing = false;
		}

		private async void SwipeItem_DeleteInvoked(object sender, EventArgs e)
		{
			if (!await DisplayAlert(AppResource.Warning, AppResource.DeleteItemMessage, AppResource.Delete, AppResource.Cancel))
				return;
			var toDelete = (BudgetDisplay)((SwipeItem)sender).Parent.BindingContext;
			await _viewModel.DeleteBudget(toDelete);
			await _viewModel.LoadBudgets();
		}

		private void SwipeItem_InfoInvoked(object sender, EventArgs e)
		{
			var bgDisplay = (BudgetDisplay)((SwipeItem)sender).Parent.BindingContext;

			StringBuilder message = new StringBuilder();
			message.AppendLine($"{AppResource.Description}: {bgDisplay.Budget.Name,30}");
			message.AppendLine($"{AppResource.Remaining}: {bgDisplay.RemainingString,30}");
			message.AppendLine($"{AppResource.Used}: {bgDisplay.UsedString,30}");
			message.AppendLine($"{AppResource.Total}: {bgDisplay.Budget.MaxAmount.ToCurrencyString(),30}");
			message.AppendLine($"{AppResource.InitialDate}: {bgDisplay.InitialString,30}");
			message.Append($"{AppResource.FinalDate}: {bgDisplay.EndingString,30}");

			DisplayAlert(AppResource.Budget, message.ToString(), "Ok");
		}

		private async void Add_Clicked(object sedenr, EventArgs e) 
			=> await Navigation.PushAsync(new AddBudgetPage());
	}
}