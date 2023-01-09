using App.Data;
using App.Helpers;
using App.Resx;
using System;
using System.Resources;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App
{
	public partial class App : Application
	{
		private readonly AppDatabase _database;

		private readonly StatisticsHolder _statistics;

		private readonly SettingsManager _settings;

		private readonly CurrenciesManager _currencies;

		public static readonly ResourceManager ResourceManager = new ResourceManager(typeof(AppResource));

		public App()
		{
			SetUpExceptionHandling();

			InitializeComponent();

			_database = new AppDatabase();
			_statistics = new StatisticsHolder();
			_settings = new SettingsManager();

			DependencyService.RegisterSingleton(_database);
			DependencyService.RegisterSingleton(_statistics);
			DependencyService.RegisterSingleton(_settings);

			// Currencies needs _settings service
			_currencies = new CurrenciesManager();
			DependencyService.RegisterSingleton(_currencies);

			ThemeManager.Settings = _settings;
			ThemeManager.LoadTheme();

			MainPage = new AppShell();
		}

		protected override void OnStart()
		{
			try
			{
				SubscriptionHelper.ValidateSubscriptions();
				StatisticsHelper.CheckStatisticsForReset();
				BudgetHelper.ValidateBudgets();
			}
			catch(Exception ex)
			{
				NotifyException(ex);
			}
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}

		private void SetUpExceptionHandling()
		{
			AppDomain appDomain = AppDomain.CurrentDomain;
			appDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhadledException);
			TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedException;
		}

		private void OnUnhadledException(object	sender, UnhandledExceptionEventArgs args)
			=> NotifyException((Exception) args.ExceptionObject);

		private void OnTaskSchedulerUnobservedException(object sender, UnobservedTaskExceptionEventArgs args)
			=> NotifyException(args.Exception);

		public static void NotifyException(Exception ex)
		{
			var exceptionMessage = $"Thrown by: {ex.TargetSite.Name}\n\rMessage: {ex.Message}";
			NotificationHelper.SendNotification(AppResource.Error, exceptionMessage);
		}
	}
}
