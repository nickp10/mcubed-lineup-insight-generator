using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class PlayerInjury
	{
		[JsonProperty("display")]
		public string Display { get; set; }

		[JsonProperty("injuryType")]
		public InjuryType InjuryType { get; set; }
	}
}
