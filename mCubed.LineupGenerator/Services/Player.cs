using System.Collections.Generic;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class Player
	{
		[JsonProperty("ID")]
		public string ID { get; set; }

		[JsonProperty("injury")]
		public PlayerInjury Injury { get; set; }

		[JsonProperty("isPlaying")]
		public bool IsPlaying { get; set; }

		[JsonProperty("isStarter")]
		public bool IsStarter { get; set; }

		[JsonProperty("likeability")]
		public double? Likeability { get; set; }

		[JsonProperty("mlbSpecific")]
		public PlayerMLBSpecific MLBSpecific { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("newsStatus")]
		public NewsStatus NewsStatus { get; set; }

		[JsonProperty("opponent")]
		public string Opponent { get; set; }

		[JsonProperty("oppositionPercentile")]
		public double? OppositionPercentile { get; set; }

		[JsonProperty("position")]
		public string Position { get; set; }

		[JsonProperty("positionEligibility")]
		public List<string> PositionEligibility { get; set; }

		[JsonProperty("projectedPointsPercentiles")]
		public List<PositionPercentile> ProjectedPointsPercentiles { get; set; }

		[JsonProperty("projectedPointsPerDollar")]
		public double? ProjectedPointsPerDollar { get; set; }

		[JsonProperty("projectedPointsPerDollarPercentiles")]
		public List<PositionPercentile> ProjectedPointsPerDollarPercentiles { get; set; }

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

		[JsonProperty("thumbnailURL")]
		public string ThumbnailURL { get; set; }
	}
}
