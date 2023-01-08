namespace App
{
	public partial class AppShell : Xamarin.Forms.Shell
	{
		public AppShell()
		{
			InitializeComponent();
			MainTabBar.CurrentItem = HomeTab;
		}
	}
}
