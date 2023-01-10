using App.Models.Enums;

namespace App.Helpers.Themes
{
    public interface INativeThemeManager
    {
        void OnThemeChanged(Theme theme);
    }
}
