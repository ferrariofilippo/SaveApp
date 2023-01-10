using App.Extensions;
using App.Models;
using CommunityToolkit.Mvvm.ComponentModel;
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

        public string DescriptionString => Subscription.Description.Length > 20
            ? Subscription.Description.Substring(0, 20)
            : Subscription.Description.PadRight(20);

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
    }
}
