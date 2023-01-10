using SkiaSharp;
using Xamarin.Forms;

namespace App
{
    public static class ReadOnlies
    {
        // Months of the year
        public static readonly string[] Months =
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        // Xamarin Types Colors
        public static readonly Color[] MovementTypeColors =
        {
            new Color(213.0 / 255, 166.0 / 255, 189.0 / 255),
            new Color(217.0 / 255, 234.0 / 255, 211.0 / 255),
            new Color(164.0 / 255, 194.0 / 255, 244.0 / 255),
            new Color(221.0 / 255, 126.0 / 255, 107.0 / 255),
            new Color(249.0 / 255, 219.0 / 255, 156.0 / 255),
            new Color(162.0 / 255, 196.0 / 255, 201.0 / 255),
            new Color(180.0 / 255, 167.0 / 255, 214.0 / 255),
            new Color(159.0 / 255, 197.0 / 255, 232.0 / 255),
            new Color(217.0 / 255, 217.0 / 255, 217.0 / 255),
            new Color(182.0 / 255, 215.0 / 255, 168.0 / 255),
            new Color(255.0 / 255, 217.0 / 255, 102.0 / 255),
            new Color(234.0 / 255, 153.0 / 255, 153.0 / 255)
        };

        // SkiaSharp Types Colors
        public static readonly SKColor[] SK_MovementTypeColors =
        {
            new SKColor(213, 166, 189),
            new SKColor(217, 234, 211),
            new SKColor(164, 194, 244),
            new SKColor(221, 126, 107),
            new SKColor(249, 219, 156),
            new SKColor(162, 196, 201),
            new SKColor(180, 167, 214),
            new SKColor(159, 197, 232),
            new SKColor(217, 217, 217),
            new SKColor(182, 215, 168),
            new SKColor(255, 217, 102),
            new SKColor(234, 153, 153)
        };
    }
}
