namespace App
{
	public class Constants
	{
		// Local resources
		public const string DbPath = "database.db";

		public const string StatsPath = "stats.json";

		public const string SettingsPath = "settings.json";

		public const string CurrenciesCachePath = "currencies.json";
		
		public const string LogPath = "debug-sa.log";

		// Online resources
		public const string BaseCurrenciesUrl = "https://api.frankfurter.app/";

		public const string CreditsUrl = "https://github.com/ferrariofilippo/SaveApp";

		public const string ReportBugUrl = "https://github.com/ferrariofilippo/SaveApp/issues/new";

		// UI Constants
		public const int ListViewCellHeight = 65;

		public const string TrasnparentHex = "#00FFFFFF";

		// CSV files headers
		public const string MovementsFileHeader = "ID,VALUE,IS_EXPENSE,DESCRIPTION,TYPE,DATE";

		public const string SubscriptionsFileHeader = "ID,VALUE,DESCRIPTION,EXPENSE_TYPE,RENEWAL_TYPE,LAST_PAID,NEXT_RENEWAL,CREATION_DATE";
	}
}
