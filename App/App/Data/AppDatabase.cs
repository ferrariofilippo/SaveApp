using App.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace App.Data
{
	public class AppDatabase : IAppDatabase
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
		public Task<int> SaveMovementsAsync(IEnumerable<Movement> movements) => _database.InsertAllAsync(movements);
		public Task<int> SaveSubscriptionAsync(Subscription subscription) => subscription.Id == 0
			? _database.InsertAsync(subscription)
			: _database.UpdateAsync(subscription);
		public Task<int> SaveBudgetAsync(Budget budget) => budget.Id == 0
			? _database.InsertAsync(budget)
			: _database.UpdateAsync(budget);

		public Task<int> DeleteMovementAsync(Movement movement) => _database.DeleteAsync(movement);
		public Task<int> DeleteSubscriptionAsync(Subscription subscription) => _database.DeleteAsync(subscription);
		public Task<int> DeleteBudgetAsync(Budget budget) => _database.DeleteAsync(budget);

		public Task UpdateDbToNewCurrency(decimal changeRatio) => Task.WhenAll(
				UpdateAllMovements(changeRatio),
				UpdateAllBudgets(changeRatio),
				UpdateAllSubscriptions(changeRatio));

		private async Task UpdateAllMovements(decimal changeRatio)
		{
			var movements = await GetMovementsAsync();
			movements.ForEach(m => m.Value *= changeRatio);
			await Task.WhenAll(movements.Select(x => SaveMovementAsync(x)));
		}

		private async Task UpdateAllBudgets(decimal changeRatio)
		{
			var budgets = await GetBudgetsAsync();
			budgets.ForEach(s =>
			{
				s.Used *= changeRatio;
				s.MaxAmount *= changeRatio;
				s.Remaining *= changeRatio;
			});
			await Task.WhenAll(budgets.Select(b => SaveBudgetAsync(b)));
		}

		private async Task UpdateAllSubscriptions(decimal changeRatio)
		{
			var subs = await GetSubscriptionsAsync();
			subs.ForEach(s => s.Value *= changeRatio);
			await Task.WhenAll(subs.Select(s => SaveSubscriptionAsync(s)));
		}
	}
}
