using App.Extensions;
using App.Models.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace App.Models
{
	public class StatisticsDisplay
	{
		private static readonly SKColor _transparent = SKColor.Parse(Constants.TrasnparentHex);

		public string Title { get; set; }

		public Chart StatChart { get; set; }

		public ObservableCollection<StatisticItem> Items { get; } = new ObservableCollection<StatisticItem>();

		public double ChartSize => App.Current.MainPage.Width - 30;

		public bool ShowEmptyLabel { get; set; } = false;

		public StatisticsDisplay(string title, List<decimal> values, decimal isExpense = -1.0m, bool isMonthly = false)
		{
			Title = title;

			var sum = values.Sum();
			ShowEmptyLabel = sum == 0.0m;
			var entries = CreateEntries(values, isExpense, sum, isMonthly);

			StatChart = new DonutChart()
			{
				AnimationDuration = TimeSpan.FromSeconds(2),
				BackgroundColor = _transparent,
				Entries = entries,
				HoleRadius = 0.8f,
				LabelMode = LabelMode.None
			};
		}

		public StatisticsDisplay(string title, Dictionary<int, decimal> values, decimal isExpense = -1.0m)
		{
			Title = title;
			var entries = CreateEntries(values, isExpense);
			
			ShowEmptyLabel = entries.Length == 0;

			StatChart = new PointChart()
			{
				AnimationDuration = TimeSpan.FromSeconds(2),
				BackgroundColor = _transparent,
				Entries = entries,
				LabelColor = SKColor.Parse(((Color)App.Current.Resources["ForegroundMainColor"]).ToHex()),
				LabelOrientation = Orientation.Vertical,
				PointSize = 30.0f,
				PointMode = PointMode.Circle
			};
		}

		public void UpdateGraph(List<decimal> values, decimal isExpense = -1.0m, bool isMonthly = false)
		{
			var sum = values.Sum();
			ShowEmptyLabel = sum == 0.0m;
			StatChart.Entries = CreateEntries(values, isExpense, sum, isMonthly);
		}

		public void UpdateGraph(Dictionary<int, decimal> values, decimal isExpense = -1.0m)
		{
			ShowEmptyLabel = values.Keys.Count == 0;
			StatChart.Entries = CreateEntries(values, isExpense);
		}

		private ChartEntry[] CreateEntries(List<decimal> values, decimal isExpense, decimal sum, bool isMonthly)
		{
			Items.Clear();
			byte[] rgb = new byte[3];
			var entries = new ChartEntry[values.Count];
			decimal total = sum / 100.0m;

			if (total == 0)
				return Array.Empty<ChartEntry>();

			for (int i = 0; i < values.Count; i++)
			{
				var color = Constants.MovementTypeColors[i];
				rgb[0] = (byte)(color.R * 255);
				rgb[1] = (byte)(color.G * 255);
				rgb[2] = (byte)(color.B * 255);

				entries[i] = new ChartEntry((float)values[i])
				{
					Color = new SKColor(rgb[0], rgb[1], rgb[2])
				};

				Items.Add(new StatisticItem()
				{
					Name = isMonthly 
						? App.ResourceManager.GetString(Constants.Months[i])
						: App.ResourceManager.GetString(((ExpenseType)i).ToString()),
					Percentage = $"{(values[i] / total):0.00}%",
					TypeColor = color,
					ValueString = (values[i] * isExpense).ToCurrencyString()
				});
			}
			return entries;
		}

		private ChartEntry[] CreateEntries(Dictionary<int, decimal> values, decimal isExpense)
		{
			Items.Clear();
			if (values.Values.Count == 0)
				return Array.Empty<ChartEntry>();

			var entries = new ChartEntry[values.Count];
			decimal avg = values.Values.Sum() / values.Values.Count;
			var keys = values.Keys.OrderByDescending(x => x).ToArray();

			var expenseColor = (Color)Application.Current.Resources["ExpenseColor"];
			var incomeColor = (Color)Application.Current.Resources["IncomeColor"];

			for (int i = 0; i < keys.Length; i++)
			{
				var value = values[keys[i]];
				var color = value >= avg ? expenseColor : incomeColor;

				entries[i] = new ChartEntry((float)value)
				{
					Label = keys[i].ToString(),
					Color = SKColor.Parse(color.ToHex())
				};

				Items.Add(new StatisticItem()
				{
					Name = keys[i].ToString(),
					Percentage = string.Empty,
					TypeColor = color,
					ValueString = (value * isExpense).ToCurrencyString()
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
