using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AInBox.Astove.Core.Validations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class MinimumAttribute : ValidationAttribute
    {
        private readonly int _minimumIntValue;
        private readonly float _minimumFloatValue;
        private readonly double _minimumDoubleValue;
        private readonly decimal _minimumDecimalValue;

        public MinimumAttribute(int minimum) : base(errorMessage: "O campo {0} tem que ser no mínimo {1}.")
        {
            _minimumIntValue = minimum;
        }

        public MinimumAttribute(float minimum) : base(errorMessage: "O campo {0} tem que ser no mínimo {1}.")
        {
            _minimumFloatValue = minimum;
        }

        public MinimumAttribute(double minimum) : base(errorMessage: "O campo {0} tem que ser no mínimo {1}.")
        {
            _minimumDoubleValue = minimum;
        }

        public MinimumAttribute(decimal minimum) : base(errorMessage: "O campo {0} tem que ser no mínimo {1}.")
        {
            _minimumDecimalValue = minimum;
        }

        public override string FormatErrorMessage(string name)
        {
            if (_minimumIntValue > 0)
                return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, name, _minimumIntValue);
            else if (_minimumFloatValue > 0)
                return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, name, _minimumFloatValue);
            else if (_minimumDoubleValue > 0)
                return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, name, _minimumDoubleValue);
            else
                return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, name, _minimumDecimalValue);
        }

        public override bool IsValid(object value)
        {
            int intValue;
            float floatValue;
            double doubleValue;
            decimal decimalValue;
            if (value != null && value.GetType() == typeof(Int32) && int.TryParse(value.ToString(), out intValue))
            {
                return (intValue >= _minimumIntValue);
            }
            else if (value != null && value.GetType() == typeof(Single) && float.TryParse(value.ToString(), NumberStyles.Number, Astove.Core.Globalization.Cultures.PTBR, out floatValue))
            {
                return (floatValue >= _minimumFloatValue);
            }
            else if (value != null && value.GetType() == typeof(Double) && double.TryParse(value.ToString(), NumberStyles.Number, Astove.Core.Globalization.Cultures.PTBR, out doubleValue))
            {
                return (doubleValue >= _minimumDoubleValue);
            }
            else if (value != null && value.GetType() == typeof(Decimal) && decimal.TryParse(value.ToString(), NumberStyles.Number, Astove.Core.Globalization.Cultures.PTBR, out decimalValue))
            {
                return (decimalValue >= _minimumDecimalValue);
            }
            return false;
        }
    }
}
