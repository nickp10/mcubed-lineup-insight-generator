using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class PlayerStats
	{
		[JsonProperty("projectedCeiling")]
		public double? ProjectedCeiling { get; set; }

		[JsonProperty("projectedFloor")]
		public double? ProjectedFloor { get; set; }

		[JsonProperty("projectedPoints")]
		public double? ProjectedPoints { get; set; }

		[JsonProperty("recentAveragePoints")]
		public double? RecentAveragePoints { get; set; }

		[JsonProperty("seasonAveragePoints")]
		public double? SeasonAveragePoints { get; set; }

		[JsonProperty("source")]
		public string Source { get; set; }
	}
}
