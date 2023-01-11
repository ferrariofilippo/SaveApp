using App.Data;
using App.Helpers;
using App.Helpers.Themes;
using App.Models.Enums;
using App.Resx;
using App.ViewModels;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SettingsPage : ContentPage
	{
		private readonly ISettingsManager _settings = DependencyService.Get<ISettingsManager>();

		private readonly SettingsViewModel _viewModel = new SettingsViewModel();

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

		private void ToggleMovementSection_Clicked(object sender, EventArgs e)
			=> _viewModel.IsDataSectionVisible = !_viewModel.IsDataSectionVisible;

		private void DownloadData_Clicked(object sender, EventArgs e)
			=> _ = DataImportExportHelper.ExportData();

		private void DownloadTemplate_Clicked(object sender, EventArgs e)
			=> _ = DataImportExportHelper.GetTemplateFile();

		private async void ImportMovements_Clicked(object sender, EventArgs e)
			=> await DataImportExportHelper.ImportMovements();

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
			var currencies = DependencyService.Get<ICurrenciesManager>();
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