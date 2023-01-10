using App.Data;
using App.Helpers;
using App.Helpers.LogHelper;
using App.Helpers.Notifications;
using App.Helpers.Themes;
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

		private readonly Logger _logger;

		public static readonly ResourceManager ResourceManager = new ResourceManager(typeof(AppResource));

		public App()
		{
			SetUpExceptionHandling();

			InitializeComponent();

			_database = new AppDatabase();
			_statistics = new StatisticsHolder();
			_settings = new SettingsManager();
			_logger = new Logger();

			DependencyService.RegisterSingleton(_database);
			DependencyService.RegisterSingleton(_statistics);
			DependencyService.RegisterSingleton(_settings);
			DependencyService.RegisterSingleton(_logger);

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
				NotificationHelper.NotifyException(ex);
			}
		}

		private void SetUpExceptionHandling()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnUnhadledException);
			TaskScheduler.UnobservedTaskException += OnTaskSchedulerUnobservedException;
		}

		private void OnUnhadledException(object	sender, UnhandledExceptionEventArgs args)
			=> NotificationHelper.NotifyException((Exception) args.ExceptionObject);

		private void OnTaskSchedulerUnobservedException(object sender, UnobservedTaskExceptionEventArgs args)
			=> NotificationHelper.NotifyException(args.Exception);
	}
}
