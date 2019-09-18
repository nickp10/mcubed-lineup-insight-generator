using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class PositionPercentile
	{
		[JsonProperty("position")]
		public string Position { get; set; }

		[JsonProperty("percentile")]
		public double Percentile { get; set; }
	}
}
