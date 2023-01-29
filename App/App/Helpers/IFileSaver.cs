using System.Threading.Tasks;

namespace App.Helpers
{
    /// <summary>
    /// An OS dependent object that provide the ability to write files in shared folders
    /// </summary>
    public interface IFileSaver
    {
        /// <summary>
        /// Asynchronously writes a string to a file in Downloads shared folder
        /// </summary>
        /// <param name="relativePath">File relative path</param>
        /// <param name="content">String to write</param>
        /// <returns>A task representing the async operation</returns>
        Task SaveFile(string relativePath, string content);
    }
}
