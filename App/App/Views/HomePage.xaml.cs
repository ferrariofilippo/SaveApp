using App.ViewModels;
using SkiaSharp;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
		private struct CircleData
		{
			public float Center;
			public float Radius;
			public float TopLeftOffset;
			public float BottomRightOffset;
		}

		private const float _expensesStartAngle = 50.0f;

		private const float _backgroundStartAngle = 50.0f;

		private const float _backgroundAngle = 80.0f;

		private readonly HomeViewModel _viewModel = new HomeViewModel();
		
		private readonly SKPaint _incomePaint = new SKPaint()
		{
			IsAntialias = true,
			Color = SKColor.Parse(((Color)Application.Current.Resources["IncomeColor"]).ToHex()),
			Style = SKPaintStyle.Stroke,
			StrokeWidth = 20.0f
		};
		private readonly SKPaint _expensesPaint = new SKPaint()
		{
			TextSize = 50.0f,
			IsAntialias = true,
			Color = SKColor.Parse(((Color)Application.Current.Resources["ExpenseColor"]).ToHex()),
			Style = SKPaintStyle.Stroke,
			StrokeWidth = 20.0f
		};
		private readonly SKPaint _backgroundPaint = new SKPaint()
		{
			IsAntialias = true,
			Color = SKColor.Parse(((Color)Application.Current.Resources["BackgroundMainColor"]).ToHex()),
			Style = SKPaintStyle.Stroke,
			StrokeWidth = 21.0f,
		};

		private CircleData _circleData = new CircleData();

		public HomePage()
		{
			InitializeComponent();
			this.BindingContext = _viewModel;
			this.SizeChanged += HomePage_SizeChanged;
			_viewModel.Stats.PropertyChanged += UpdateGraph;
		}

		private void HomePage_SizeChanged(object _, EventArgs e)
		{
			double pageWidth = this.Width > 480 ? 450.0d : this.Width - 30.0d;

			MovementsCanvas.HeightRequest = pageWidth;
			MovementsCanvas.WidthRequest = pageWidth;

			_circleData.Center = (float)pageWidth / 2.0f;
			_circleData.Radius = _circleData.Center * 0.9f;
			_circleData.TopLeftOffset = _circleData.Center * 0.1f;
			_circleData.BottomRightOffset = (float)pageWidth - _circleData.TopLeftOffset;
			UpdateGraph("");
		}

		private void MovementsCanvas_PaintSurface(object _, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
		{
			var netWorth = _viewModel.Expenses + _viewModel.Income;
			var expensesAngle = -140.0f;
			expensesAngle -= netWorth == 0.00m ? 0.00f : (float)(280.0m * ((_viewModel.Expenses / netWorth) - 0.5m));

			var canvas = e.Surface.Canvas;
			var rect = new SKRect(
				_circleData.TopLeftOffset,
				_circleData.TopLeftOffset,
				_circleData.BottomRightOffset,
				_circleData.BottomRightOffset);

			canvas.Clear();
			canvas.DrawCircle(
				_circleData.Center,
				_circleData.Center,
				_circleData.Radius, 
				_incomePaint);

			using (var path = new SKPath())
			{
				path.AddArc(rect, _expensesStartAngle, expensesAngle);
				canvas.DrawPath(path, _expensesPaint);
			}
			using (var path = new SKPath())
			{
				path.AddArc(rect, _backgroundStartAngle, _backgroundAngle);
				canvas.DrawPath(path, _backgroundPaint);
			}
		}

		private async void OpenAdd_Clicked(object _, EventArgs e)
			=> await Navigation.PushAsync(new AddPage());

		private void RefreshView_Refreshing(object _, EventArgs e)
			=> UpdateGraph("");

		private void UpdateGraph(string name)
		{
			_viewModel.UpdateData();
			MovementsCanvas.InvalidateSurface();
            _viewModel.IsRefreshing = false;
        }
    }
}