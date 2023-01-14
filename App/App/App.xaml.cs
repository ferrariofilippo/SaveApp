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
		private readonly IAppDatabase _database;

		private readonly StatisticsManager _statistics;

		private readonly ISettingsManager _settings;

		private readonly ICurrenciesManager _currencies;

		private readonly ILogger _logger;

		public static readonly ResourceManager ResourceManager = new ResourceManager(typeof(AppResource));

		public App()
		{
			SetUpExceptionHandling();

			InitializeComponent();

			_database = new AppDatabase();
			_statistics = new StatisticsManager();
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
