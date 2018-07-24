using System.Collections.Generic;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class ContestPosition
	{
		[JsonProperty("eligiblePlayerPositions")]
		public List<string> EligiblePlayerPositions { get; set; }

		[JsonProperty("label")]
		public string Label { get; set; }
	}
}
