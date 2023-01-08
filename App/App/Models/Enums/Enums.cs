namespace App.Models.Enums
{
	public enum ExpenseType : byte
	{
		Bets = 0,
		Clothes = 1,
		Culture = 2,
		Entertainment = 3,
		Food = 4,
		Gifts = 5,
		Holydays = 6,
		PersonalCare = 7,
		Others = 8,
		Sport = 9,
		Tech = 10,
		Transports = 11
	}

	public enum RenewalType : byte
	{
		Weekly,
		Monthly,
		Bimonthly,
		Quarterly,
		Semiannual,
		Yearly
	}

	public enum Theme : byte
	{
		Light = 0,
		Nord = 1,
		Ice = 2,
		Teal = 3
	}

	public enum Currencies : byte
	{
		EUR = 0,
		USD = 1,
		AUD = 2,
		CAD = 3,
		GBP = 4,
		CHF = 5,
		JPY = 6,
		CNY = 7
	}
}
