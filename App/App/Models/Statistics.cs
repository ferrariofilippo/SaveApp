using System.Collections.Generic;

namespace App.Models
{
	public class Statistics
	{
		public decimal TotalIncome { get; set; } = 0.0m;

		public decimal TotalExpenses { get; set; } = 0.0m;

		public decimal YearlySubscriptionExpense { get; set; } = 0.0m;

		public decimal SubscriptionPaidYTD { get; set; } = 0.0m;

		public List<decimal> ExpensesByType { get; set; } = new List<decimal>()
		{
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m
		};

		public List<decimal> ExpensesByMonth { get; set; } = new List<decimal>()
		{
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m
		};

		public Dictionary<int, decimal> ExpensesByYear { get; set; } = new Dictionary<int, decimal>();

		public Dictionary<int, decimal> IncomeByYear { get; set; } = new Dictionary<int, decimal>();
	}
}
