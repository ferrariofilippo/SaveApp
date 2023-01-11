using App.Models;
using System.Threading.Tasks;

namespace App.Data
{
    public interface ISettingsManager
    {
        Settings Settings { get; }

        Task SaveSettings();
    }
}
