using System.Collections.Generic;
using mCubed.LineupGenerator.Services;

namespace mCubed.LineupGenerator.Controller
{
	public class ContestPositionComparer : IEqualityComparer<ContestPosition>
	{
		public bool Equals(ContestPosition x, ContestPosition y)
		{
			return x.Label == y.Label;
		}

		public int GetHashCode(ContestPosition obj)
		{
			return obj.Label.GetHashCode();
		}
	}
}
