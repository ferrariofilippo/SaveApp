using SkiaSharp;
using Xamarin.Forms;

namespace App.ViewModels.DataViewModels
{
    public class HomeGraphViewModel
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

		private CircleData _circle;

		public void UpdateGraphSize(float diameter)
		{
			_circle.Center = diameter / 2.0f;
			_circle.Radius = _circle.Center * 0.9f;
			_circle.TopLeftOffset = _circle.Center * 0.1f;
			_circle.BottomRightOffset = diameter - _circle.TopLeftOffset;
		}

		public void Draw(decimal expenses, decimal netWorth, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
		{
			var expensesAngle = -140.0f;
			expensesAngle -= netWorth == 0.00m ? 0.00f : (float)(280.0m * ((expenses / netWorth) - 0.5m));

			var canvas = e.Surface.Canvas;
			var rect = new SKRect(
				_circle.TopLeftOffset,
				_circle.TopLeftOffset,
				_circle.BottomRightOffset,
				_circle.BottomRightOffset);

			canvas.Clear();
			canvas.DrawCircle(
				_circle.Center,
				_circle.Center,
				_circle.Radius,
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
	}
}
