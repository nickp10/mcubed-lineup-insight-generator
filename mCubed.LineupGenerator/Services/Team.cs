using System.Collections.Generic;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class Team
	{
		[JsonProperty("code")]
		public string Code { get; set; }

		[JsonProperty("fullName")]
		public string FullName { get; set; }

		[JsonProperty("players")]
		public List<Player> Players { get; set; }
	}
}
