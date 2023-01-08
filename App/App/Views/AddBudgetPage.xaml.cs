using App.Data;
using App.Models;
using App.Models.Enums;
using App.Resx;
using App.ViewModels;
using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddBudgetPage : ContentPage
	{
		private readonly AppDatabase _database = DependencyService.Get<AppDatabase>();

		private readonly AddBudgetViewModel _viewModel = new AddBudgetViewModel();

		public AddBudgetPage()
		{
			InitializeComponent();
			InitializePickers();
			this.BindingContext = _viewModel;

			InitialDateLabel.Text = DateTime.Today.ToString("dd/MM/yyyy");
			FinalDateLabel.Text = DateTime.Today.ToString("dd/MM/yyyy");
		}

		private void InitializePickers()
		{
			var settings = DependencyService.Get<SettingsManager>();

			_viewModel.MovementTypes.ForEach(x => ExpenseTypePicker.Items.Add(x));
			_viewModel.Currencies.ForEach(x => CurrencyPicker.Items.Add(x));

			CurrencyPicker.SelectedIndex = settings.Settings.BaseCurrency;
		}

		private async void Save_Clicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(ValueEntry.Text) ||
				!decimal.TryParse(ValueEntry.Text, out decimal value))
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

		private async void Cancel_Clicked(object sender, EventArgs e)
			=> await Navigation.PopAsync();

		private void InitialDateLabel_Focused(object sender, EventArgs e)
			=> InitialDatePicker.Focus();

		private void FinalDateLabel_Focused(object sender, EventArgs e)
			=> FinalDatePicker.Focus();

		private void InitialDatePicker_DateSelected(object sender, DateChangedEventArgs e)
		{
			InitialDateLabel.Text = InitialDatePicker.Date.ToString("dd/MM/yyyy");
			InitialDateLabel.Unfocus();
		}

		private void FinalDatePicker_DateSelected(object sender, DateChangedEventArgs e)
		{
			FinalDateLabel.Text = FinalDatePicker.Date.ToString("dd/MM/yyyy");
			FinalDateLabel.Unfocus();
		}

		private void InitialDatePicker_Unfocused(object sender, FocusEventArgs e)
			=> InitialDateLabel.Unfocus();

		private void FinalDatePicker_Unfocused(object sender, FocusEventArgs e)
			=> InitialDateLabel.Unfocus();

		private void ExpenseTypeLabel_Focused(object sender, FocusEventArgs e)
			=> ExpenseTypePicker.Focus();
		private void ExpenseTypePicker_Unfocused(object sender, FocusEventArgs e)
			=> ExpenseTypeLabel.Unfocus();

		private void ExpenseTypePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			ExpenseTypeLabel.Text = ExpenseTypePicker.SelectedItem.ToString();
			ExpenseTypeLabel.Unfocus();
		}

		private void ValueEntry_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (e.NewTextValue.Length > 0)
			{
				if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," &&
					e.NewTextValue.Last() == '.')
				{
					ValueEntry.Text = e.OldTextValue + ',';
				}
				else if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "." &&
					e.NewTextValue.Last() == ',')
				{
					ValueEntry.Text = e.OldTextValue + '.';
				}
			}
		}
	}
}