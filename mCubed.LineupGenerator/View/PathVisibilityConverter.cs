using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace mCubed.LineupGenerator.View
{
	public class PathVisibilityConverter : IMultiValueConverter
	{
		#region IMultiValueConverter Members

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var linkDirection = parameter as string;
			if (values != null && values.Length == 3 && linkDirection != null)
			{
				var linkProperty = values[0] as string;
				var sourceProperty = values[1] as string;
				var sourceDirection = values[2] as ListSortDirection?;
				if (linkProperty != null && sourceProperty != null && sourceDirection != null)
				{
					if (linkProperty == sourceProperty && sourceDirection.Value.ToString() == linkDirection)
					{
						return Visibility.Visible;
					}
				}
			}
			return Visibility.Collapsed;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
