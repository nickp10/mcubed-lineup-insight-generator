using System.Collections.Generic;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class Player
	{
		[JsonProperty("battingOrder")]
		public string BattingOrder { get; set; }

		[JsonProperty("ID")]
		public string ID { get; set; }

		[JsonProperty("injury")]
		public PlayerInjury Injury { get; set; }

		[JsonProperty("isPlaying")]
		public bool IsPlaying { get; set; }

		[JsonProperty("isProbablePitcher")]
		public bool IsProbablePitcher { get; set; }

		[JsonProperty("isStarter")]
		public bool IsStarter { get; set; }

		[JsonProperty("likability")]
		public double? Likability { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("newsStatus")]
		public NewsStatus NewsStatus { get; set; }

		[JsonProperty("position")]
		public string Position { get; set; }

		[JsonProperty("projectedPointsPerDollar")]
		public double? ProjectedPointsPerDollar { get; set; }

		[JsonProperty("projectedCeiling")]
		public double? ProjectedCeiling { get; set; }

		[JsonProperty("projectedFloor")]
		public double? ProjectedFloor { get; set; }

		[JsonProperty("projectedPoints")]
		public double? ProjectedPoints { get; set; }

		[JsonProperty("recentAveragePoints")]
		public double? RecentAveragePoints { get; set; }

		[JsonProperty("salary")]
		public double Salary { get; set; }

		[JsonProperty("seasonAveragePoints")]
		public double? SeasonAveragePoints { get; set; }

		[JsonProperty("stats")]
		public List<PlayerStats> Stats { get; set; }

		[JsonProperty("team")]
		public string Team { get; set; }
	}
}
