using App.Data;
using App.Models;
using App.Resx;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App.Helpers
{
	public static class BudgetHelper
	{
		private static readonly AppDatabase _database = DependencyService.Get<AppDatabase>();

		public static async Task<int> AddMovementToBudget(Movement m)
		{
			var budget = await _database.GetBudgetAsync(m.BudgetId);
			if (budget is null)
			{
				m.BudgetId = 0;
				return -2;
			}

			if (m.CreationDate < budget.CreationDate || m.CreationDate > budget.EndingDate)
				return -1;
			if (budget.Remaining <= 0.0m)
				return 0;

			budget.Remaining -= m.Value;
			budget.Used += m.Value;

			if (budget.Remaining <= 0.00m)
			{
				string message = string.Format(
					AppResource.BudgetEnded,
					budget.Name);
				NotificationHelper.SendNotification(
					AppResource.Warning,
					message);	
			}

			return await _database.SaveBudgetAsync(budget);
		}

		public static async Task<int> RemoveMovementFromBudget(Movement m)
		{
			if (m.BudgetId == 0)
				return -1;

			var budget = await _database.GetBudgetAsync(m.BudgetId);
			if (budget is null)
				return -1;

			budget.Used -= m.Value;
			budget.Remaining += m.Value;
			return await _database.SaveBudgetAsync(budget);
		}

		public static async void ValidateBudgets()
		{
			var budgets = await _database.GetBudgetsAsync();
			foreach (var item in budgets)
			{
				if (DateTime.Today.Date > item.EndingDate)
				{
					string message = string.Format(
						AppResource.BudgetExpired,
						item.Name);
					NotificationHelper.SendNotification(AppResource.BudgetExpiredTitle, message);
					await _database.DeleteBudgetAsync(item);
				}
			}
		}
	}
}
