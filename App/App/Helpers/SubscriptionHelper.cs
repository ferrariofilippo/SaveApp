using App.Data;
using App.Models;
using App.Resx;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App.Helpers
{
	public static class SubscriptionHelper
	{
		private static readonly IAppDatabase _database = DependencyService.Get<IAppDatabase>();

		private static readonly StatisticsManager _statistics = DependencyService.Get<StatisticsManager>();

		public static async void ValidateSubscriptions()
		{
			var subs = await _database.GetSubscriptionsAsync();
			if (subs is null)
				return;
			foreach (var item in subs)
			{
				var movement = await CreateMovementFromSubscription(item);
				while (!(movement is null))
				{
					if (await BudgetHelper.AddMovementToBudget(movement) != Models.Enums.AddToBudgetResult.Succeded)
					{
						item.BudgetId = 0;
						await _database.SaveSubscriptionAsync(item);
					}
					
					await Task.WhenAll(
						_statistics.AddMovement(movement, true),
						_database.SaveMovementAsync(movement));
					movement = await CreateMovementFromSubscription(item);
				}
			}
		}

		public static async Task<Movement> CreateMovementFromSubscription(Subscription subscription)
		{
			var renewal = subscription.NextRenewal.Date;
			if (renewal > DateTime.Today.Date)
				return null;

			var paymentOf = string.Format(AppResource.PaymentOf, subscription.Description);
			var month = App.ResourceManager.GetString(ReadOnlies.Months[renewal.Month - 1], CultureInfo.CurrentCulture);

			var movement = new Movement()
			{
				BudgetId = subscription.BudgetId,
				CreationDate = subscription.NextRenewal.Date,
				Description = $"{paymentOf} - {month} {renewal.Year}",
				ExpenseType = subscription.ExpenseType,
				IsExpense = true,
				Value = subscription.Value,
			};
			subscription.UpdateNextRenewal();
			await _database.SaveSubscriptionAsync(subscription);
			return movement;
		}
	}
}
