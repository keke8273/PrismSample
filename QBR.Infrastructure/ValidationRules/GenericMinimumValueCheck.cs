using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace QBR.Infrastructure.ValidationRules
{
    public class GenericMinimumValueCheck <T> : ValidationRule where T:IComparable
    {
        public T Min { get; set;}

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            if (string.IsNullOrEmpty((string)value))
            {
                return new ValidationResult(false, "Field cannot be empty");
            }

            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                var parsedValue = (T)converter.ConvertFromString((string)value);

                if (parsedValue.CompareTo(Min) < 0)
                    return new ValidationResult(false, string.Format("Enter a value greater than {0}.", Min));
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Illegal characters: " + (string)value);
            }

            return ValidationResult.ValidResult;
        }
    }
}
