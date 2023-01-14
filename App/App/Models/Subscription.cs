using App.Models.Enums;
using SQLite;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
	public class Subscription
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(13,2)")]
		public decimal Value { get; set; } = 0.0m;

		[System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "varchar(256)")]
		public string Description { get; set; } = string.Empty;
		
		public ExpenseType ExpenseType { get; set; } = ExpenseType.Others;
		
		public RenewalType RenewalType { get; set; } = RenewalType.Yearly;
		
		public DateTime LastPaid { get; set; }
		
		public DateTime NextRenewal { get; set; }
		
		public DateTime CreationDate { get; set; }

		[ForeignKey(name: "BudgetId")]
		public int BudgetId { get; set; } = 0;

		public Subscription() { }

		public Subscription(RenewalType type, DateTime last)
		{
			RenewalType = type;
			CreationDate = DateTime.Now;
			LastPaid = last;
		}

		public void UpdateNextRenewal()
		{
			var isLeapYear = DateTime.IsLeapYear(LastPaid.Year);
            var renewalOffset = isLeapYear ? 366 : 365;
			
			switch (RenewalType)
			{
				case RenewalType.Weekly:
					renewalOffset = 7;
					break;
				case RenewalType.Monthly:
					renewalOffset = DateTime.DaysInMonth(LastPaid.Year, LastPaid.Month);
					break;
				case RenewalType.Bimonthly:
					var nextMonth = LastPaid.AddMonths(1);

					renewalOffset =
						DateTime.DaysInMonth(LastPaid.Year, LastPaid.Month) +
						DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
					break;
				case RenewalType.Quarterly:
					var secondMonth = LastPaid.AddMonths(1);
					var thirdMonth = secondMonth.AddMonths(1);

					renewalOffset =
						DateTime.DaysInMonth(LastPaid.Year, LastPaid.Month) +
						DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month) +
						DateTime.DaysInMonth(thirdMonth.Year, thirdMonth.Month);
					break;
				case RenewalType.Semiannual:
					renewalOffset = isLeapYear ? 182 : 181;
					break;
			}

			NextRenewal = LastPaid.AddDays(renewalOffset);
		}
	}
}
