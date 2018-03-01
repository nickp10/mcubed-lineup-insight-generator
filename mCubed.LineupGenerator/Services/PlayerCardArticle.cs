using System;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class PlayerCardArticle
	{
		[JsonProperty("date")]
		public DateTime? Date { get; set; }

		[JsonProperty("details")]
		public string Details { get; set; }

		[JsonProperty("summary")]
		public string Summary { get; set; }
	}
}
