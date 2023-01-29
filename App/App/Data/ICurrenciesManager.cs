using App.Models.Enums;
using System;
using System.Threading.Tasks;

namespace App.Data
{
    /// <summary>
    /// An object that holds info about main currency and change-ratios
    /// </summary>
    public interface ICurrenciesManager
    {
        /// <summary>
        /// Last update of change-ratios
        /// </summary>
        DateTime LastUpdate { get; }

        /// <summary>
        /// Converts a value to its default-currency equivalent
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="from">Currency to convert from</param>
        /// <returns>Default-currency equivalent of <paramref name="value"/></returns>
        decimal ConvertCurrencyToDefault(decimal value, Currencies from);

		/// <summary>
		/// Asynchronously updates all instances saved on the db to the new default currency
		/// </summary>
		/// <param name="previous">Previous default-currency</param>
		/// <returns>A task representing the async operation</returns>
		Task UpdateAllToCurrent(Currencies previous);
	}
}
