namespace App
{
	public static class Constants
	{
		// Local resources
		public const string DB_PATH = "database.db";

		public const string STATS_PATH = "stats.json";

		public const string SETTINGS_PATH = "settings.json";

		public const string CURRENCIES_CACHE_PATH = "currencies.json";
		
		public const string LOG_PATH = "debug-sa.log";

		// Online resources
		public const string BASE_CURRENCIES_URL = "https://api.frankfurter.app/";

		public const string CREDITS_URL = "https://github.com/ferrariofilippo/SaveApp";

		public const string REPORT_BUG_URL = "https://github.com/ferrariofilippo/SaveApp/issues/new";

		// UI Constants
		public const int LIST_VIEW_CELL_HEIGHT = 65;

		public const string TRANSPARENT = "#00FFFFFF";

		public const int STRING_LENGTH = 20;

        // CSV files headers
        public const string MOVEMENTS_FILE_HEADER = "ID;VALUE;IS_EXPENSE;DESCRIPTION;TYPE;DATE";

		public const string SUBSCRIPTIONS_FILE_HEADER = "ID;VALUE;DESCRIPTION;EXPENSE_TYPE;RENEWAL_TYPE;LAST_PAID;NEXT_RENEWAL;CREATION_DATE";

		// Time Constants
		public const int WEEKS_IN_YEAR = 52;

		public const int MONTHS_IN_YEAR = 12;

		public const int SIXTHS_IN_YEAR = 6;

        public const int QUARTERS_IN_YEAR = 4;

        public const int SEMESTERS_IN_YEAR = 2;

		public const int DAYS_IN_WEEK = 7;

        public const int DAYS_IN_YEAR = 365;

        public const int DAYS_IN_LEAP_YEAR = 366;

		public const int DAYS_IN_SEMESTER = 181;

		public const int DAYS_IN_LEAP_SEMESTER = 182;
    }
}
