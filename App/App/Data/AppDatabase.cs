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
		readonly SQLiteAsyncConnection _database;

		public AppDatabase()
		{
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.DbPath);
			_database = new SQLiteAsyncConnection(path);
			_database.CreateTableAsync<Movement>().Wait();
			_database.CreateTableAsync<Subscription>().Wait();
			_database.CreateTableAsync<Budget>().Wait();
		}

		public Task<List<Movement>> GetMovementsAsync() => _database.Table<Movement>().ToListAsync();
		public Task<List<Movement>> GetMovementsAsync(DateTime starting, DateTime ending) 
			=> _database.Table<Movement>()
				.Where(m => m.CreationDate > starting && m.CreationDate < ending)
				.ToListAsync();
		public Task<List<Subscription>> GetSubscriptionsAsync() => _database.Table<Subscription>().ToListAsync();
		public Task<List<Budget>> GetBudgetsAsync() => _database.Table<Budget>().ToListAsync();
		public Task<Budget> GetBudgetAsync(int id) => _database.Table<Budget>().FirstOrDefaultAsync(m => m.Id == id);

		public Task<int> SaveMovementAsync(Movement m) => m.Id == 0 
			? _database.InsertAsync(m) 
			: _database.UpdateAsync(m);
		public Task<int> SaveSubscriptionAsync(Subscription s) => s.Id == 0
			? _database.InsertAsync(s)
			: _database.UpdateAsync(s); 
		public Task<int> SaveBudgetAsync(Budget b) => b.Id == 0
			? _database.InsertAsync(b)
			: _database.UpdateAsync(b);

		public Task<int> DeleteMovementAsync(Movement m) => _database.DeleteAsync(m);
		public Task<int> DeleteSubscriptionAsync(Subscription s) => _database.DeleteAsync(s);
		public Task<int> DeleteBudgetAsync(Budget b) => _database.DeleteAsync(b);

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
