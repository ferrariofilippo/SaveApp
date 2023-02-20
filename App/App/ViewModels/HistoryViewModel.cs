using App.Data;
using App.Helpers;
using App.Helpers.Notifications;
using App.Models;
using App.Models.Enums;
using App.Resx;
using App.ViewModels.DataViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App.ViewModels
{
	public struct MonthAndDay
	{
		public int Month { get; set; }
		public int Day { get; set; }

		public MonthAndDay(int month, int day)
		{
			Month = month;
			Day = day;
		}
	}

	public sealed class HistoryViewModel : ObservableObject
	{
		private static CultureInfo Culture => CultureInfo.CurrentCulture;

		private readonly IAppDatabase _database = DependencyService.Get<IAppDatabase>();

		private readonly StatisticsManager _stats = DependencyService.Get<StatisticsManager>();

		public readonly string[] Categories = Enum.GetValues(typeof(ExpenseType))
			.Cast<ExpenseType>()
			.Select(x => App.ResourceManager.GetString(x.ToString(), Culture))
			.Union(new string[] { AppResource.None })
			.ToArray();

		public MonthAndDay Date = new MonthAndDay(DateTime.Now.Month, DateTime.Now.Day);

		public ObservableCollection<MovementItemViewModel> Movements = new ObservableCollection<MovementItemViewModel>();

		public bool FirstLoad { get; set; } = true;

		public string DescriptionFilterString { get; set; }

		public bool ShowEmptyLabel => Movements.Count == 0 && !FirstLoad;

		public Color FilterCategoryColor => IsFilteringByType ? ReadOnlies.MovementTypeColors[(int)TypeFilter] : Color.Transparent;

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

		private SortOrder _sortOrder;
		public SortOrder SortOrder
		{
			get => _sortOrder;
			set => SetProperty(ref _sortOrder, value);
		}

		private ExpenseType _typeFilter;
		public ExpenseType TypeFilter
		{
			get => _typeFilter;
			set
			{
				if (SetProperty(ref _typeFilter, value))
					OnPropertyChanged(nameof(FilterCategoryColor));
			}
		}

		private bool _isFilteringByType;
		public bool IsFilteringByType
		{
			get => _isFilteringByType;
			set
			{
				if (SetProperty(ref _isFilteringByType, value))
					OnPropertyChanged(nameof(FilterCategoryColor));
			}
		}

		private bool _isFilteringByDescription;
		public bool IsFilteringByDescription
		{
			get => _isFilteringByDescription;
			set => SetProperty(ref _isFilteringByDescription, value);
		}

		private bool _areFilterButtonsEnabled = true;
		public bool AreFilterButtonsEnabled
		{
			get => _areFilterButtonsEnabled;
			set => SetProperty(ref _areFilterButtonsEnabled, value);
		}

		public HistoryViewModel()
		{
			Year = DateTime.Now.Year;
		}

		public Task OrderHistory()
		{
			AreFilterButtonsEnabled = false;
			var sortedMovements = Movements.Reverse().ToArray();
			Movements.Clear();
			foreach (var item in sortedMovements)
				Movements.Add(item);
			AreFilterButtonsEnabled = true;
			return Task.CompletedTask;
		}

		public async Task DeleteItemAsync(Movement m)
		{
			await BudgetHelper.RemoveMovementFromBudget(m);
			await _stats.RemoveMovement(m);
			await _database.DeleteMovementAsync(m);
		}

		public async void FilterByYear()
		{
			await Filter(
				new DateTime(_year, 1, 1),
				m => m.CreationDate.Year == _year,
				SearchDepth.Year).ConfigureAwait(false);
		}

		public async void FilterByMonth()
		{
			await Filter(
				new DateTime(_year, Date.Month, 1),
				m => m.CreationDate.Year == _year && m.CreationDate.Month == Date.Month,
				SearchDepth.Month).ConfigureAwait(false);
		}

		public async void FilterByDay()
		{
			var date = new DateTime(_year, Date.Month, Date.Day);
			await Filter(
				date,
				m => m.CreationDate.Date == date.Date,
				SearchDepth.Day).ConfigureAwait(false);
		}

		private async Task Filter(DateTime date, Func<Movement, bool> filterCondition, SearchDepth depth)
		{
			try
			{
				Movements.Clear();
				var data = (await _database.GetMovementsAsync())
					.OrderBy(x => x.CreationDate)
					.ToArray();

				if (!data.Any())
					return;

				var index = FilterHelpers.GetStartingIndex(data, date, depth);
				if (index == -1)
					return;

				if (SortOrder == SortOrder.Ascending)
					InsertAscending(index, data, filterCondition);
				else
					InsertDescending(index, data, filterCondition);
			}
			catch (Exception ex)
			{
				NotificationHelper.NotifyException(ex);
			}
			finally
			{
				AreFilterButtonsEnabled = true;
				OnPropertyChanged(nameof(ShowEmptyLabel));
			}
		}

		private void InsertAscending(int index, Movement[] movementsToAdd, Func<Movement, bool> condition)
		{
			for (; index < movementsToAdd.Length; index++)
			{
				if (condition(movementsToAdd[index]))
				{
					if ((!IsFilteringByType || movementsToAdd[index].ExpenseType == TypeFilter) &&
						(!IsFilteringByDescription || movementsToAdd[index].Description.ToLower().Contains(DescriptionFilterString)))
						Movements.Add(new MovementItemViewModel(movementsToAdd[index]));
				}
				else
					return;
			}
		}

		private void InsertDescending(int index, Movement[] movementsToAdd, Func<Movement, bool> condition)
		{
			for (; index < movementsToAdd.Length; index++)
			{
				if (condition(movementsToAdd[index]))
				{
					if ((!IsFilteringByType || movementsToAdd[index].ExpenseType == TypeFilter) &&
						(!IsFilteringByDescription || movementsToAdd[index].Description.ToLower().Contains(DescriptionFilterString)))
						Movements.Insert(0, new MovementItemViewModel(movementsToAdd[index]));
				}
				else
					return;
			}
		}
	}
}
