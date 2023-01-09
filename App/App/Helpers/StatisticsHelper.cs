using App.Data;
using System;
using System.Linq;
using Xamarin.Forms;

namespace App.Helpers
{
	public static class StatisticsHelper
	{
		private static readonly StatisticsHolder _statistics = DependencyService.Get<StatisticsHolder>();

		private static readonly AppDatabase _database = DependencyService.Get<AppDatabase>();

		public static async void CheckStatisticsForReset()
		{
			var lastMovement = (await _database.GetMovementsAsync()).LastOrDefault();
			if (lastMovement is null || lastMovement.CreationDate.Year >= DateTime.Now.Year)
				return;

			var allZeroes = true;
			for (int i = 0; i < 12 && allZeroes; i++)
			{
				allZeroes = _statistics.Statistics.ExpensesByType[i] == 0 &&
					_statistics.Statistics.ExpensesByMonth[i] == 0;
			}
			if (allZeroes)
				return;

			await _statistics.EmptyPreviousYear();
		}
	}
}
