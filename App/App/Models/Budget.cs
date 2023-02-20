using App.Models.Enums;
using SQLite;
using System;

namespace App.Models
{
	public sealed class Budget
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		
		[System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(13,2)")]
		public decimal MaxAmount { get; set; } = 0.0m;
		
		[System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(13,2)")]
		public decimal Used { get; set; } = 0.0m;
		
		[System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(13,2)")]
		public decimal Remaining { get; set; } = 0.0m;

		[System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "varchar(32)")]
		public string Name { get; set; } = string.Empty;
		
		public DateTime CreationDate { get; set; }
		
		public DateTime EndingDate { get; set; }
		
		public ExpenseType MovementType { get; set; } = ExpenseType.Others;
	}
}
