using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using LineupGenerator.Controller;

namespace LineupGenerator
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new LineupViewModel();
		}

		private void OnRetrievePlayersClick(object sender, RoutedEventArgs e)
		{
			((LineupViewModel)DataContext).RetrievePlayerList();
		}

		private void OnGenerateLineupsClick(object sender, RoutedEventArgs e)
		{
			((LineupViewModel)DataContext).GenerateLineups();
		}

		#region Sort

		private GridViewColumnHeader listViewSortCol = null;
		private SortAdorner listViewSortAdorner = null;

		private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
		{
			GridViewColumnHeader column = (sender as GridViewColumnHeader);
			string sortBy = column.Tag.ToString();
			string[] sortBys = sortBy.Split(',');
			sortBy = sortBys[0];
			if (listViewSortCol != null)
			{
				AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
				LineupsListView.Items.SortDescriptions.Clear();
			}

			ListSortDirection newDir = sortBys.Length > 1 && sortBys[1] == "Descending" ? ListSortDirection.Descending : ListSortDirection.Ascending;
			if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
			{
				newDir = newDir == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
			}

			listViewSortCol = column;
			listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
			AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
			LineupsListView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
		}

		public class SortAdorner : Adorner
		{
			private static Geometry ascGeometry = Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

			private static Geometry descGeometry = Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

			public ListSortDirection Direction { get; private set; }

			public SortAdorner(UIElement element, ListSortDirection dir)
				: base(element)
			{
				this.Direction = dir;
			}

			protected override void OnRender(DrawingContext drawingContext)
			{
				base.OnRender(drawingContext);

				if (AdornedElement.RenderSize.Width < 20)
				{
					return;
				}

				TranslateTransform transform = new TranslateTransform(AdornedElement.RenderSize.Width - 15, (AdornedElement.RenderSize.Height - 5) / 2);
				drawingContext.PushTransform(transform);

				Geometry geometry = ascGeometry;
				if (this.Direction == ListSortDirection.Descending)
				{
					geometry = descGeometry;
				}
				drawingContext.DrawGeometry(Brushes.Black, null, geometry);

				drawingContext.Pop();
			}
		}

		#endregion
	}
}
