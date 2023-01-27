using App.Extensions;
using App.Models;
using App.Resx;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace App.ViewModels.DataViewModels
{
    public class MovementItemViewModel : ObservableObject
    {
        private Color _backgroundColor;
        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        public string ValueString => (Movement.IsExpense ? -Movement.Value : Movement.Value).ToCurrencyString();

        public string DateString => Movement.CreationDate.ToString("dd/MM/yyyy");

        public string DescriptionString => Movement.Description.Length > Constants.STRING_LENGTH
            ? Movement.Description.Substring(0, Constants.STRING_LENGTH)
            : Movement.Description.PadRight(Constants.STRING_LENGTH);

        private Movement _movement;
        public Movement Movement
        {
            get => _movement;
            set
            {
                if (SetProperty(ref _movement, value))
                {
                    OnPropertyChanged(nameof(ValueString));
                    OnPropertyChanged(nameof(DateString));
                    OnPropertyChanged(nameof(DescriptionString));
                }
            }
        }

        public MovementItemViewModel(Movement m)
        {
            Movement = m;
            BackgroundColor = m.IsExpense 
                ? ReadOnlies.MovementTypeColors[(int)m.ExpenseType] 
                : (Color)Application.Current.Resources["IncomeColor"];
        }

		public override string ToString()
		{
			var message = new StringBuilder();
			message.AppendLine($"{AppResource.Description}: {Movement.Description,30}");
			message.AppendLine($"{AppResource.Value}: {ValueString,30}");
			message.AppendLine($"{AppResource.Date}: {DateString,30}");
			message.Append($"{AppResource.ExpenseType}: {App.ResourceManager.GetString(Movement.ExpenseType.ToString()),30}");
            return message.ToString();
		}
	}
}
