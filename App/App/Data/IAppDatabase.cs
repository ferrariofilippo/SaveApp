using App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Data
{
    /// <summary>
    /// An object that give access to a local SQLite database and provides basic APIs
    /// </summary>
    public interface IAppDatabase
    {
		/// <summary>
		/// Asynchronously query the database to fetch <see cref="Movement"/>
		/// </summary>
		/// <returns>A list of all the saved instances of <see cref="Movement"/></returns>
		Task<List<Movement>> GetMovementsAsync();

		/// <summary>
		/// Asynchronously query the database to fetch <see cref="Subscription"/>
		/// </summary>
		/// <returns>A list of all the saved instances of <see cref="Subscription"/></returns>
		Task<List<Subscription>> GetSubscriptionsAsync();

		/// <summary>
		/// Asynchronously query the database to fetch <see cref="Budget"/>
		/// </summary>
		/// <returns>A list of all the saved instances of <see cref="Budget"/></returns>
		Task<List<Budget>> GetBudgetsAsync();

		/// <summary>
		/// Asynchronously query the database to fetch a specific <see cref="Budget"/>
		/// </summary>
		/// <param name="id">Id of the budget to fetch</param>
		/// <returns>Matching <see cref="Budget"/>, otherwise null</returns>
		Task<Budget> GetBudgetAsync(int id);

		/// <summary>
		/// Asynchronously query the database to save a <see cref="Movement"/>
		/// </summary>
		/// <param name="movement"><see cref="Movement"/> that has to be saved</param>
		/// <returns>Number of updated rows</returns>
		Task<int> SaveMovementAsync(Movement movement);

		/// <summary>
		/// Asynchronously query the database to save a list of <see cref="Movement"/>
		/// </summary>
		/// <param name="movements">List of <see cref="Movement"/> that has to be saved</param>
		/// <returns>Number of updated rows</returns>
		Task<int> SaveMovementsAsync(IEnumerable<Movement> movements);

		/// <summary>
		/// Asynchronously query the database to save a list of <see cref="Movement"/>
		/// </summary>
		/// <param name="subscription"><see cref="Subscription"/> that has to be saved</param>
		/// <returns>Number of updated rows</returns>
		Task<int> SaveSubscriptionAsync(Subscription subscription);

		/// <summary>
		/// Asynchronously query the database to save an instance of <see cref="Budget"/>
		/// </summary>
		/// <param name="budget"><see cref="Budget"/> that has to be saved</param>
		/// <returns>Number of updated rows</returns>
		Task<int> SaveBudgetAsync(Budget budget);

		/// <summary>
		/// Asynchronously query the database to delete a specific <see cref="Movement"/>
		/// </summary>
		/// <param name="movement"><see cref="Movement"/> that should be deleted</param>
		/// <returns>Number of updated rows</returns>
		Task<int> DeleteMovementAsync(Movement movement);

		/// <summary>
		/// Asynchronously query the database to delete a specific <see cref="Subscription"/>
		/// </summary>
		/// <param name="subscription"><see cref="Subscription"/> that should be deleted</param>
		/// <returns>Number of updated rows</returns>
		Task<int> DeleteSubscriptionAsync(Subscription subscription);

		/// <summary>
		/// Asynchronously query the database to delete a specific <see cref="Budget"/>
		/// </summary>
		/// <param name="budget"><see cref="Budget"/> that should be deleted</param>
		/// <returns>Number of updated rows</returns>
		Task<int> DeleteBudgetAsync(Budget budget);

		/// <summary>
		/// Updates all the instances saved on the db to a new currency
		/// </summary>
		/// <param name="changeRatio">Change ratio (NewCurrency / OldCurrency)</param>
		/// <returns>A task representing the async operation</returns>
		Task UpdateDbToNewCurrency(decimal changeRatio);
    }
}
