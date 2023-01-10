using App.Data;
using App.Helpers.Notifications;
using App.Models;
using App.Models.Enums;
using App.Resx;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App.Helpers
{
    public static class BudgetHelper
	{
		private static readonly AppDatabase _database = DependencyService.Get<AppDatabase>();

		public static async Task<AddToBudgetResult> AddMovementToBudget(Movement movement)
		{
			if (movement.BudgetId == 0)
				return AddToBudgetResult.Succeded;

			var budget = await _database.GetBudgetAsync(movement.BudgetId);
			if (budget is null)
			{
				movement.BudgetId = 0;
				return AddToBudgetResult.NotExists;
			}

			if (movement.CreationDate < budget.CreationDate || movement.CreationDate > budget.EndingDate)
				return AddToBudgetResult.DateOutOfRange;
			if (budget.Remaining <= 0.0m)
				return AddToBudgetResult.BudgetEnded;

			budget.Remaining -= movement.Value;
			budget.Used += movement.Value;

			if (budget.Remaining <= 0.00m)
			{
				var message = string.Format(
					AppResource.BudgetEnded,
					budget.Name);
				NotificationHelper.SendNotification(
					AppResource.Warning,
					message);	
			}

			await _database.SaveBudgetAsync(budget);
			return AddToBudgetResult.Succeded;
		}

		public static async Task RemoveMovementFromBudget(Movement movement)
		{
			if (movement.BudgetId == 0)
				return;

			var budget = await _database.GetBudgetAsync(movement.BudgetId);
			if (budget is null)
				return;

			budget.Used -= movement.Value;
			budget.Remaining += movement.Value;
			await _database.SaveBudgetAsync(budget);
		}

		public static async void ValidateBudgets()
		{
			var budgets = await _database.GetBudgetsAsync();
			if (budgets is null)
				return;

			foreach (var budget in budgets)
			{
				if (DateTime.Today.Date > budget.EndingDate)
				{
					var message = string.Format(
						AppResource.BudgetExpired,
						budget.Name);
					NotificationHelper.SendNotification(AppResource.BudgetExpiredTitle, message);
					await _database.DeleteBudgetAsync(budget);
				}
			}
		}
	}
}
