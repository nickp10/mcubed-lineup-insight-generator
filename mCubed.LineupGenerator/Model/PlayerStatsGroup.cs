using System.Collections.Generic;

namespace mCubed.LineupGenerator.Model
{
	public class PlayerStatsGroup
	{
		public string GroupName { get; set; }
		public IEnumerable<PlayerStatsGroupItem> Stats { get; set; }
	}
}
