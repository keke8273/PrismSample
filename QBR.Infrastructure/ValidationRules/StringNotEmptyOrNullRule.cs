using System;
using System.Globalization;
using System.Windows.Controls;

namespace QBR.Infrastructure.ValidationRules
{
    public class StringNotEmptyOrNullRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (String.IsNullOrEmpty((String) value))
            {
                return new ValidationResult(false, "Field cannot be empty");
            }

            return ValidationResult.ValidResult;
        }
    }
}
