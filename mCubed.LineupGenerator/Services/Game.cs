using System;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class Game
	{
		[JsonProperty("awayTeam")]
		public Team AwayTeam { get; set; }

		[JsonProperty("homeTeam")]
		public Team HomeTeam { get; set; }

		[JsonProperty("startTime")]
		public DateTime? StartTime { get; set; }
	}
}
