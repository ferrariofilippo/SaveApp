using App.Data;
using App.Helpers;
using App.Helpers.Notifications;
using App.Models;
using App.Models.Enums;
using App.ViewModels.DataViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App.ViewModels
{
    public class HistoryViewModel : ObservableObject
	{
		private readonly AppDatabase _database = DependencyService.Get<AppDatabase>();

		private readonly StatisticsHolder _stats = DependencyService.Get<StatisticsHolder>();

		public readonly int[] MonthAndDay = { DateTime.Now.Month, DateTime.Now.Day };

		private int _year;
		public int Year
		{
			get => _year;
			set
			{
				if (SetProperty(ref _year, value))
					CalendarTitle = _year.ToString();
			}
		}

		private string _calendarTitle;
		public string CalendarTitle
		{
			get => _calendarTitle;
			set => SetProperty(ref _calendarTitle, value);
		}

		private bool _isRefreshing;
		public bool IsRefreshing
		{
			get => _isRefreshing;
			set => SetProperty(ref _isRefreshing, value);
		}

		public ObservableCollection<MovementItemViewModel> Movements = new ObservableCollection<MovementItemViewModel>();

		public bool ShowEmptyLabel => Movements.Count == 0 && !FirstLoad;

		public bool FirstLoad { get; set; } = true;

		public HistoryViewModel()
		{
			Year = DateTime.Now.Year;
		}

		public async Task DeleteItemAsync(Movement m)
		{
			await BudgetHelper.RemoveMovementFromBudget(m);
			await _stats.RemoveMovement(m);
			await _database.DeleteMovementAsync(m);
		}

		public async void FilterByYear()
		{
			var items = await Filter(
				new DateTime(_year, 1, 1),
				x => x.CreationDate.Year == _year,
				SearchDepth.Year);

            UpdateMovements(items);
        }

        public async void FilterByMonth()
		{
			var items = await Filter(
				new DateTime(_year, MonthAndDay[0], 1),
				x => x.CreationDate.Year == _year && x.CreationDate.Month == MonthAndDay[0],
				SearchDepth.Month);

			UpdateMovements(items);
        }

        public async void FilterByDay()
		{
			var date = new DateTime(_year, MonthAndDay[0], MonthAndDay[1]);
			var items = await Filter(
				date,
				x => x.CreationDate.Date == date.Date,
				SearchDepth.Day);

			UpdateMovements(items);
        }

		private void UpdateMovements(List<MovementItemViewModel> movementItems)
		{
            Movements.Clear();
            movementItems.ForEach(m => Movements.Add(m));
			OnPropertyChanged(nameof(ShowEmptyLabel));
		}

		private async Task<List<MovementItemViewModel>> Filter(DateTime date, Func<Movement, bool> filterCondition, SearchDepth depth)
		{
			List<MovementItemViewModel> returnList = new List<MovementItemViewModel>();
			try
			{
				var data = (await _database.GetMovementsAsync())
					.OrderBy(x => x.CreationDate)
					.ToArray();

				if (data.Length == 0)
					return returnList;

				var index = GetStartingIndex(data, date, depth);
				if (index == -1)
					return returnList;

				for (; index < data.Length; index++)
				{
					if (filterCondition(data[index]))
						returnList.Add(new MovementItemViewModel(data[index]));
					else
						return returnList;
				}
			}
			catch (Exception ex)
			{
				NotificationHelper.NotifyException(ex);
			}

			return returnList;
		}

		private int GetStartingIndex(Movement[] items, DateTime toFind, SearchDepth depth)
		{
			var index = -1;

			switch (depth)
			{
				case SearchDepth.Year:
					index = BinarySearchDate(items, toFind.Year, 0, items.Length);
					break;
				case SearchDepth.Month:
					index = BinarySearchDate(items, toFind.Year, toFind.Month, 0, items.Length);
					break;
				case SearchDepth.Day:
					index = BinarySearchDate(items, toFind, 0, items.Length);
					break;
			}

			while (index >= 0 && items[index].CreationDate.Date >= toFind.Date)
				index--;

			return index > 0
				? ++index
				: items[0].CreationDate.Date >= toFind.Date
					? 0
					: -1;
		}

		private int BinarySearchDate(Movement[] items, int year, int low, int high)
		{
			if (low >= high)
				return -1;
			var mid = (low + high - 1) / 2;
			if (items[mid].CreationDate.Year == year)
				return mid;
			if (year < items[mid].CreationDate.Year)
				return BinarySearchDate(items, year, low, mid - 1);
			return BinarySearchDate(items, year, mid + 1, high);
		}

		private int BinarySearchDate(Movement[] items, int year, int month, int low, int high)
		{
			if (low >= high)
				return -1;
			var mid = (low + high - 1) / 2;
			if (items[mid].CreationDate.Year == year && items[mid].CreationDate.Month == month)
				return mid;
			if (year < items[mid].CreationDate.Year || month < items[mid].CreationDate.Month)
				return BinarySearchDate(items, year, month, low, mid - 1);
			return BinarySearchDate(items, year, month, mid + 1, high);
		}

		private int BinarySearchDate(Movement[] items, DateTime date, int low, int high)
		{
			if (low >= high)
				return -1;
			var mid = (low + high - 1) / 2;
			if (items[mid].CreationDate.Date == date.Date)
				return mid;
			if (date.Date < items[mid].CreationDate.Date)
				return BinarySearchDate(items, date, low, mid - 1);
			return BinarySearchDate(items, date, mid + 1, high);
		}
	}
}
