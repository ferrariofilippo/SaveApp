using App.Models.Enums;

namespace App.Helpers.Themes
{
    /// <summary>
    /// An OS specific object that manages user's theme
    /// </summary>
    public interface INativeThemeManager
    {
        /// <summary>
        /// Raises ThemeChanged event
        /// </summary>
        /// <param name="theme">Theme selected by the user</param>
        void OnThemeChanged(Theme theme);
    }
}
