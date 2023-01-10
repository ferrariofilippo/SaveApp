using App.Data;
using App.Helpers;
using App.Helpers.Notifications;
using App.Helpers.Themes;
using App.Models.Enums;
using App.Resx;
using App.ViewModels;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace App.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		private readonly IFileSaver _saver = DependencyService.Get<IFileSaver>();

		private readonly SettingsManager _settings = DependencyService.Get<SettingsManager>();

		private readonly SettingsViewModel _viewModel = new SettingsViewModel();

		private readonly object _lock = new object();

		private bool _isSaving = false;

		public SettingsPage()
		{
			InitializeComponent();
			this.BindingContext = _viewModel;
			InitializePickers();
		}

		private void InitializePickers()
		{
			_viewModel.Themes.ForEach(x => ThemePicker.Items.Add(x));
			_viewModel.Currencies.ForEach(x => CurrencyPicker.Items.Add(x));
			ThemePicker.SelectedIndex = _settings.Settings.Theme;
			CurrencyPicker.SelectedIndex = _settings.Settings.BaseCurrency;
			CurrencyLabel.Text = _viewModel.Currencies[CurrencyPicker.SelectedIndex];
		}

		private async void ManageSubs_Clicked(object sender, EventArgs e)
			=> await Navigation.PushAsync(new SubscriptionPage());

		private void DownloadData_Clicked(object sender, EventArgs e)
		{
			lock (_lock)
			{
				if (!_isSaving)
					_ = SaveData();
			}
		}

		private async Task SaveData()
		{
			try
			{
				lock (_lock)
					_isSaving = true;

				var database = DependencyService.Get<AppDatabase>();

				await Task.WhenAll(
					Task.Run(async () =>
					{
						var builder = new StringBuilder("ID,VALUE,IS_EXPENSE,DESCRIPTION,TYPE,DATE");
						var movements = await database.GetMovementsAsync();
						var path = $"Expenses-{DateTime.Now:dd-MM-yyyy}.csv";

						if (!(movements is null))
						{
							foreach (var item in movements)
							{
								builder.AppendLine(
									$"{item.Id},{item.Value},{item.IsExpense},{item.Description}," +
									$"{(item.IsExpense ? item.ExpenseType.ToString() : "null")}," +
									$"{item.CreationDate.Date}");
							}
						}

						await _saver.SaveFile(path, builder.ToString());
					}),
					Task.Run(async () =>
					{
						var builder = new StringBuilder(
							"ID,VALUE,DESCRIPTION,EXPENSE_TYPE,RENEWAL_TYPE,LAST_PAID,NEXT_RENEWAL,CREATION_DATE");
						var subs = await database.GetSubscriptionsAsync();
						var path = $"Subscriptions-{DateTime.Now:dd-MM-yyyy}.csv";

						if (!(subs is null))
						{
							foreach (var item in subs)
							{
								builder.AppendLine(
									$"{item.Id},{item.Value},{item.Description},{item.ExpenseType}" +
									$"{item.RenewalType},{item.LastPaid.Date},{item.NextRenewal.Date}" +
									$"{item.CreationDate.Date}");
							}
						}

						await _saver.SaveFile(path, builder.ToString());
					})
				);

				NotificationHelper.SendNotification(
					AppResource.FileDownloadedNotificationTItle,
					AppResource.FileDownloadNotificationDescription);
			}
			catch (Exception ex)
			{
				NotificationHelper.NotifyException(ex);
			}

			lock (_lock)
				_isSaving = false;
		}

		private void ThemePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			var theme = ThemePicker.SelectedIndex;
			if (theme != _settings.Settings.Theme)
				ThemeManager.ChangeTheme((Theme)theme);

			ThemeLabel.Text = ThemePicker.SelectedItem.ToString();
			ThemeLabel.Unfocus();
		}

		private async void CurrencyPicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrencyLabel.Unfocus();
			if (CurrencyPicker.SelectedIndex == _settings.Settings.BaseCurrency)
				return;

			if (!await DisplayAlert(AppResource.Warning, AppResource.UpdatingCurrencyMessage, "Ok", AppResource.Cancel))
			{
				CurrencyPicker.SelectedIndex = _settings.Settings.BaseCurrency;
				return;
			}

			CurrencyLabel.Text = CurrencyPicker.SelectedItem.ToString();

			var previous = (Currencies)_settings.Settings.BaseCurrency;
			var currencies = DependencyService.Get<CurrenciesManager>();
			_settings.Settings.BaseCurrency = (byte)CurrencyPicker.SelectedIndex;

			await _settings.SaveSettings();
			_ = currencies.UpdateAllToCurrent(previous);
		}

		private async void Credits_Clicked(object sender, EventArgs e)
			=> await Launcher.OpenAsync(Constants.CreditsUrl);

		private async void ReportBug_Clicked(object sender, EventArgs e)
			=> await Launcher.OpenAsync(Constants.ReportBugUrl);

		private void ThemePicker_Unfocused(object sender, FocusEventArgs e)
			=> ThemeLabel.Unfocus();
		private void ThemeLabel_Focused(object sender, FocusEventArgs e)
			=> ThemePicker.Focus();

		private void CurrencyLabel_Focused(object sender, FocusEventArgs e)
			=> CurrencyPicker.Focus();
		private void CurrencyPicker_Unfocused(object sender, FocusEventArgs e)
			=> CurrencyLabel.Unfocus();
	}
}