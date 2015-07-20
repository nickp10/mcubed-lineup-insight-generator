﻿
namespace mCubed.LineupGenerator.Model
{
	public class PlayerStats
	{
		public string Name { get; set; }
		public string Source { get; set; }
		public string BattingOrder { get; set; }
		public string ESPNID { get; set; }
		public double? ProjectedPoints { get; set; }
		public double? RecentAveragePoints { get; set; }
		public double? SeasonAveragePoints { get; set; }
	}
}
