using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SASB_SI_ASSESSMENT.Converters
{
    /// <summary>
    /// Converts a device status string to a corresponding Brush for UI display.
    /// Used for color-coding device status in the interface.
    /// </summary>
    public class StatusToBrushConverter : IValueConverter
    {
        // Converts Status string → Brush
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Online" => Brushes.Green,
                    "Idle"   => Brushes.Orange,
                    "Offline" => Brushes.Gray,
                    "Error"  => Brushes.Red,
                    "Off"    => Brushes.DarkGray,
                    _ => Brushes.Black
                };
            }
            return Brushes.Black;
        }

        /// <summary>
        /// ConvertBack is not implemented since binding is one-way (status → color).
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
