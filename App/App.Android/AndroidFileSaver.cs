using Android.OS;
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
				Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).AbsolutePath,
				path);

			await File.WriteAllTextAsync(filePath, content);
		}
	}
}
