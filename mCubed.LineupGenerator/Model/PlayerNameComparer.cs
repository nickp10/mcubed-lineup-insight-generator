using System.Collections.Generic;

namespace mCubed.LineupGenerator.Model
{
	public class PlayerNameComparer : IEqualityComparer<Player>
	{
		#region IEqualityComparer<Player> Members

		public bool Equals(Player x, Player y)
		{
			if (x == null)
			{
				return y == null;
			}
			else if (y == null)
			{
				return false;
			}
			else
			{
				return x.Name == y.Name;
			}
		}

		public int GetHashCode(Player obj)
		{
			return obj.Name.GetHashCode();
		}

		#endregion
	}
}
