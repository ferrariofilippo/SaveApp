using App.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Xamarin.Forms;

namespace App.Models
{
	public class MovementDisplay : ObservableObject
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

		public MovementDisplay(Movement m)
		{
			Movement = m;
			BackgroundColor = m.IsExpense ? Constants.MovementTypeColors[(int)m.ExpenseType] : (Color)App.Current.Resources["IncomeColor"];
		}
	}
}
