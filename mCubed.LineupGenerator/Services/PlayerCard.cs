using System.Collections.Generic;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class PlayerCard
	{
		[JsonProperty("gameLog")]
		public List<PlayerCardGameStats> GameLog { get; set; }

		[JsonProperty("news")]
		public List<PlayerCardArticle> News { get; set; }
	}
}
