using App.Resx;
using App.ViewModels;
using App.ViewModels.DataViewModels;
using System;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SubscriptionPage : ContentPage
	{
		private readonly SubscriptionViewModel _viewModel = new SubscriptionViewModel();

		public SubscriptionPage()
		{
			InitializeComponent();
			this.BindingContext = _viewModel;
			SubscriptionsListView.ItemsSource = _viewModel.Subscriptions;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			RefreshView_Refreshing(MainRefresh, EventArgs.Empty);
		}

		protected override async void OnDisappearing()
		{
			base.OnDisappearing();
			await Navigation.PopAsync();
		}

		private async void RefreshView_Refreshing(object _, EventArgs e)
		{
			await _viewModel.UpdateLayout();
			_viewModel.IsRefreshing = false;
		}

		private async void SwipeItem_DeleteInvoked(object sender, EventArgs e)
		{
			if (!await DisplayAlert(AppResource.Warning, AppResource.DeleteItemMessage, AppResource.Delete, AppResource.Cancel))
				return;
			var toDelete = (SubscriptionItemViewModel)((SwipeItem)sender).Parent.BindingContext;
			await _viewModel.DeleteSubscription(toDelete);
			_viewModel.IsRefreshing = true;
			RefreshView_Refreshing(null, EventArgs.Empty);
		}

		private async void SwipeItem_EditInvoked(object sender, EventArgs e)
		{
			var sub = ((SubscriptionItemViewModel)((SwipeItem)sender).Parent.BindingContext).Subscription;
			await Navigation.PushAsync(new AddPage(sub));
		}

		private void SwipeItem_InfoClicked(object sender, EventArgs e)
		{
			var currentCulture = CultureInfo.CurrentCulture;
			var displayItem = (SubscriptionItemViewModel)((SwipeItem)sender).Parent.BindingContext;

			var message = new StringBuilder();
			message.AppendLine($"{AppResource.Description}: {displayItem.Subscription.Description,30}");
			message.AppendLine($"{AppResource.Value}: {displayItem.ValueString,30}");
			message.AppendLine($"{AppResource.ExpenseType}: {App.ResourceManager.GetString(displayItem.Subscription.ExpenseType.ToString(), currentCulture),30}");
			message.AppendLine($"{AppResource.RenewalType}: {App.ResourceManager.GetString(displayItem.Subscription.RenewalType.ToString(), currentCulture),30}");
			message.AppendLine($"{AppResource.NextRenewal}: {displayItem.NextRenewalString,30}");
			message.Append($"{AppResource.LastRenewal}: {displayItem.Subscription.LastPaid,30:dd/MM/yyyy}");

			DisplayAlert(AppResource.Subscriptions, message.ToString(), "Ok");
		}
	}
}