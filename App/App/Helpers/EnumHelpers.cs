using App.Models.Enums;

namespace App.Helpers
{
	public static class EnumHelpers
	{
		public static bool TryParseExpenseType(string value, out ExpenseType type)
		{
			value = value.ToLower();

			switch (value)
			{
				case "bets":
					type = ExpenseType.Bets;
					return true;
				case "clothes":
					type = ExpenseType.Clothes;
					return true;
				case "culture":
					type = ExpenseType.Culture;
					return true;
				case "entertainment":
					type = ExpenseType.Entertainment;
					return true;
				case "food":
					type = ExpenseType.Food;
					return true;
				case "gifts":
					type = ExpenseType.Gifts;
					return true;
				case "holydays":
					type = ExpenseType.Holydays;
					return true;
				case "personalcare":
					type = ExpenseType.PersonalCare;
					return true;
				case "others":
					type = ExpenseType.Others;
					return true;
				case "sport":
					type = ExpenseType.Sport;
					return true;
				case "tech":
					type = ExpenseType.Tech;
					return true;
				case "transports":
					type = ExpenseType.Transports;
					return true;
				default:
					type = ExpenseType.Bets;
					return false;
			}
		}
	}
}
