﻿using App.Helpers;
using App.Models.Enums;
using App.Resx;
using App.ViewModels;
using App.ViewModels.DataViewModels;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistoryPage : ContentPage
	{
		private const int CALENDAR_COLUMNS = 4;
		private const int MAX_DAYS_IN_MONTH = 31;

		private readonly HistoryViewModel _viewModel = new HistoryViewModel();

		private readonly Guid[] _monthButtons = new Guid[Constants.MONTHS_IN_YEAR];

		private SearchDepth _filterDepth = SearchDepth.Year;

		private int _lastMonthLength = MAX_DAYS_IN_MONTH;

		public HistoryPage()
		{
			InitializeComponent();
			this.BindingContext = _viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			FilterByTypePicker.ItemsSource = _viewModel.Categories;
			HistoryListView.ItemsSource = _viewModel.Movements;
			CreateCalendar();
			Refresh_ListView(MainView, EventArgs.Empty);
			_viewModel.FirstLoad = false;
		}

		private void CreateCalendar()
		{
			var textColor = (Color)App.Current.Resources["ForegroundMainColor"];

			MonthGrid.Children.Clear();

			for (byte i = 0; i < Constants.MONTHS_IN_YEAR; i++)
			{
				var btn = new Button()
				{
					BackgroundColor = Color.Transparent,
					Text = App.ResourceManager.GetString(ReadOnlies.Months[i], CultureInfo.CurrentCulture),
					TextColor = textColor,
					Visual = VisualMarker.Default
				};
				btn.Clicked += MonthClicked;
				_monthButtons[i] = btn.Id;
				MonthGrid.Children.Add(btn, i % CALENDAR_COLUMNS, i / CALENDAR_COLUMNS);
			}

			DayGrid.Children.Clear();
			for (byte i = 0; i < MAX_DAYS_IN_MONTH; i++)
			{
				var btn = new Button()
				{
					BackgroundColor = Color.Transparent,
					Text = (i + 1).ToString(),
					TextColor = textColor,
					Visual = VisualMarker.Default
				};
				btn.Clicked += DayClicked;
				DayGrid.Children.Add(btn, i % Constants.DAYS_IN_WEEK, i / Constants.DAYS_IN_WEEK);
			}
		}

		private void MonthClicked(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			_filterDepth = SearchDepth.Month;
			FocusMonth(_monthButtons.IndexOf(btn.Id) + 1);
			MonthGrid.IsVisible = false;
			DayGrid.IsVisible = true;
			OnPropertyChanged(nameof(DayGrid));
		}

		private void DayClicked(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			_filterDepth = SearchDepth.Day;
			FocusDay(int.Parse(btn.Text));
		}

		private void BackClicked(object sender, EventArgs e)
		{
			if (_filterDepth is SearchDepth.Year)
				FocusYear(_viewModel.Year - 1);
			else if (_filterDepth is SearchDepth.Month && _viewModel.Date.Month > 1)
				FocusMonth(_viewModel.Date.Month - 1);
			else if (_filterDepth is SearchDepth.Day && _viewModel.Date.Day > 1)
				FocusDay(_viewModel.Date.Day - 1);
		}

		private void ForwardClicked(object sender, EventArgs e)
		{
			if (_filterDepth is SearchDepth.Year)
				FocusYear(_viewModel.Year + 1);
			else if (_filterDepth is SearchDepth.Month && _viewModel.Date.Month < 12)
				FocusMonth(_viewModel.Date.Month + 1);
			else if (_filterDepth is SearchDepth.Day && _viewModel.Date.Day < _lastMonthLength)
				FocusDay(_viewModel.Date.Day + 1);
		}

		private void LatterClicked(object sender, EventArgs e)
		{
			if (_filterDepth == SearchDepth.Month)
			{
				_filterDepth = SearchDepth.Year;
				_viewModel.CalendarTitle = _viewModel.Year.ToString();
				FocusYear(_viewModel.Year);
				DayGrid.IsVisible = false;
				MonthGrid.IsVisible = true;
				OnPropertyChanged(nameof(MonthGrid));
				return;
			}
			else if (_filterDepth == SearchDepth.Day)
			{
				_filterDepth = SearchDepth.Month;
				FocusMonth(_viewModel.Date.Month);
			}
		}

		private void Refresh_ListView(object sender, EventArgs e)
		{
			switch (_filterDepth)
			{
				case SearchDepth.Year:
					_viewModel.FilterByYear();
					break;
				case SearchDepth.Month:
					_viewModel.FilterByMonth();
					break;
				case SearchDepth.Day:
					_viewModel.FilterByDay();
					break;
			}
			_viewModel.IsRefreshing = false;
		}

		private async void SwipeItem_DeleteInvoked(object sender, EventArgs e)
		{
			if (!await DisplayAlert(AppResource.Warning, AppResource.DeleteItemMessage, AppResource.Delete, AppResource.Cancel))
				return;
			var toDelete = ((MovementItemViewModel)((SwipeItem)sender).Parent.BindingContext).Movement;
			await BudgetHelper.RemoveMovementFromBudget(toDelete);
			await _viewModel.DeleteItemAsync(toDelete);
			Refresh_ListView(MainView, EventArgs.Empty);
		}

		private async void SwipeItem_EditInvoked(object sender, EventArgs e)
		{
			var toEdit = ((MovementItemViewModel)((SwipeItem)sender).Parent.BindingContext).Movement;
			await Navigation.PushAsync(new AddPage(toEdit));
		}

		private void SwipeItem_InfoInvoked(object sender, EventArgs e)
		{
			var mvDisplay = (MovementItemViewModel)((SwipeItem)sender).Parent.BindingContext;
			DisplayAlert(AppResource.Movement, mvDisplay.ToString(), "Ok");
		}

		private async void ChangeSortingOrder_Clicked(object sender, EventArgs e)
		{
			_viewModel.SortOrder = _viewModel.SortOrder is SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
			await _viewModel.OrderHistory();
		}

		private void FocusDay(int day)
		{
			_viewModel.Date.Day = day;
			_viewModel.CalendarTitle = $"{_viewModel.Date.Day} {App.ResourceManager.GetString(ReadOnlies.Months[_viewModel.Date.Month - 1])}";
			_viewModel.FilterByDay();
		}

		private void FocusMonth(int month)
		{
			_viewModel.Date.Month = month;
			_viewModel.CalendarTitle = App.ResourceManager.GetString(
				ReadOnlies.Months[month - 1]);

			var monthLength = DateTime.DaysInMonth(_viewModel.Year, _viewModel.Date.Month);
			if (monthLength == _lastMonthLength)
				return;

			if (monthLength > _lastMonthLength)
				for (int i = _lastMonthLength; i < monthLength; i++)
					((Button)DayGrid.Children[i]).IsVisible = true;
			else
				for (int i = monthLength; i < _lastMonthLength; i++)
					((Button)DayGrid.Children[i]).IsVisible = false;

			_lastMonthLength = monthLength;

			_viewModel.FilterByMonth();
		}

		private void FocusYear(int year)
		{
			_viewModel.Year = year;
			_viewModel.FilterByYear();
		}

		private void ChangeFilter_Clicked(object sender, EventArgs e)
			=> FilterByTypePicker.Focus();

		private void FilterByTypePicker_SelectedIndexChanged(object sender, EventArgs e)
		{
			var filterSelected = FilterByTypePicker.SelectedIndex != 12;
			_viewModel.AreFilterButtonsEnabled = false;
			_viewModel.IsFilteringByType = filterSelected;
			if (filterSelected)
				_viewModel.TypeFilter = (ExpenseType)FilterByTypePicker.SelectedIndex;

			Refresh_ListView(null, null);
		}

		private async void DescriptionFilter_Clicked(object sender, EventArgs e)
		{
			if (_viewModel.IsFilteringByDescription)
				_viewModel.IsFilteringByDescription = false;
			else
			{
				var query = await DisplayPromptAsync(
					AppResource.Search,
					" ".PadRight(40, ' '),
					cancel: AppResource.Cancel,
					placeholder: AppResource.SearchPlaceholder,
					maxLength: 20);

				if (string.IsNullOrWhiteSpace(query) || query.ToLower() == _viewModel.DescriptionFilterString)
					return;

				_viewModel.IsFilteringByDescription = true;
				_viewModel.DescriptionFilterString = query.ToLower();
			}
			Refresh_ListView(null, null);
		}
	}
}