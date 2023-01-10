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

        public async Task AddMovement(Movement movement, bool isSubscriptionPayment = false)
        {
            if (movement.IsExpense)
            {
                Statistics.TotalExpenses += movement.Value;

                if (Statistics.ExpensesByYear.ContainsKey(movement.CreationDate.Year))
                    Statistics.ExpensesByYear[movement.CreationDate.Year] += movement.Value;
                else
                    Statistics.ExpensesByYear[movement.CreationDate.Year] = movement.Value;

                if (movement.CreationDate.Year == DateTime.Now.Year)
                {
                    Statistics.ExpensesByType[(byte)movement.ExpenseType] += movement.Value;
                    Statistics.ExpensesByMonth[movement.CreationDate.Month - 1] += movement.Value;
                }

                if (isSubscriptionPayment)
                    Statistics.SubscriptionPaidYTD += movement.Value;
            }
            else
            {
                Statistics.TotalIncome += movement.Value;
                if (Statistics.IncomeByYear.ContainsKey(movement.CreationDate.Year))
                    Statistics.IncomeByYear[movement.CreationDate.Year] += movement.Value;
                else
                    Statistics.IncomeByYear[movement.CreationDate.Year] = movement.Value;
            }
            PropertyChanged?.Invoke(nameof(Statistics));
            await SaveStats();
        }

        public async Task RemoveMovement(Movement movement)
        {
            if (movement.IsExpense)
            {
                Statistics.TotalExpenses -= movement.Value;
                Statistics.ExpensesByYear[movement.CreationDate.Year] -= movement.Value;

                if (movement.CreationDate.Year == DateTime.Now.Year)
                {
                    Statistics.ExpensesByType[(byte)movement.ExpenseType] -= movement.Value;
                    Statistics.ExpensesByMonth[movement.CreationDate.Month - 1] -= movement.Value;
                }
            }
            else
            {
                Statistics.TotalIncome -= movement.Value;
                Statistics.IncomeByYear[movement.CreationDate.Year] -= movement.Value;
            }
            PropertyChanged?.Invoke(nameof(Statistics));
            await SaveStats();
        }

        public async Task AddSubscription(Subscription subscription)
        {
            Statistics.YearlySubscriptionExpense += subscription.Value * GetMultiplier(subscription.RenewalType);
            await SaveStats();
        }

        public async Task RemoveSubscription(Subscription subscription)
        {
            Statistics.YearlySubscriptionExpense -= subscription.Value * GetMultiplier(subscription.RenewalType);
            await SaveStats();
        }

        public async Task SaveStats()
        {
            using (var writer = new StreamWriter(_statsPath, false))
                await writer.WriteAsync(JsonSerializer.Serialize(Statistics));
        }

        public async Task EmptyPreviousYear()
        {
            for (int i = 0; i < Statistics.ExpensesByType.Count; i++)
            {
                Statistics.ExpensesByType[i] = 0.0m;
                Statistics.ExpensesByMonth[i] = 0.0m;
            }
            Statistics.SubscriptionPaidYTD = 0.0m;
            await SaveStats();
        }

        public async Task UpdateAllToNewCurrency(decimal changeRatio)
        {
            Statistics.TotalExpenses *= changeRatio;
            Statistics.TotalIncome *= changeRatio;
            Statistics.YearlySubscriptionExpense *= changeRatio;

            for (int i = 0; i < Statistics.ExpensesByType.Count; i++)
                Statistics.ExpensesByType[i] *= changeRatio;
            for (int i = 0; i < Statistics.ExpensesByMonth.Count; i++)
                Statistics.ExpensesByMonth[i] *= changeRatio;

            PropertyChanged?.Invoke(nameof(Statistics));
            await SaveStats();
        }

        private int GetMultiplier(RenewalType renwal)
        {
            switch (renwal)
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
