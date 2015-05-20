using System.Collections.Generic;
using mCubed.LineupGenerator.StatRetrievers;

namespace mCubed.LineupGenerator.Model
{
	public class StatsInfo
	{
		public IDictionary<string, InjuryData> InjuryMappings { get; set; }
		public IEnumerable<IStatRetriever> StatRetrievers { get; set; }
	}
}
