using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using QBR.Infrastructure.Models;

namespace QBR.AnalyzerManagerModule.Resources.ValueConverters
{
    /// <summary>
    /// Data binding converter converts AnalyzerType to bitmap icons
    /// </summary>
    public class AnalyzerTypeToIconConverter : IValueConverter
    {
        #region Member Variables
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Functions
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var analyzerType = (AnalyzerTypes)value;
            switch (analyzerType)
            {
                case AnalyzerTypes.PRO:
                    return Application.Current.FindResource("ProteusMeterIcon");
                case AnalyzerTypes.RUB:
                    return Application.Current.FindResource("RubixMeterIcon");
                default:
                    return new BitmapImage();
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Enums
        #endregion

    }
}