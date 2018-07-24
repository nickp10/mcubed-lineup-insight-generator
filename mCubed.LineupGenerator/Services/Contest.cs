using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class Contest
	{
		[JsonProperty("contestType")]
		public ContestType ContestType { get; set; }

		[JsonProperty("games")]
		public List<Game> Games { get; set; }

		[JsonProperty("ID")]
		public string ID { get; set; }

		[JsonProperty("label")]
		public string Label { get; set; }

		[JsonProperty("maxPlayersPerTeam")]
		public int? MaxPlayersPerTeam { get; set; }

		[JsonProperty("maxSalary")]
		public double? MaxSalary { get; set; }

		[JsonProperty("playerDataLastUpdateTime")]
		public DateTime? PlayerDataLastUpdateTime { get; set; }

		[JsonProperty("playerDataNextUpdateTime")]
		public DateTime? PlayerDataNextUpdateTime { get; set; }

		[JsonProperty("positions")]
		public List<ContestPosition> Positions { get; set; }

		[JsonProperty("sport")]
		public Sport Sport { get; set; }

		[JsonProperty("startTime")]
		public DateTime? StartTime { get; set; }
	}
}
