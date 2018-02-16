using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.Controller
{
	public class SortableListCollectionView : ListCollectionView, ISortable
	{
		#region Constructors

		public SortableListCollectionView(IList list)
			: base(list)
		{
		}

		#endregion

		#region Properties

		#region SortDirection

		private ListSortDirection? _sortDirection;
		public ListSortDirection? SortDirection
		{
			get { return _sortDirection; }
			private set
			{
				if (_sortDirection != value)
				{
					_sortDirection = value;
					OnPropertyChanged(new PropertyChangedEventArgs("SortDirection"));
				}
			}
		}

		#endregion

		#region SortProperty

		private string _sortProperty;
		public string SortProperty
		{
			get { return _sortProperty; }
			private set
			{
				if (_sortProperty != value)
				{
					_sortProperty = value;
					OnPropertyChanged(new PropertyChangedEventArgs("SortProperty"));
				}
			}
		}

		#endregion

		#endregion

		#region Methods

		public void Sort(string property, ListSortDirection initialSortDirection)
		{
			ListSortDirection? sortDirection = null;
			var existingSort = SortDescriptions.FirstOrDefault(s => s.PropertyName == property);
			if (existingSort.PropertyName != property)
			{
				sortDirection = initialSortDirection;
			}
			else
			{
				var existingSortDirection = existingSort.Direction;
				if (existingSortDirection == ListSortDirection.Ascending)
				{
					sortDirection = initialSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : (ListSortDirection?)null;
				}
				else
				{
					sortDirection = initialSortDirection == ListSortDirection.Descending ? ListSortDirection.Ascending : (ListSortDirection?)null;
				}
			}
			SortDescriptions.Clear();
			if (sortDirection == null)
			{
				SortProperty = null;
				SortDirection = null;
			}
			else
			{
				SortDescriptions.Add(new SortDescription(property, sortDirection.Value));
				SortProperty = property;
				SortDirection = sortDirection.Value;
			}
		}

		#endregion
	}
}
