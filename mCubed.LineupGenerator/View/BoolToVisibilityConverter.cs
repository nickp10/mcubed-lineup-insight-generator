using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace mCubed.LineupGenerator.View
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public sealed class BoolToVisibilityConverter : IValueConverter
	{
		#region Properties

		#region FalseValue

		private Visibility _falseValue = Visibility.Collapsed;
		public Visibility FalseValue
		{
			get { return _falseValue; }
			set { _falseValue = value; }
		}

		#endregion

		#region TrueValue

		private Visibility _trueValue = Visibility.Visible;
		public Visibility TrueValue
		{
			get { return _trueValue; }
			set { _trueValue = value; }
		}

		#endregion

		#endregion

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool))
			{
				return null;
			}
			return (bool)value ? TrueValue : FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (Equals(value, TrueValue))
			{
				return true;
			}
			if (Equals(value, FalseValue))
			{
				return false;
			}
			return null;
		}

		#endregion
	}
}
