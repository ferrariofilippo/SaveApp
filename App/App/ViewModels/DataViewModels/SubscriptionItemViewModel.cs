using App.Extensions;
using App.Models;
using App.Resx;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace App.ViewModels.DataViewModels
{
    public class SubscriptionItemViewModel : ObservableObject
    {
        private Color _typeColor;
        public Color TypeColor
        {
            get => _typeColor;
            set => SetProperty(ref _typeColor, value);
        }

        public string DescriptionString => Subscription.Description.Length > Constants.STRING_LENGTH
            ? Subscription.Description.Substring(0, Constants.STRING_LENGTH)
            : Subscription.Description.PadRight(Constants.STRING_LENGTH);

        public string ValueString => Subscription.Value.ToCurrencyString();

        public string NextRenewalString => Subscription.NextRenewal.ToString("dd/MM/yyyy");

        private Subscription _subscription;
        public Subscription Subscription
        {
            get => _subscription;
            set
            {
                if (SetProperty(ref _subscription, value))
                {
                    OnPropertyChanged(nameof(DescriptionString));
                    OnPropertyChanged(nameof(ValueString));
                    OnPropertyChanged(nameof(NextRenewalString));
                }
            }
        }
        public SubscriptionItemViewModel(Subscription sub)
        {
            Subscription = sub;
            TypeColor = ReadOnlies.MovementTypeColors[(int)sub.ExpenseType];
        }

		public override string ToString()
		{
			var currentCulture = CultureInfo.CurrentCulture;
			var message = new StringBuilder();
			message.AppendLine($"{AppResource.Description}: {Subscription.Description,30}");
			message.AppendLine($"{AppResource.Value}: {ValueString,30}");
			message.AppendLine($"{AppResource.ExpenseType}: {App.ResourceManager.GetString(Subscription.ExpenseType.ToString(), currentCulture),30}");
			message.AppendLine($"{AppResource.RenewalType}: {App.ResourceManager.GetString(Subscription.RenewalType.ToString(), currentCulture),30}");
			message.AppendLine($"{AppResource.NextRenewal}: {NextRenewalString,30}");
			message.Append($"{AppResource.LastRenewal}: {Subscription.LastPaid,30:dd/MM/yyyy}");
            return message.ToString();
		}
	}
}
