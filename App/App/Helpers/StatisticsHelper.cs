using App.Data;
using System;
using System.Linq;
using Xamarin.Forms;

namespace App.Helpers
{
	public static class StatisticsHelper
	{
		private static readonly StatisticsManager _statistics = DependencyService.Get<StatisticsManager>();

		private static readonly IAppDatabase _database = DependencyService.Get<IAppDatabase>();

		public static async void CheckStatisticsForReset()
		{
			var lastMovement = (await _database.GetMovementsAsync()).LastOrDefault();
			if (lastMovement is null || lastMovement.CreationDate.Year >= DateTime.Now.Year)
				return;

			var areAllZeroes = true;
			for (int i = 0; i < _statistics.Statistics.ExpensesByType.Count && areAllZeroes; i++)
				areAllZeroes = _statistics.Statistics.ExpensesByType[i] == 0;

			for (int i = 0; i < _statistics.Statistics.ExpensesByMonth.Count && areAllZeroes; i++)
                areAllZeroes = _statistics.Statistics.ExpensesByMonth[i] == 0;

            if (areAllZeroes)
				return;
			await _statistics.EmptyPreviousYear();
		}
	}
}
