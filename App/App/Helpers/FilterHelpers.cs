using App.Models;
using App.Models.Enums;
using System;

namespace App.Helpers
{
    public static class FilterHelpers
    {
		public static int GetStartingIndex(Movement[] items, DateTime toFind, SearchDepth depth)
		{
			var index = -1;

			switch (depth)
			{
				case SearchDepth.Year:
					index = BinarySearchDate(items, toFind.Year, 0, items.Length);
					break;
				case SearchDepth.Month:
					index = BinarySearchDate(items, toFind.Year, toFind.Month, 0, items.Length);
					break;
				case SearchDepth.Day:
					index = BinarySearchDate(items, toFind, 0, items.Length);
					break;
			}

			while (index >= 0 && items[index].CreationDate.Date >= toFind.Date)
				index--;

			return index > 0
				? ++index
				: items[0].CreationDate.Date >= toFind.Date
					? 0
					: -1;
		}

		private static int BinarySearchDate(Movement[] items, int year, int low, int high)
		{
			if (low >= high)
				return -1;
			var mid = (low + high - 1) / 2;
			if (items[mid].CreationDate.Year == year)
				return mid;
			if (year < items[mid].CreationDate.Year)
				return BinarySearchDate(items, year, low, mid - 1);
			return BinarySearchDate(items, year, mid + 1, high);
		}

		private static int BinarySearchDate(Movement[] items, int year, int month, int low, int high)
		{
			if (low >= high)
				return -1;
			var mid = (low + high - 1) / 2;
			if (items[mid].CreationDate.Year == year && items[mid].CreationDate.Month == month)
				return mid;
			if (year < items[mid].CreationDate.Year || month < items[mid].CreationDate.Month)
				return BinarySearchDate(items, year, month, low, mid - 1);
			return BinarySearchDate(items, year, month, mid + 1, high);
		}

		private static int BinarySearchDate(Movement[] items, DateTime date, int low, int high)
		{
			if (low >= high)
				return -1;
			var mid = (low + high - 1) / 2;
			if (items[mid].CreationDate.Date == date.Date)
				return mid;
			if (date.Date < items[mid].CreationDate.Date)
				return BinarySearchDate(items, date, low, mid - 1);
			return BinarySearchDate(items, date, mid + 1, high);
		}
	}
}
