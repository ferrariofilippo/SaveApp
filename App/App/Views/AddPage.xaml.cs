using App.Data;
using App.Helpers;
using App.Helpers.UIHelpers;
using App.Models;
using App.Models.Enums;
using App.Resx;
using App.ViewModels;
using System;
using System.Collections.Generic;
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
            MovementDatePicker.IsVisible = SubscriptionSwitch.IsVisible = false;
            _viewModel.IsExpense = _viewModel.IsSubscription = true;
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

        private async void Cancel_Clicked(object sender, EventArgs e)
            => await Navigation.PopAsync();

        private async void Save_Clicked(object sender, EventArgs e)
        {
            var (isValid, value) = await ValidateInput();
            if (!isValid)
                return;

            if ((_viewModel.IsSubscription && await CreateAndSaveSubscription(value)) || await CreateAndSaveMovement(value))
                await Navigation.PopAsync();
        }

        private async Task<(bool, decimal)> ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(ValueEntry.Text) ||
                !decimal.TryParse(ValueEntry.Text, out decimal value) ||
                value <= 0.0m)
            {
                await ShowWarning(AppResource.ValueEnteredNotValid);
                return (false, 0.0m);
            }
            if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
            {
                await ShowWarning(AppResource.DescriptionEnteredNotValid);
                return (false, 0.0m);
            }
            if (ExpenseTypePicker.SelectedIndex == -1 && _viewModel.IsExpense)
            {
                await ShowWarning(AppResource.TypeSelectedNotValid);
                return (false, 0.0m);
            }

            var currencyManager = DependencyService.Get<CurrenciesManager>();
            var selectedCurrency = (Currencies)CurrencyPicker.SelectedIndex;
            return (true, currencyManager.ConvertCurrencyToDefault(value, selectedCurrency));
        }

        private async Task<bool> CreateAndSaveSubscription(decimal value)
        {
            if (RenewalPicker.SelectedIndex == -1)
            {
                await DisplayAlert(AppResource.Warning, AppResource.RenewalSelectedNotValid, "Ok");
                return false;
            }

            var renewal = (RenewalType)RenewalPicker.SelectedItem;
            var subscription = new Subscription(renewal, MovementDatePicker.Date)
            {
                BudgetId = IndexToBudgetId[BudgetPicker.SelectedIndex],
                Description = DescriptionEntry.Text,
                ExpenseType = _viewModel.IsExpense ? (ExpenseType)ExpenseTypePicker.SelectedIndex : ExpenseType.Others,
                Value = value,
            };

            if (_isEditingSubscription)
            {
                subscription.Id = _subToEdit.Id;
                subscription.CreationDate = _subToEdit.CreationDate;
                subscription.NextRenewal = _subToEdit.NextRenewal;
                subscription.LastPaid = _subToEdit.LastPaid;
            }

            await Task.WhenAll(
                _database.SaveSubscriptionAsync(subscription),
                _stats.AddSubscription(subscription));

            var movement = SubscriptionHelper.CreateMovementFromSubscription(subscription);
            if (!(movement is null))
            {
                var result = movement.BudgetId == 0
                    ? 1
                    : await BudgetHelper.AddMovementToBudget(movement);

                if (result == -1)
                    await ShowError(AppResource.DateOutOfBudget);
                else if (result == 0)
                    await ShowError(AppResource.BudgetEndedAlert);
                else
                    await Task.WhenAll(
                        _database.SaveMovementAsync(movement),
                        _stats.AddMovement(movement));
            }
            return true;
        }

        private async Task<bool> CreateAndSaveMovement(decimal value)
        {
            var movement = new Movement()
            {
                Id = _isEditingMovement ? _toEdit.Id : 0,
                BudgetId = IndexToBudgetId[BudgetPicker.SelectedIndex],
                CreationDate = MovementDatePicker.Date,
                Description = DescriptionEntry.Text,
                ExpenseType = _viewModel.IsExpense ? (ExpenseType)ExpenseTypePicker.SelectedIndex : ExpenseType.Others,
                IsExpense = _viewModel.IsExpense,
                Value = value
            };

            if (_isEditingMovement)
            {
                var result = await BudgetHelper.AddMovementToBudget(movement);
                if (result == -1)
                {
                    await ShowError(AppResource.DateOutOfBudget);
                    return false;
                }
                else if (result == 0)
                {
                    await ShowError(AppResource.BudgetEndedAlert);
                    return false;
                }
            }

            await Task.WhenAll(
                _database.SaveMovementAsync(movement),
                _stats.AddMovement(movement));
            return true;
        }

        private void MovementDateLabel_Focused(object sender, FocusEventArgs e)
            => MovementDatePicker.Focus();
        private void DatePicker_Unfocused(object _, FocusEventArgs e)
            => MovementDateLabel.Unfocus();
        private void MovementDatePicker_DateSelected(object _, DateChangedEventArgs e)
        {
            MovementDateLabel.Text = MovementDatePicker.Date.ToString("dd/MM/yyyy");
            MovementDateLabel.Unfocus();
        }

        private void RenewalLabel_Focused(object _, FocusEventArgs e)
            => RenewalPicker.Focus();
        private void RenewalPicker_Unfocused(object _, FocusEventArgs e)
            => RenewalLabel.Unfocus();
        private void RenewalPicker_SelectedIndexChanged(object _, EventArgs e)
        {
            RenewalLabel.Text = RenewalPicker.SelectedItem.ToString();
            RenewalLabel.Unfocus();
        }

        private void ExpenseTypeLabel_Focused(object _, FocusEventArgs e)
            => ExpenseTypePicker.Focus();
        private void ExpensePicker_Unfocused(object _, FocusEventArgs e)
            => ExpenseTypeLabel.Unfocus();
        private void ExpenseTypePicker_SelectedIndexChanged(object _, EventArgs e)
        {
            ExpenseTypeLabel.Text = ExpenseTypePicker.SelectedItem.ToString();
            ExpenseTypeLabel.Unfocus();
        }

        private void BudgetLabel_Focused(object _, FocusEventArgs e)
            => BudgetPicker.Focus();
        private void BudgetPicker_Unfocused(object _, FocusEventArgs e)
            => BudgetLabel.Unfocus();
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

        private void CurrencyLabel_Focused(object _, FocusEventArgs e)
            => CurrencyPicker.Focus();
        private void CurrencyPicker_Unfocused(object _, FocusEventArgs e)
            => CurrencyLabel.Unfocus();
        private void CurrencyPicker_SelectedIndexChanged(object _, EventArgs e)
        {
            CurrencyLabel.Text = CurrencyPicker.SelectedItem.ToString();
            CurrencyLabel.Unfocus();
        }

        private void ValueEntry_TextChanged(object _, TextChangedEventArgs e)
            => EntryHelpers.AdjustCurrencyEntryText(ValueEntry, e);

        private void Switch_Toggled(object _, ToggledEventArgs e)
            => _viewModel.ShowExpenseTypePicker = _viewModel.IsExpense;

        private Task ShowWarning(string message)
            => DisplayAlert(AppResource.Warning, message, "OK");

        private Task ShowError(string message)
            => DisplayAlert(AppResource.Error, message, "OK");

        private int IndexFromId(int id)
        {
            foreach (var item in IndexToBudgetId.Keys)
            {
                if (IndexToBudgetId[item] == id)
                    return item;
            }
            return 0;
        }
    }
}