using App.Extensions;
using App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
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

        public string DescriptionString => Movement.Description.Length > 20
            ? Movement.Description.Substring(0, 20)
            : Movement.Description.PadRight(20);

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
    }
}
