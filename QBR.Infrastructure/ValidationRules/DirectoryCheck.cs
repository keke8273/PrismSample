using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Windows.Controls;

namespace QBR.Infrastructure.ValidationRules
{
    public class DirectoryCheck : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (String.IsNullOrEmpty((String) value))
            {
                return new ValidationResult(false, "Field cannot be empty");
            }

            try
            {
                var directoryInfo = new DirectoryInfo((string) value);
            }
            catch (SecurityException exception)
            {
                return new ValidationResult(false, "Security limited. User cannot access the directory");
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Invalid directory");
            }

            return ValidationResult.ValidResult;
        }
    }
}
