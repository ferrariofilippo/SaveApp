using App.Models;
using App.Models.Enums;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Data
{
	public class StatisticsHolder
	{
		private readonly string _statsPath;

		public delegate void PropertyChangedEventHandler(string name);
		public event PropertyChangedEventHandler PropertyChanged;

		public Statistics Statistics { get; private set; } = new Statistics();

		public StatisticsHolder()
		{
			_statsPath = Path.Combine(
				Environment.GetFolderPath(
					Environment.SpecialFolder.LocalApplicationData),
				Constants.StatsPath);

			if (File.Exists(_statsPath))
			{ 
				var json = File.ReadAllText(_statsPath);
				if (!string.IsNullOrWhiteSpace(json))
					Statistics = JsonSerializer.Deserialize<Statistics>(json);
			}
		}

		public Task AddMovement(Movement m, bool isSubPayment = false)
		{
			return Task.Run(async () =>
			{
				if (m.IsExpense)
				{
					Statistics.TotalExpenses += m.Value;
					if (m.CreationDate.Year == DateTime.Now.Year)
					{
						Statistics.ExpensesByType[(byte)m.ExpenseType] += m.Value;
						Statistics.ExpensesByMonth[m.CreationDate.Month - 1] += m.Value;
					}

					if (Statistics.ExpensesByYear.ContainsKey(m.CreationDate.Year))
						Statistics.ExpensesByYear[m.CreationDate.Year] += m.Value;
					else
						Statistics.ExpensesByYear[m.CreationDate.Year] = m.Value;

					if (isSubPayment)
						Statistics.SubscriptionPaidYTD += m.Value;
				}
				else
				{
					Statistics.TotalIncome += m.Value;
					if (Statistics.IncomeByYear.ContainsKey(m.CreationDate.Year))
						Statistics.IncomeByYear[m.CreationDate.Year] += m.Value;
					else
						Statistics.IncomeByYear[m.CreationDate.Year] = m.Value;
				}
				PropertyChanged?.Invoke(nameof(Statistics));
				await SaveStats();
			});
		}

		public Task RemoveMovement(Movement m)
		{
			return Task.Run(async () =>
			{
				if (m.IsExpense)
				{
					Statistics.TotalExpenses -= m.Value;
					Statistics.ExpensesByYear[m.CreationDate.Year] -= m.Value;

					if (m.CreationDate.Year == DateTime.Now.Year)
					{
						Statistics.ExpensesByType[(byte)m.ExpenseType] -= m.Value;
						Statistics.ExpensesByMonth[(byte)m.CreationDate.Month - 1] -= m.Value;
					}
				}
				else
				{
					Statistics.TotalIncome -= m.Value;
					Statistics.IncomeByYear[m.CreationDate.Year] -= m.Value;
				}
				PropertyChanged?.Invoke(nameof(Statistics));
				await SaveStats();
			});
		}

		public Task AddSubscription(Subscription s)
		{
			return Task.Run(async () =>
			{
				Statistics.YearlySubscriptionExpense += s.Value * GetMultiplier(s.RenewalType);
				await SaveStats();
			});
		}

		public Task RemoveSubscription(Subscription s)
		{
			return Task.Run(async () =>
			{
				Statistics.YearlySubscriptionExpense -= s.Value * GetMultiplier(s.RenewalType);
				await SaveStats();
			});
		}

		public async Task SaveStats()
		{
			using (var writer = new StreamWriter(_statsPath, false))
				await writer.WriteAsync(JsonSerializer.Serialize(Statistics));
		}

		public Task EmptyPreviousYear()
		{
			return Task.Run(async () =>
			{
				for (int i = 0; i < Statistics.ExpensesByType.Count; i++)
				{
					Statistics.ExpensesByType[i] = 0.0m;
					Statistics.ExpensesByMonth[i] = 0.0m;
				}
				await SaveStats();
			});
		}

		public async Task UpdateAllToNewCurrency(decimal currencyRatio)
		{
			Statistics.TotalExpenses *= currencyRatio;
			Statistics.TotalIncome *= currencyRatio;
			Statistics.YearlySubscriptionExpense *= currencyRatio;
			
			for (int i = 0; i < Statistics.ExpensesByType.Count; i++)
			{
				Statistics.ExpensesByType[i] *= currencyRatio;
				Statistics.ExpensesByMonth[i] *= currencyRatio;
			}

			PropertyChanged?.Invoke(nameof(Statistics));
			await SaveStats();
		}

		private int GetMultiplier(RenewalType t)
		{
			switch (t)
			{
				case RenewalType.Weekly:
					return 52;
				case RenewalType.Monthly:
					return 12;
				case RenewalType.Bimonthly:
					return 6;
				case RenewalType.Quarterly:
					return 4;
				case RenewalType.Semiannual:
					return 2;
				default:
					return 1;
			}
		}
	}
}
