﻿using Common.ValueRanges;
using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFVersion.Converters
{
    internal sealed class RangedDoubleToStringToConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = default;
            try
            {
                result = System.Convert.ToDouble(value);
                result = Math.Round(result, 0);
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double result = default;

            if (IsValidParametres(value, parameter))
            {
                value = value?.ToString().Replace('.', ',');
                var range = parameter as IValueRange;
                var val = System.Convert.ToDouble(value);
                if (val > range.UpperValueOfRange)
                    val = range.UpperValueOfRange;
                if (val < range.LowerValueOfRange)
                    val = range.LowerValueOfRange;
                result = val;
            }

            return result;
        }

        private bool IsValidParametres(object value, object parametre)
        {
            return double.TryParse(value?.ToString(), out _) && parametre is IValueRange;
        }
    }
}
