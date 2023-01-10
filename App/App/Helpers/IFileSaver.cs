using System.Threading.Tasks;

namespace App.Helpers
{
    public interface IFileSaver
    {
        Task SaveFile(string relativePath, string content);
    }
}
