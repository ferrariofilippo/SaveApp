using App.Models.Enums;

namespace App.Helpers
{
	public interface INativeThemeManager
	{
		void OnThemeChanged(Theme theme);
	}
}
