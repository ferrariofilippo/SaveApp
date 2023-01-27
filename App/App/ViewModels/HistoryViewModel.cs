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
	public class HistoryViewModel : ObservableObject
	{
		private const string ASCENDING_ICON_SOURCE = "ascending.png";
		private const string DESCENDING_ICON_SOURCE = "descending.png";

		private static CultureInfo Culture => CultureInfo.CurrentCulture;

		private readonly IAppDatabase _database = DependencyService.Get<IAppDatabase>();

		private readonly StatisticsManager _stats = DependencyService.Get<StatisticsManager>();

		public readonly string[] Categories = Enum.GetValues(typeof(ExpenseType))
			.Cast<ExpenseType>()
			.Select(x => App.ResourceManager.GetString(x.ToString(), Culture))
			.Union(new string[] { AppResource.None })
			.ToArray();

		public readonly int[] MonthAndDay = { DateTime.Now.Month, DateTime.Now.Day };

		public ObservableCollection<MovementItemViewModel> Movements = new ObservableCollection<MovementItemViewModel>();

		public bool ShowEmptyLabel => Movements.Count == 0 && !FirstLoad;

		public bool FirstLoad { get; set; } = true;

		public string SortIconSource => SortOrder == SortOrder.Ascending ? DESCENDING_ICON_SOURCE : ASCENDING_ICON_SOURCE;

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
			set
			{
				if (SetProperty(ref _sortOrder, value))
					OnPropertyChanged(nameof(SortIconSource));
			}
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

		private bool _isSortButtonEnabled = true;
		public bool IsSortButtonEnabled
		{
			get => _isSortButtonEnabled;
			set => SetProperty(ref _isSortButtonEnabled, value);
		}

		private bool _isFilterButtonEnabled = true;
		public bool IsFilterButtonEnabled
		{
			get => _isFilterButtonEnabled;
			set => SetProperty(ref _isFilterButtonEnabled, value);
		}

		public HistoryViewModel()
		{
			Year = DateTime.Now.Year;
		}

		public Task OrderHistory()
		{
			IsSortButtonEnabled = false;
			var sortedMovements = Movements.Reverse().ToArray();
			Movements.Clear();
			foreach (var item in sortedMovements)
				Movements.Add(item);
			IsSortButtonEnabled = true;
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
				new DateTime(_year, MonthAndDay[0], 1),
				m => m.CreationDate.Year == _year && m.CreationDate.Month == MonthAndDay[0],
				SearchDepth.Month).ConfigureAwait(false);
		}

		public async void FilterByDay()
		{
			var date = new DateTime(_year, MonthAndDay[0], MonthAndDay[1]);
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

				if (data.Length == 0)
					return;

				if (IsFilteringByType)
					data = data.Where(mv => mv.ExpenseType == TypeFilter).ToArray();

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
				IsFilterButtonEnabled = true;
				OnPropertyChanged(nameof(ShowEmptyLabel));
			}
		}

		private void InsertAscending(int index, Movement[] movementsToAdd, Func<Movement, bool> condition)
		{
			for (; index < movementsToAdd.Length; index++)
			{
				if (condition(movementsToAdd[index]))
					Movements.Add(new MovementItemViewModel(movementsToAdd[index]));
				else
					return;
			}
		}

		private void InsertDescending(int index, Movement[] movementsToAdd, Func<Movement, bool> condition)
		{
			for (; index < movementsToAdd.Length; index++)
			{
				if (condition(movementsToAdd[index]))
					Movements.Insert(0, new MovementItemViewModel(movementsToAdd[index]));
				else
					return;
			}
		}
	}
}
