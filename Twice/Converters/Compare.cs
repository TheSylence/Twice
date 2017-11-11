using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Twice.Converters
{
	internal class Compare : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			double n;

			if( value is double )
			{
				n = (double)value;
			}
			else
			{
				if( !double.TryParse( value?.ToString(), NumberStyles.Any, culture, out n ) )
				{
					return DependencyProperty.UnsetValue;
				}
			}

			double compareTo = CompareValue;

			if( parameter != null )
			{
				double tmp;
				if( !double.TryParse( parameter.ToString(), NumberStyles.Any, culture, out tmp ) )
				{
					return DependencyProperty.UnsetValue;
				}

				compareTo = tmp;
			}

			switch( Operation )
			{
			case CompareOperation.Equal:
				return Math.Abs( compareTo - n ) < RoundingTolerance;

			case CompareOperation.NotEqual:
				return Math.Abs( compareTo - n ) > RoundingTolerance;

			case CompareOperation.Less:
				return n < compareTo;

			case CompareOperation.LessOrEqual:
				return n <= compareTo;

			case CompareOperation.Greater:
				return n > compareTo;

			case CompareOperation.GreaterOrEqual:
				return n >= compareTo;
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}

		private const double RoundingTolerance = 0.0001;

		public double CompareValue { get; set; }
		public CompareOperation Operation { get; set; }
	}
}