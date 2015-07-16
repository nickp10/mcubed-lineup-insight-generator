using System.Collections.Generic;
using mCubed.LineupGenerator.StartingPlayerRetrievers;
using mCubed.LineupGenerator.StatRetrievers;

namespace mCubed.LineupGenerator.Model
{
	public class StatsInfo
	{
		public IEnumerable<IStatRetriever> StatRetrievers { get; set; }
		public IStartingPlayerRetriever StartingPlayerRetriever { get; set; }
	}
}
