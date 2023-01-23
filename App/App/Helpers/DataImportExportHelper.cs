using App.Data;
using App.Helpers.Notifications;
using App.Models;
using App.Resx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App.Helpers
{
	public static class DataImportExportHelper
	{
		private static readonly Dictionary<DevicePlatform, IEnumerable<string>> _fileTypes = new Dictionary<DevicePlatform, IEnumerable<string>>
		{
			{ DevicePlatform.Android, new[] { ".csv" } }
		};

		private static readonly FilePickerFileType _acceptedFileTypes = new FilePickerFileType(_fileTypes);

		private static bool _isExporting;

		private static readonly object _lock = new object();

		public static async Task ExportData()
		{
			lock (_lock)
			{
				if (_isExporting)
					return;
				else
					_isExporting = true;
			}

			try
			{
				var database = DependencyService.Get<IAppDatabase>();
				var saver = DependencyService.Get<IFileSaver>();

				await Task.WhenAll(
					SaveMovements(database, saver),
					SaveSubscriptionts(database, saver));

				NotificationHelper.SendNotification(
					AppResource.FileDownloadedNotificationTitle,
					AppResource.FileDownloadNotificationDescription);
			}
			catch (Exception ex)
			{
				NotificationHelper.NotifyException(ex);
			}
			finally
			{
				lock (_lock)
					_isExporting = false;
			}
		}

		public static async Task ImportMovements()
		{
			var movements = await GetMovementsFromFile();
			if (movements is null)
			{
				NotificationHelper.SendNotification(
					AppResource.ImportFailedTitle,
					AppResource.ImportFailedMessage);
				return;
			}
			var database = DependencyService.Get<IAppDatabase>();
			var stats = DependencyService.Get<StatisticsManager>();
			await Task.WhenAll( 
				database.SaveMovementsAsync(movements),
				stats.AddMovements(movements));
			NotificationHelper.SendNotification(
				AppResource.ImportSuccededTitle,
				AppResource.ImportSuccededMessage);
		}

		public static async Task GetTemplateFile()
		{
			var saver = DependencyService.Get<IFileSaver>();
			var path = $"SaveApp_Template.csv";
			await saver.SaveFile(path, Constants.MOVEMENTS_FILE_HEADER);
			NotificationHelper.SendNotification(
				AppResource.FileDownloadedNotificationTitle,
				AppResource.TemplateDownloadedMessage);
		}

		private static async Task SaveMovements(IAppDatabase database, IFileSaver saver)
		{
			var builder = new StringBuilder(Constants.MOVEMENTS_FILE_HEADER);
			var movements = await database.GetMovementsAsync();
			var path = $"{AppResource.Movements}_{DateTime.Now:dd-MM-yyyy}.csv";

			if (!(movements is null))
			{
				foreach (var item in movements)
				{
					builder.AppendLine(
						$"{item.Id},{item.Value},{item.IsExpense},{item.Description}," +
						$"{(item.IsExpense ? item.ExpenseType.ToString() : "null")}," +
						$"{item.CreationDate.Date}");
				}
			}

			await saver.SaveFile(path, builder.ToString());
		}

		private static async Task SaveSubscriptionts(IAppDatabase database, IFileSaver saver)
		{
			var builder = new StringBuilder(Constants.SUBSCRIPTIONS_FILE_HEADER);
			var subs = await database.GetSubscriptionsAsync();
			var path = $"{AppResource.Subscriptions}_{DateTime.Now:dd-MM-yyyy}.csv";

			if (!(subs is null))
			{
				foreach (var item in subs)
				{
					builder.AppendLine(
						$"{item.Id},{item.Value},{item.Description},{item.ExpenseType}" +
						$"{item.RenewalType},{item.LastPaid.Date},{item.NextRenewal.Date}" +
						$"{item.CreationDate.Date}");
				}
			}

			await saver.SaveFile(path, builder.ToString());
		}

		private static async Task<List<Movement>> GetMovementsFromFile()
		{
			var options = new PickOptions()
			{
				FileTypes = _acceptedFileTypes,
				PickerTitle = AppResource.FilePickerTitle
			};
			var selectedFile = await FilePicker.PickAsync(options);
			if (selectedFile is null)
				return null;

			List<Movement> movements = null;
			using (var reader = new StreamReader(await selectedFile.OpenReadAsync()))
			{
				var fileContent = await reader.ReadToEndAsync();
				if (!string.IsNullOrWhiteSpace(fileContent))
					movements = JsonSerializer.Deserialize<List<Movement>>(fileContent);
			}
			return movements;
		}
	}
}
