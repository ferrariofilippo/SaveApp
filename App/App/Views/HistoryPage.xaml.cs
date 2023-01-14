using App.Helpers;
using App.Resx;
using App.ViewModels;
using App.ViewModels.DataViewModels;
using System;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HistoryPage : ContentPage
	{
		private readonly HistoryViewModel _viewModel = new HistoryViewModel();

		private readonly Guid[] _monthButtons = new Guid[12];

		private byte _filterDepth;

		private int _lastMonthLength = 31;

		public HistoryPage()
		{
			InitializeComponent();
			this.BindingContext = _viewModel;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			HistoryListView.ItemsSource = _viewModel.Movements;
			CreateCalendar();
			Refresh_ListView(MainView, EventArgs.Empty);
			_viewModel.FirstLoad = false;
		}

		private void CreateCalendar()
		{
			var textColor = (Color)App.Current.Resources["ForegroundMainColor"];

			MonthGrid.Children.Clear();

			for (byte i = 0; i < 12; i++)
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
				MonthGrid.Children.Add(btn, i % 4, i / 4);
			}

			DayGrid.Children.Clear();
			for (byte i = 0; i < 31; i++)
			{
				var btn = new Button()
				{
					BackgroundColor = Color.Transparent,
					Text = (i + 1).ToString(),
					TextColor = textColor,
					Visual = VisualMarker.Default
				};
				btn.Clicked += DayClicked;
				DayGrid.Children.Add(btn, i % 7, i / 7);
			}
		}

		private void MonthClicked(object sender, EventArgs e)
		{
			_filterDepth = 1;
			var btn = (Button)sender;
			FocusMonth(_monthButtons.IndexOf(btn.Id) + 1);
			MonthGrid.IsVisible = false;
			DayGrid.IsVisible = true;
			OnPropertyChanged(nameof(DayGrid));
		}

		private void DayClicked(object sender, EventArgs e)
		{
			_filterDepth = 2;
			var btn = (Button)sender;
			FocusDay(int.Parse(btn.Text));
		}

		private void BackClicked(object sender, EventArgs e)
		{
			if (_filterDepth == 0)
				FocusYear(_viewModel.Year - 1);
			else if (_filterDepth == 1 && _viewModel.MonthAndDay[0] > 1)
				FocusMonth(_viewModel.MonthAndDay[0] - 1);
			else if (_filterDepth == 2 && _viewModel.MonthAndDay[1] > 1)
				FocusDay(_viewModel.MonthAndDay[1] - 1);
		}

		private void ForwardClicked(object sender, EventArgs e)
		{
			if (_filterDepth == 0)
				FocusYear(_viewModel.Year + 1);
			else if (_filterDepth == 1 && _viewModel.MonthAndDay[0] < 12)
				FocusMonth(_viewModel.MonthAndDay[0] + 1);
			else if (_filterDepth == 2 && _viewModel.MonthAndDay[1] < _lastMonthLength)
				FocusDay(_viewModel.MonthAndDay[1] + 1);
		}

		private void LatterClicked(object sender, EventArgs e)
		{
			if (_filterDepth == 0)
				return;
			if (_filterDepth == 1)
			{
				_filterDepth--;
				_viewModel.CalendarTitle = _viewModel.Year.ToString();
				FocusYear(_viewModel.Year);
				DayGrid.IsVisible = false;
				MonthGrid.IsVisible = true;
				OnPropertyChanged(nameof(MonthGrid));
				return;
			}

			_filterDepth = 1;
			FocusMonth(_viewModel.MonthAndDay[0]);
		}

		private void Refresh_ListView(object sender, EventArgs e)
		{
			switch (_filterDepth)
			{
				case 0:
					_viewModel.FilterByYear();
					break;
				case 1:
					_viewModel.FilterByMonth();
					break;
				case 2:
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

			var message = new StringBuilder();
			message.AppendLine($"{AppResource.Description}: {mvDisplay.Movement.Description,30}");
			message.AppendLine($"{AppResource.Value}: {mvDisplay.ValueString,30}");
			message.AppendLine($"{AppResource.Date}: {mvDisplay.DateString,30}");
			message.Append($"{AppResource.ExpenseType}: {App.ResourceManager.GetString(mvDisplay.Movement.ExpenseType.ToString()),30}");

			DisplayAlert(AppResource.Movement, message.ToString(), "Ok");
		}

		private async void ChangeSortingOrder_Clicked(object sender, EventArgs e)
		{
			if (_viewModel.SortOrder == Models.Enums.SortOrder.Ascending)
				_viewModel.SortOrder = Models.Enums.SortOrder.Descending;
			else
				_viewModel.SortOrder = Models.Enums.SortOrder.Ascending;
			await _viewModel.OrderHistory();
		}

		private void FocusDay(int day)
		{
			_viewModel.MonthAndDay[1] = day;
			_viewModel.CalendarTitle = $"{_viewModel.MonthAndDay[1]} {App.ResourceManager.GetString(ReadOnlies.Months[_viewModel.MonthAndDay[0] - 1])}";
			_viewModel.FilterByDay();
		}

		private void FocusMonth(int month)
		{
			_viewModel.MonthAndDay[0] = month;
			_viewModel.CalendarTitle = App.ResourceManager.GetString(
				ReadOnlies.Months[month - 1]);

            var monthLength = DateTime.DaysInMonth(_viewModel.Year, _viewModel.MonthAndDay[0]);
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
	}
}