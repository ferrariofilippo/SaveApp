using Xamarin.Forms;

namespace App
{
	public class Constants
	{
		public const string DbPath = "database.db";

		public const string StatsPath = "stats.json";

		public const string SettingsPath = "settings.json";

		public const string CurrenciesCachePath = "currencies.json";

		public const string BaseCurrenciesUrl = "https://api.frankfurter.app/";

		public const string CreditsUrl = "https://github.com/ferrariofilippo/SaveApp";

		public const string ReportBugUrl = "https://github.com/ferrariofilippo/SaveApp/issues/new";

		public const int ListViewCellHeight = 65;

		public const string TrasnparentHex = "#00FFFFFF";

		public static readonly string[] Months = 
		{ 
			"January",
			"February",
			"March",
			"April",
			"May",
			"June",
			"July",
			"August",
			"September",
			"October",
			"November",
			"December"
		};

		public static readonly Color[] MovementTypeColors =
		{
			new Color(213.0 / 255, 166.0 / 255, 189.0 / 255),
			new Color(217.0 / 255, 234.0 / 255, 211.0 / 255),
			new Color(164.0 / 255, 194.0 / 255, 244.0 / 255),
			new Color(221.0 / 255, 126.0 / 255, 107.0 / 255),
			new Color(249.0 / 255, 219.0 / 255, 156.0 / 255),
			new Color(162.0 / 255, 196.0 / 255, 201.0 / 255),
			new Color(180.0 / 255, 167.0 / 255, 214.0 / 255),
			new Color(159.0 / 255, 197.0 / 255, 232.0 / 255),
			new Color(217.0 / 255, 217.0 / 255, 217.0 / 255),
			new Color(182.0 / 255, 215.0 / 255, 168.0 / 255),
			new Color(255.0 / 255, 217.0 / 255, 102.0 / 255),
			new Color(234.0 / 255, 153.0 / 255, 153.0 / 255)
		};
	}
}
