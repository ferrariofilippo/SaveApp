﻿using App.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Xamarin.Forms;

namespace App.Models
{
    public class BudgetDisplay : ObservableObject
    {
        private Color _typeColor;
        public Color TypeColor
        {
            get => _typeColor;
            set => SetProperty(ref _typeColor, value);
        }

        public string RemainingString => Budget.Remaining.ToCurrencyString();

        public string UsedString => (-Budget.Used).ToCurrencyString();

        public string InitialString => Budget.CreationDate.ToString("dd/MM/yyyy");

        public string EndingString => Budget.EndingDate.ToString("dd/MM/yyyy");

        public float UsedPercent => (float)(Budget.Used / Budget.MaxAmount);

        public string DescriptionString => Budget.Name.Length > 20 
            ? Budget.Name.Substring(0, 20) 
            : Budget.Name.PadRight(20);

        private Budget _budget;
        public Budget Budget
        {
            get => _budget;
            set
            {
                if (SetProperty(ref _budget, value))
                {
                    OnPropertyChanged(nameof(RemainingString));
					OnPropertyChanged(nameof(UsedString));
					OnPropertyChanged(nameof(InitialString));
					OnPropertyChanged(nameof(EndingString));
					OnPropertyChanged(nameof(DescriptionString));
                    OnPropertyChanged(nameof(UsedPercent));
				}
			}
        }

        public BudgetDisplay(Budget budget)
        {
            Budget = budget;
            TypeColor = Constants.MovementTypeColors[(int)budget.MovementType];
        }
    }
}
