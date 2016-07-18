using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using QBR.Infrastructure.Models;
using QBR.Infrastructure.Models.Enums;

namespace QBR.UserDataModule.Resources.ValueConverters
{
    /// <summary>
    /// Data binding converter converts AnalyzerType to bitmap icons
    /// </summary>
    public class ProjectToTestTargetConverter : IValueConverter
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
            var stripType = (StripType)value;
            switch (stripType)
            {
                case StripType.Proteus:
                    return new List<string>()
                    {
                        "MultiCalibrator lvl1",
                        "MultiCalibrator lvl2",
                        "MultiCalibrator lvl3",
                        "MultiCalibrator lvl4",
                        "MultiCalibrator lvl5",
                        "MultiCalibrator lvl6",
                    };
                default:
                    return new List<string>();
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