using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using mCubed.LineupGenerator.Controller;

namespace mCubed.LineupGenerator
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

		#region ItemsSource

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(SortableListCollectionView), typeof(SortColumnLink), new PropertyMetadata(null));

		public SortableListCollectionView ItemsSource
		{
			get { return (SortableListCollectionView)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
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
			var source = ItemsSource;
			if (source != null)
			{
				source.Sort(SortProperty, InitialSortDirection);
			}
		}

		#endregion
	}
}
