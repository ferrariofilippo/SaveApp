using App.Extensions;
using App.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace App.ViewModels.DataViewModels
{
    public class StatisticsItemViewModel
    {
        private static readonly SKColor _transparent = SKColor.Parse(Constants.TrasnparentHex);

        public string Title { get; set; }

        public Chart StatChart { get; set; }

        public ObservableCollection<StatisticItem> Items { get; } = new ObservableCollection<StatisticItem>();

        public double ChartSize => Application.Current.MainPage.Width - 30;

        public bool ShowEmptyLabel { get; set; }

        public StatisticsItemViewModel(string title, List<decimal> values, bool isExpense = true, bool isMonthly = false)
        {
            Title = title;
            var entries = CreateEntries(values, isExpense, isMonthly);

            ShowEmptyLabel = entries.Length == 0;

            StatChart = new DonutChart()
            {
                AnimationDuration = TimeSpan.FromSeconds(2),
                BackgroundColor = _transparent,
                Entries = entries,
                HoleRadius = 0.8f,
                LabelMode = LabelMode.None
            };
        }

        public StatisticsItemViewModel(string title, Dictionary<int, decimal> values, bool isExpense = true)
        {
            Title = title;
            var entries = CreateEntries(values, isExpense);

            ShowEmptyLabel = entries.Length == 0;

            StatChart = new PointChart()
            {
                AnimationDuration = TimeSpan.FromSeconds(2),
                BackgroundColor = _transparent,
                Entries = entries,
                LabelColor = SKColor.Parse(((Color)Application.Current.Resources["ForegroundMainColor"]).ToHex()),
                LabelOrientation = Orientation.Vertical,
                PointSize = 30.0f,
                PointMode = PointMode.Circle
            };
        }

        public void UpdateGraph(List<decimal> values, bool isExpense = true, bool isMonthly = false)
        { 
            ShowEmptyLabel = values is null || values.Count == 0;
            if (ShowEmptyLabel)
                return;
            StatChart.Entries = CreateEntries(values, isExpense, isMonthly);
        }

        public void UpdateGraph(Dictionary<int, decimal> values, bool isExpense = true)
        {
            ShowEmptyLabel = values is null || values.Keys.Count == 0;
            if (ShowEmptyLabel)
                return;
            StatChart.Entries = CreateEntries(values, isExpense);
        }

        private ChartEntry[] CreateEntries(List<decimal> values, bool isExpense = true, bool isMonthly = false)
        {
            Items.Clear();

            var culture = CultureInfo.CurrentCulture;
            var sum = values.Sum();
            var entries = new ChartEntry[values.Count];
            var total = sum / 100.0m;
            var sign = isExpense ? -1.0m : 1.0m;

            Color xamarinColor;
            SKColor skColor;

            for (int i = 0; i < values.Count; i++)
            {
                xamarinColor = ReadOnlies.MovementTypeColors[i];
                skColor = ReadOnlies.SKMovementTypeColors[i];

                entries[i] = new ChartEntry((float)values[i])
                {
                    Color = skColor
                };

                Items.Add(new StatisticItem()
                {
                    Name = isMonthly
                        ? App.ResourceManager.GetString(ReadOnlies.Months[i], culture)
                        : App.ResourceManager.GetString(((ExpenseType)i).ToString(), culture),
                    Percentage = $"{values[i] / total:0.00}%",
                    TypeColor = xamarinColor,
                    ValueString = (values[i] * sign).ToCurrencyString()
                });
            }
            return entries;
        }

        private ChartEntry[] CreateEntries(Dictionary<int, decimal> values, bool isExpense)
        {
            Items.Clear();
            var valuesCount = values.Values.Count;

            var entries = new ChartEntry[valuesCount];
            var avg = values.Values.Sum() / valuesCount;
            var keys = values.Keys.OrderByDescending(x => x).ToArray();
            var sign = isExpense ? -1.0m : 1.0m;

            var negativeXamarinColor = (Color)Application.Current.Resources["ExpenseColor"];
            var negativeSKColor = SKColor.Parse(negativeXamarinColor.ToHex());
            var positiveXamarinColor = (Color)Application.Current.Resources["IncomeColor"];
            var positiveSKColor = SKColor.Parse(positiveXamarinColor.ToHex());

            var negativeColors = (negativeSKColor, negativeXamarinColor);
            var positiveColors = (positiveSKColor, positiveXamarinColor);

            for (int i = 0; i < keys.Length; i++)
            {
                var value = values[keys[i]];
                var colors = value > avg ? negativeColors : positiveColors;

                entries[i] = new ChartEntry((float)value)
                {
                    Label = keys[i].ToString(),
                    Color = colors.Item1
                };

                Items.Add(new StatisticItem()
                {
                    Name = keys[i].ToString(),
                    Percentage = string.Empty,
                    TypeColor = colors.Item2,
                    ValueString = (value * sign).ToCurrencyString()
                });
            }
            return entries;
        }
    }

    public class StatisticItem : ObservableObject
    {
        private Color _typeColor;
        public Color TypeColor
        {
            get => _typeColor;
            set => SetProperty(ref _typeColor, value);
        }

        private string _valueString;
        public string ValueString
        {
            get => _valueString;
            set => SetProperty(ref _valueString, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _percentage;
        public string Percentage
        {
            get => _percentage;
            set => SetProperty(ref _percentage, value);
        }
    }
}
