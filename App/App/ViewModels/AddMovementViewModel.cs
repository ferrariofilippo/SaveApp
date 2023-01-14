using App.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Globalization;
using System.Linq;

namespace App.ViewModels
{
	public class AddMovementViewModel : ObservableObject
	{
		private static CultureInfo culture => CultureInfo.CurrentCulture;

		public readonly string[] MovementTypes = Enum.GetValues(typeof(ExpenseType))
			.Cast<ExpenseType>()
			.Select(x => App.ResourceManager.GetString(x.ToString(), culture))
			.ToArray();

		public readonly string[] RenewalTypes = Enum.GetValues(typeof(RenewalType))
			.Cast<RenewalType>()
			.Select(x => App.ResourceManager.GetString(x.ToString(), culture))
			.ToArray();

		public readonly string[] Currencies = Enum.GetValues(typeof(Currencies))
			.Cast<Currencies>()
			.Select(x => x.ToString())
			.ToArray();

		public readonly DateTime MaxDate = DateTime.Now.AddMonths(1);

		public readonly DateTime MinDate = DateTime.Now.AddYears(-5);

		private bool _isSubscription;
		public bool IsSubscription
		{
			get => _isSubscription;
			set => SetProperty(ref _isSubscription, value);
		}

		private bool _isExpense = true;
		public bool IsExpense
		{
			get => _isExpense;
			set => SetProperty(ref _isExpense, value);
		}

		private bool _showExpenseTypePicker = true;
		public bool ShowExpenseTypePicker
		{
			get => _showExpenseTypePicker;
			set => SetProperty(ref _showExpenseTypePicker, value);
		}
	}
}
