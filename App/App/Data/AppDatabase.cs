using App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using System;
using System.IO;
using System.Linq;

namespace App.Data
{
	public class AppDatabase
	{
		private readonly SQLiteAsyncConnection _database;

		public AppDatabase()
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.DbPath);
			_database = new SQLiteAsyncConnection(path);
			_database.CreateTableAsync<Movement>().Wait();
			_database.CreateTableAsync<Subscription>().Wait();
			_database.CreateTableAsync<Budget>().Wait();
		}

		public Task<List<Movement>> GetMovementsAsync() => _database.Table<Movement>().ToListAsync();

		public Task<List<Subscription>> GetSubscriptionsAsync() => _database.Table<Subscription>().ToListAsync();
		public Task<List<Budget>> GetBudgetsAsync() => _database.Table<Budget>().ToListAsync();
		public Task<Budget> GetBudgetAsync(int id) => _database.Table<Budget>().FirstOrDefaultAsync(m => m.Id == id);

		public Task<int> SaveMovementAsync(Movement movement) => movement.Id == 0 
			? _database.InsertAsync(movement) 
			: _database.UpdateAsync(movement);
		public Task<int> SaveSubscriptionAsync(Subscription subscription) => subscription.Id == 0
			? _database.InsertAsync(subscription)
			: _database.UpdateAsync(subscription); 
		public Task<int> SaveBudgetAsync(Budget budget) => budget.Id == 0
			? _database.InsertAsync(budget)
			: _database.UpdateAsync(budget);

		public Task<int> DeleteMovementAsync(Movement movement) => _database.DeleteAsync(movement);
		public Task<int> DeleteSubscriptionAsync(Subscription subscription) => _database.DeleteAsync(subscription);
		public Task<int> DeleteBudgetAsync(Budget budget) => _database.DeleteAsync(budget);

		public Task UpdateDbToNewCurrency(decimal currencyRatio) => Task.WhenAll(
				UpdateAllMovements(currencyRatio),
				UpdateAllBudgets(currencyRatio),
				UpdateAllSubscriptions(currencyRatio));

		private async Task UpdateAllMovements(decimal currencyRatio)
		{
			var movements = await GetMovementsAsync();
			movements.ForEach(m => m.Value *= currencyRatio);
			await Task.WhenAll(movements.Select(x => SaveMovementAsync(x)));
		}

		private async Task UpdateAllBudgets(decimal currencyRatio)
		{
			var budgets = await GetBudgetsAsync();
			budgets.ForEach(s =>
			{
				s.Used *= currencyRatio;
				s.MaxAmount *= currencyRatio;
				s.Remaining *= currencyRatio;
			});
			await Task.WhenAll(budgets.Select(b => SaveBudgetAsync(b)));
		}

		private async Task UpdateAllSubscriptions(decimal currencyRatio)
		{
			var subs = await GetSubscriptionsAsync();
			subs.ForEach(s => s.Value *= currencyRatio);
			await Task.WhenAll(subs.Select(s => SaveSubscriptionAsync(s)));
		}
	}
}
