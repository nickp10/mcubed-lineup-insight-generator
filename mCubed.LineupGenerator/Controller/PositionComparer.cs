using System.Collections.Generic;
using System.Linq;

namespace LineupGenerator.Controller
{
	public class PositionComparer : IComparer<string>
	{
		#region Data Members

		private readonly IList<string> _positions;

		#endregion

		#region Constructors

		public PositionComparer(IEnumerable<string> positions)
		{
			_positions = positions.ToList();
		}

		#endregion

		#region IComparer<string> Members

		public int Compare(string x, string y)
		{
			var indexOfX = _positions.IndexOf(x);
			var indexOfY = _positions.IndexOf(y);
			return indexOfX - indexOfY;
		}

		#endregion
	}
}
