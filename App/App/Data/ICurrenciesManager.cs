using App.Models.Enums;
using System;
using System.Threading.Tasks;

namespace App.Data
{
    public interface ICurrenciesManager
    {
        DateTime LastUpdate { get; }

        decimal ConvertCurrencyToDefault(decimal value, Currencies from);

        Task UpdateAllToCurrent(Currencies previous);
	}
}
