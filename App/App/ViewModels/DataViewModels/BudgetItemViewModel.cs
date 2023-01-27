using App.Extensions;
using App.Models;
using App.Resx;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace App.ViewModels.DataViewModels
{
    public class BudgetItemViewModel : ObservableObject
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

        public string DescriptionString => Budget.Name.Length > Constants.STRING_LENGTH
            ? Budget.Name.Substring(0, Constants.STRING_LENGTH)
            : Budget.Name.PadRight(Constants.STRING_LENGTH);

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

        public BudgetItemViewModel(Budget budget)
        {
            Budget = budget;
            TypeColor = ReadOnlies.MovementTypeColors[(int)budget.MovementType];
        }

		public override string ToString()
		{
			var message = new StringBuilder();
			message.AppendLine($"{AppResource.Description}: {Budget.Name,30}");
			message.AppendLine($"{AppResource.Remaining}: {RemainingString,30}");
			message.AppendLine($"{AppResource.Used}: {UsedString,30}");
			message.AppendLine($"{AppResource.Total}: {Budget.MaxAmount.ToCurrencyString(),30}");
			message.AppendLine($"{AppResource.InitialDate}: {InitialString,30}");
			message.Append($"{AppResource.FinalDate}: {EndingString,30}");
            return message.ToString();
		}
	}
}
