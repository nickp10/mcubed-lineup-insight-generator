using System.ComponentModel;

namespace mCubed.LineupGenerator.Model
{
	public interface ISortable
	{
		ListSortDirection? SortDirection { get; }
		string SortProperty { get; }
		void Sort(string property, ListSortDirection initialSortDirection = ListSortDirection.Ascending);
	}
}
