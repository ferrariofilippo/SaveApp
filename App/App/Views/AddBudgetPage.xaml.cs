using App.Data;
using App.Helpers.UIHelpers;
using App.Models;
using App.Models.Enums;
using App.Resx;
using App.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddBudgetPage : ContentPage
	{
		private readonly IAppDatabase _database = DependencyService.Get<IAppDatabase>();

		private readonly AddBudgetViewModel _viewModel = new AddBudgetViewModel();

		public AddBudgetPage()
		{
			InitializeComponent();
			InitializeUI();
			this.BindingContext = _viewModel;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			Navigation.PopToRootAsync();
		}

		private void InitializeUI()
		{
			var settings = DependencyService.Get<SettingsManager>();

			_viewModel.MovementTypes.ForEach(x => ExpenseTypePicker.Items.Add(x));
			_viewModel.Currencies.ForEach(x => CurrencyPicker.Items.Add(x));

			CurrencyPicker.SelectedIndex = settings.Settings.BaseCurrency;

            InitialDateLabel.Text = DateTime.Today.ToString("dd/MM/yyyy");
            FinalDateLabel.Text = DateTime.Today.ToString("dd/MM/yyyy");
        }

		private async void Save_Clicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(ValueEntry.Text) ||
				!decimal.TryParse(ValueEntry.Text, out decimal value) ||
				value <= 0.0m)
			{
				await DisplayAlert(AppResource.Warning, AppResource.ValueEnteredNotValid, "Ok");
				return;
			}
			if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
			{
				await DisplayAlert(AppResource.Warning, AppResource.DescriptionEnteredNotValid, "Ok");
				return;
			}
			if (ExpenseTypePicker.SelectedIndex == -1)
			{
				await DisplayAlert(AppResource.Warning, AppResource.TypeSelectedNotValid, "Ok");
				return;
			}

			var currenciesManager = DependencyService.Get<CurrenciesManager>();
			value = currenciesManager.ConvertCurrencyToDefault(
				value, 
				(Currencies)CurrencyPicker.SelectedIndex);

			var toSave = new Budget()
			{
				CreationDate = InitialDatePicker.Date,
				EndingDate = FinalDatePicker.Date,
				MaxAmount = value,
				MovementType = (ExpenseType)ExpenseTypePicker.SelectedIndex,
				Name = DescriptionEntry.Text,
				Remaining = value,
				Used = 0.0m
			};

			await _database.SaveBudgetAsync(toSave);
			await Navigation.PopAsync();
		}

		private async void Cancel_Clicked(object _, EventArgs e)
			=> await Navigation.PopAsync();

		private void InitialDateLabel_Focused(object _, EventArgs e)
			=> InitialDatePicker.Focus();

		private void FinalDateLabel_Focused(object _, EventArgs e)
			=> FinalDatePicker.Focus();

		private void InitialDatePicker_DateSelected(object _, DateChangedEventArgs e)
		{
			InitialDateLabel.Text = InitialDatePicker.Date.ToString("dd/MM/yyyy");
			InitialDateLabel.Unfocus();
		}

		private void FinalDatePicker_DateSelected(object _, DateChangedEventArgs e)
		{
			FinalDateLabel.Text = FinalDatePicker.Date.ToString("dd/MM/yyyy");
			FinalDateLabel.Unfocus();
		}

		private void InitialDatePicker_Unfocused(object _, FocusEventArgs e)
			=> InitialDateLabel.Unfocus();

		private void FinalDatePicker_Unfocused(object _, FocusEventArgs e)
			=> InitialDateLabel.Unfocus();

		private void ExpenseTypeLabel_Focused(object _, FocusEventArgs e)
			=> ExpenseTypePicker.Focus();
		private void ExpenseTypePicker_Unfocused(object _, FocusEventArgs e)
			=> ExpenseTypeLabel.Unfocus();

		private void ExpenseTypePicker_SelectedIndexChanged(object _, EventArgs e)
		{
			ExpenseTypeLabel.Text = ExpenseTypePicker.SelectedItem.ToString();
			ExpenseTypeLabel.Unfocus();
		}

		private void ValueEntry_TextChanged(object _, TextChangedEventArgs e)
			=> EntryHelpers.AdjustCurrencyEntryText(ValueEntry, e);
	}
}