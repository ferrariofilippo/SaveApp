using App.Data;
using App.Helpers;
using App.Models;
using App.Models.Enums;
using App.Resx;
using App.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddPage : ContentPage
	{
		private readonly AppDatabase _database = DependencyService.Get<AppDatabase>();

		private readonly StatisticsHolder _stats = DependencyService.Get<StatisticsHolder>();

		private readonly AddMovementViewModel _viewModel = new AddMovementViewModel();

		private readonly Dictionary<int, int> IndexToBudgetId = new Dictionary<int, int>();

		private readonly Movement _toEdit;

		private readonly Subscription _subToEdit;

		private readonly bool _isEditingMovement;

		private readonly bool _isEditingSubscription;

		public AddPage()
		{
			Init();

			_isEditingMovement = false;
			_isEditingSubscription = false;
			MovementDateLabel.Text = DateTime.Today.ToString("dd/MM/yyyy");
		}

		public AddPage(Movement m)
		{
			Init();

			_isEditingMovement = true;
			_isEditingSubscription = false;
			_toEdit = m;

			ValueEntry.Text = m.Value.ToString();
			DescriptionEntry.Text = m.Description;
			MovementDatePicker.Date = m.CreationDate;
			BudgetPicker.SelectedIndex = IndexFromId(m.BudgetId);
			ExpenseTypePicker.SelectedIndex = (int)m.ExpenseType;
			SubscriptionSwitch.IsVisible = false;
			_viewModel.IsExpense = m.IsExpense;
		}

		public AddPage(Subscription s)
		{
			Init();

			_isEditingMovement = false;
			_isEditingSubscription = true;
			_subToEdit = s;

			ValueEntry.Text = s.Value.ToString();
			DescriptionEntry.Text = s.Description;
			BudgetPicker.SelectedIndex = IndexFromId(s.BudgetId);
			ExpenseTypePicker.SelectedIndex = (int)s.ExpenseType;
			MovementDatePicker.IsVisible = false;
			SubscriptionSwitch.IsVisible = false;
			_viewModel.IsExpense = true;
			_viewModel.IsSubscription = true;
		}

		private void Init()
		{
			InitializeComponent();
			this.BindingContext = _viewModel;
			InitializaPickers();
		}

		private async void InitializaPickers()
		{
			var settings = DependencyService.Get<SettingsManager>();

			_viewModel.MovementTypes.ForEach(x => ExpenseTypePicker.Items.Add(x));
			_viewModel.RenewalTypes.ForEach(x => RenewalPicker.Items.Add(x));
			_viewModel.Currencies.ForEach(x => CurrencyPicker.Items.Add(x));

			var budgets = await _database.GetBudgetsAsync();
			IndexToBudgetId.Clear();
			IndexToBudgetId.Add(0, 0);
			BudgetPicker.Items.Add(AppResource.None);

			for (int i = 0; i < budgets.Count; i++)
			{
				IndexToBudgetId.Add(i + 1, budgets[i].Id);
				BudgetPicker.Items.Add(budgets[i].Name);
			}

			BudgetPicker.SelectedIndex = 0;
			CurrencyPicker.SelectedIndex = settings.Settings.BaseCurrency;
		}

		private int IndexFromId(int id)
		{
			foreach (var item in IndexToBudgetId.Keys)
			{
				if (IndexToBudgetId[item] == id)
					return item;
			}
			return 0;
		}

		private async void Cancel_Clicked(object sender, EventArgs e)
			=> await Navigation.PopAsync();

		private async void BudgetPicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			_viewModel.ShowExpenseTypePicker = BudgetPicker.SelectedIndex == 0;

			BudgetLabel.Text = BudgetPicker.SelectedItem.ToString();
			BudgetLabel.Unfocus();

			if (BudgetPicker.SelectedIndex == 0)
				return;
			var budget = await _database.GetBudgetAsync(IndexToBudgetId[BudgetPicker.SelectedIndex]);
			if (budget is null)
				return;
			ExpenseTypePicker.SelectedIndex = (int)budget.MovementType;
		}

		private void MovementDateLabel_Focused(object sender, FocusEventArgs e)
			=> MovementDatePicker.Focus();

		private void MovementDatePicker_DateSelected(object sender, DateChangedEventArgs e)
		{
			MovementDateLabel.Text = MovementDatePicker.Date.ToString("dd/MM/yyyy");
			MovementDateLabel.Unfocus();
		}

		private void DatePicker_Unfocused(object sender, FocusEventArgs e)
			=> MovementDateLabel.Unfocus();

		private async void Save_Clicked(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
			{
				await DisplayAlert(AppResource.Warning, AppResource.DescriptionEnteredNotValid, "Ok");
				return;
			}
			if (ExpenseTypePicker.SelectedIndex == -1 && _viewModel.IsExpense)
			{
				await DisplayAlert(AppResource.Warning, AppResource.TypeSelectedNotValid, "Ok");
				return;
			}

			if (_viewModel.IsSubscription)
			{
				if (RenewalPicker.SelectedIndex == -1)
				{
					await DisplayAlert(AppResource.Warning, AppResource.RenewalSelectedNotValid, "Ok");
					return;
				}

				var sub = CreateSubscriptionFromInput();
				if (sub is null)
				{
					await DisplayAlert(AppResource.Warning, AppResource.ValueEnteredNotValid, "Ok");
					return;
				}

				if (_isEditingSubscription)
				{
					sub.Id = _subToEdit.Id;
					sub.CreationDate = _subToEdit.CreationDate;
					sub.NextRenewal = _subToEdit.NextRenewal;
					sub.LastPaid = _subToEdit.LastPaid;
					await _stats.RemoveSubscription(_subToEdit);
				}

				await Task.WhenAll(
					_database.SaveSubscriptionAsync(sub),
					_stats.AddSubscription(sub));

				var movement = SubscriptionHelper.CreateMovementFromSubscription(sub);
				if (!(movement is null))
				{
					int result = movement.BudgetId == 0
						? 1
						: await BudgetHelper.AddMovementToBudget(movement);

					switch (result)
					{
						case -1:
							await DisplayAlert(AppResource.Error, AppResource.DateOutOfBudget, "Ok");
							return;
						case 0:
							await DisplayAlert(AppResource.Error, AppResource.BudgetEndedAlert, "Ok");
							return;
						case 1:
							await Task.WhenAll(
								_database.SaveMovementAsync(movement),
								_stats.AddMovement(movement));
							break;
					}
				}
			}
			else
			{
				var movement = CreateMovementFromInput();
				if (movement is null)
				{
					await DisplayAlert(AppResource.Warning, AppResource.ValueEnteredNotValid, "Ok");
					return;
				}

				if (_isEditingMovement)
				{
					movement.Id = _toEdit.Id;
					await _stats.RemoveMovement(_toEdit);
				}

				int result = movement.BudgetId == 0
					? 1
					: await BudgetHelper.AddMovementToBudget(movement);

				switch (result)
				{
					case -1:
						await DisplayAlert(AppResource.Error, AppResource.DateOutOfBudget, "Ok");
						return;
					case 0:
						await DisplayAlert(AppResource.Error, AppResource.BudgetEndedAlert, "Ok");
						return;
				}

				await Task.WhenAll(
					_database.SaveMovementAsync(movement),
					_stats.AddMovement(movement));
			}
			await Navigation.PopAsync();
		}

		private Movement CreateMovementFromInput()
		{
			if (string.IsNullOrWhiteSpace(ValueEntry.Text) ||
				!decimal.TryParse(ValueEntry.Text, out decimal value))
				return null;

			var currencyManager = DependencyService.Get<CurrenciesManager>();
			value = currencyManager.ConvertCurrencyToDefault(
				value,
				(Currencies)CurrencyPicker.SelectedIndex);

			return new Movement()
			{
				BudgetId = IndexToBudgetId[BudgetPicker.SelectedIndex],
				CreationDate = MovementDatePicker.Date,
				Description = DescriptionEntry.Text,
				ExpenseType = _viewModel.IsExpense ? (ExpenseType)(byte)ExpenseTypePicker.SelectedIndex : ExpenseType.Others,
				IsExpense = _viewModel.IsExpense,
				Value = value
			};
		}

		private Subscription CreateSubscriptionFromInput()
		{
			if (string.IsNullOrWhiteSpace(ValueEntry.Text) ||
				!decimal.TryParse(ValueEntry.Text, out decimal value))
				return null;

			var currencyManager = DependencyService.Get<CurrenciesManager>();
			value = currencyManager.ConvertCurrencyToDefault(
				value,
				(Currencies)CurrencyPicker.SelectedIndex);

			var renewal = (RenewalType)RenewalPicker.SelectedItem;
			return new Subscription(renewal, MovementDatePicker.Date)
			{
				BudgetId = IndexToBudgetId[BudgetPicker.SelectedIndex],
				Description = DescriptionEntry.Text,
				ExpenseType = _viewModel.IsExpense ? (ExpenseType)(byte)ExpenseTypePicker.SelectedIndex : ExpenseType.Others,
				Value = value,
			};
		}

		private void RenewalLabel_Focused(object sender, FocusEventArgs e)
			=> RenewalPicker.Focus();
		private void RenewalPicker_Unfocused(object sender, FocusEventArgs e)
			=> RenewalLabel.Unfocus();

		private void ExpenseTypeLabel_Focused(object sender, FocusEventArgs e)
			=> ExpenseTypePicker.Focus();
		private void ExpensePicker_Unfocused(object sender, FocusEventArgs e)
			=> ExpenseTypeLabel.Unfocus();

		private void BudgetLabel_Focused(object sender, FocusEventArgs e)
			=> BudgetPicker.Focus();
		private void BudgetPicker_Unfocused(object sender, FocusEventArgs e)
			=> BudgetLabel.Unfocus();

		private void CurrencyLabel_Focused(object sender, FocusEventArgs e)
			=> CurrencyPicker.Focus();
		private void CurrencyPicker_Unfocused(object sender, FocusEventArgs e)
			=> CurrencyLabel.Unfocus();

		private void RenewalPicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			RenewalLabel.Text = RenewalPicker.SelectedItem.ToString();
			RenewalLabel.Unfocus();
		}

		private void ExpenseTypePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			ExpenseTypeLabel.Text = ExpenseTypePicker.SelectedItem.ToString();
			ExpenseTypeLabel.Unfocus();
		}

		private void CurrencyPicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			CurrencyLabel.Text = CurrencyPicker.SelectedItem.ToString();
			CurrencyLabel.Unfocus();
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

		private void Switch_Toggled(object sender, ToggledEventArgs e)
			=> _viewModel.ShowExpenseTypePicker = _viewModel.IsExpense;
	}
}