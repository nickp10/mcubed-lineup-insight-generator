using System;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class PlayerCardGameStats
	{
		[JsonProperty("date")]
		public DateTime? Date { get; set; }

		[JsonProperty("opponent")]
		public string Opponent { get; set; }

		[JsonProperty("points")]
		public double? Points { get; set; }
	}
}
