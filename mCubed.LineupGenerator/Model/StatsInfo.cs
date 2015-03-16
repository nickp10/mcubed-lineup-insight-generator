using System.Collections.Generic;

namespace mCubed.LineupGenerator.Model
{
	public class StatsInfo
	{
		public IDictionary<string, InjuryData> InjuryMappings { get; set; }
		public int NameGroupIndex { get; set; }
		public int ProjectedGroupIndex { get; set; }
		public int RecentGroupIndex { get; set; }
		public int SeasonGroupIndex { get; set; }
		public string NumberFirePlayers { get; set; }
		public string NumberFirePlayerID { get; set; }
		public string NumberFireProjections { get; set; }
		public string NumberFireProjectedPoints { get; set; }
		public string Regex { get; set; }
		public string URLNumberFire { get; set; }
		public string URLRotoWire { get; set; }
	}
}
