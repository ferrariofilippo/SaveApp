using App.Data;
using App.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App.Helpers
{
	public static class SubscriptionHelper
	{
		private static readonly AppDatabase _database = DependencyService.Get<AppDatabase>();

		private static readonly StatisticsHolder _statistics = DependencyService.Get<StatisticsHolder>();

		public static async void ValidateSubscriptions()
		{
			var subs = await _database.GetSubscriptionsAsync();
			foreach (var item in subs)
			{
				var movement = CreateMovementFromSubscription(item);
				while (!(movement is null))
				{
					if (movement.BudgetId != 0 && 
						await BudgetHelper.AddMovementToBudget(movement) == 0)
					{
						item.BudgetId = 0;
						await _database.SaveSubscriptionAsync(item);
					}
					
					await Task.WhenAll(
						_statistics.AddMovement(movement, true),
						_database.SaveMovementAsync(movement));
					movement = CreateMovementFromSubscription(item);
				}
			}
		}

		public static Movement CreateMovementFromSubscription(Subscription s)
		{
			DateTime renewal = s.NextRenewal.Date;
			if (renewal > DateTime.Today.Date)
				return null;

			Movement movement = new Movement()
			{
				BudgetId = s.BudgetId,
				CreationDate = s.NextRenewal.Date,
				Description = $"Pagamento di {s.Description} - {Constants.Months[renewal.Month]} {renewal.Year}",
				ExpenseType = s.ExpenseType,
				IsExpense = true,
				Value = s.Value,
			};
			s.UpdateNextRenewal();
			return movement;
		}
	}
}
