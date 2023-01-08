using App.Helpers;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(App.Droid.AndroidFileSaver))]
namespace App.Droid
{
	public class AndroidFileSaver : IFileSaver
	{
		public async Task SaveFile(string path, string content)
		{
			string filePath = Path.Combine(
				Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
				Android.OS.Environment.DirectoryDownloads,
				path);

			await File.WriteAllTextAsync(filePath, content);
		}
	}
}
