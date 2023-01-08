using App.Models.Enums;
using SQLite;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
	public class Movement
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(13,2)")]
		public decimal Value { get; set; } = 0.0m;
		
		[System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "varchar(64)")]
		public string Description { get; set; } = string.Empty;
		
		public bool IsExpense { get; set; } = true;
		
		public ExpenseType ExpenseType { get; set; } = ExpenseType.Others;

		public DateTime CreationDate { get; set; } = DateTime.Today;

		[ForeignKey(name:"BudgetId")]
		public int BudgetId { get; set; } = 0;
	}
}
