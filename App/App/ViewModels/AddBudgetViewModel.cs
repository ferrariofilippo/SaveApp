using App.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Linq;

namespace App.ViewModels
{
	public sealed class AddBudgetViewModel : ObservableObject
	{
		public readonly string[] MovementTypes = Enum.GetValues(typeof(ExpenseType))
			.Cast<ExpenseType>()
			.Select(x => App.ResourceManager.GetString(x.ToString()))
			.ToArray();

		public readonly string[] Currencies = Enum.GetValues(typeof(Currencies))
			.Cast<Currencies>()
			.Select(x => x.ToString())
			.ToArray();

		private DateTime _initialDate;
		public DateTime InitialDate
		{
			get => _initialDate;
			set
			{
				if (SetProperty(ref _initialDate, value))
					OnPropertyChanged(nameof(MinimumFinalDate));
			}
		}

		public DateTime MinimumFinalDate => InitialDate;
	}
}
