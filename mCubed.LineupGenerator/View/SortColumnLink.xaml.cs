using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.View
{
	public partial class SortColumnLink : UserControl
	{
		#region Properties

		#region InitialSortDirection

		public static readonly DependencyProperty InitialSortDirectionProperty =
			DependencyProperty.Register("InitialSortDirection", typeof(ListSortDirection), typeof(SortColumnLink), new PropertyMetadata(ListSortDirection.Ascending));

		public ListSortDirection InitialSortDirection
		{
			get { return (ListSortDirection)GetValue(InitialSortDirectionProperty); }
			set { SetValue(InitialSortDirectionProperty, value); }
		}

		#endregion

		#region SortProperty

		public static readonly DependencyProperty SortPropertyProperty =
			DependencyProperty.Register("SortProperty", typeof(string), typeof(SortColumnLink), new PropertyMetadata(null));

		public string SortProperty
		{
			get { return (string)GetValue(SortPropertyProperty); }
			set { SetValue(SortPropertyProperty, value); }
		}

		#endregion

		#region SortSource

		public static readonly DependencyProperty SortSourceProperty =
			DependencyProperty.Register("SortSource", typeof(ISortable), typeof(SortColumnLink), new PropertyMetadata(null));

		public ISortable SortSource
		{
			get { return (ISortable)GetValue(SortSourceProperty); }
			set { SetValue(SortSourceProperty, value); }
		}

		#endregion

		#region Text

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(SortColumnLink), new PropertyMetadata(null));

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		#endregion

		#endregion

		#region Constructors

		public SortColumnLink()
		{
			InitializeComponent();
		}

		#endregion

		#region Methods

		private void OnClick(object sender, RoutedEventArgs e)
		{
			var source = SortSource;
			if (source != null)
			{
				source.Sort(SortProperty, InitialSortDirection);
			}
		}

		#endregion
	}
}
