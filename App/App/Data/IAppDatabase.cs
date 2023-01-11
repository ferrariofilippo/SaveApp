using App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Data
{
    public interface IAppDatabase
    {
        Task<List<Movement>> GetMovementsAsync();

        Task<List<Subscription>> GetSubscriptionsAsync();

        Task<List<Budget>> GetBudgetsAsync();

        Task<Budget> GetBudgetAsync(int id);

        Task<int> SaveMovementAsync(Movement movement);

        Task<int> SaveMovementsAsync(IEnumerable<Movement> movements);

        Task<int> SaveSubscriptionAsync(Subscription subscription);

        Task<int> SaveBudgetAsync(Budget budget);

        Task<int> DeleteMovementAsync(Movement movement);

        Task<int> DeleteSubscriptionAsync(Subscription subscription);

        Task<int> DeleteBudgetAsync(Budget budget);

        Task UpdateDbToNewCurrency(decimal changeRatio);
    }
}
