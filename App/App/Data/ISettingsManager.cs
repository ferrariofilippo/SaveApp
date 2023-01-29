using App.Models;
using System.Threading.Tasks;

namespace App.Data
{
    /// <summary>
    /// An object that provide the ability to access settings and save them
    /// </summary>
    public interface ISettingsManager
    {
        /// <summary>
        /// User settings
        /// </summary>
        Settings Settings { get; }

		/// <summary>
		/// Asynchronously saves user's settings
		/// </summary>
		/// <returns>A task representing the async operation</returns>
		Task SaveSettings();
    }
}
