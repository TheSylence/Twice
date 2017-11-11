using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Anotar.NLog;

namespace Twice.Converters
{
	internal class MathConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			double a, b;

			try
			{
				a = System.Convert.ToDouble( value, CultureInfo.InvariantCulture );
			}
			catch( Exception ex )
			{
				LogTo.WarnException( $"Failed to parse value {value}", ex );
				return DependencyProperty.UnsetValue;
			}

			try
			{
				b = System.Convert.ToDouble( parameter, CultureInfo.InvariantCulture );
			}
			catch( Exception ex )
			{
				LogTo.WarnException( $"Failed to parse parameter {parameter}", ex );
				return DependencyProperty.UnsetValue;
			}

			switch( Operation )
			{
			case MathOperation.Add:
				return a + b;

			case MathOperation.Substract:
				return a - b;

			case MathOperation.Multiply:
				return a * b;

			case MathOperation.Divide:
				return a / b;
			}

			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}

		public MathOperation Operation { get; set; }
	}
}