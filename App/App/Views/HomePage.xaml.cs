using App.ViewModels;
using App.ViewModels.DataViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage : ContentPage
	{
		private const int PAGE_MAX_WIDTH = 480;
		private const double PAGE_PADDING = 30.0d;

		private readonly HomeViewModel _viewModel = new HomeViewModel();

		private readonly HomeGraphViewModel _graphViewModel = new HomeGraphViewModel();

		public HomePage()
		{
			InitializeComponent();
			this.BindingContext = _viewModel;
			this.SizeChanged += HomePage_SizeChanged;
			_viewModel.Stats.PropertyChanged += UpdateGraph;
		}

		private void HomePage_SizeChanged(object _, EventArgs e)
		{
			var pageWidth = (this.Width > PAGE_MAX_WIDTH ? PAGE_MAX_WIDTH : this.Width) - PAGE_PADDING;

			MovementsCanvas.HeightRequest = pageWidth;
			MovementsCanvas.WidthRequest = pageWidth;
			_graphViewModel.UpdateGraphSize((float)pageWidth);
			
			UpdateGraph("");
		}

		private void MovementsCanvas_PaintSurface(object _, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
		{
			var netWorth = _viewModel.Expenses + _viewModel.Income;
			_graphViewModel.Draw(_viewModel.Expenses, netWorth, e);
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