using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class PlayerMLBSpecific
	{
		[JsonProperty("battingOrder")]
		public string BattingOrder { get; set; }

		[JsonProperty("handednessBat")]
		public string HandednessBat { get; set; }

		[JsonProperty("handednessThrow")]
		public string HandednessThrow { get; set; }

		[JsonProperty("isProbablePitcher")]
		public bool IsProbablePitcher { get; set; }
	}
}
