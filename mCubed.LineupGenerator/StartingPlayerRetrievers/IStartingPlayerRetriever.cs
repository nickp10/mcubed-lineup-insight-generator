using System.Collections.Generic;

namespace mCubed.LineupGenerator.StartingPlayerRetrievers
{
	public interface IStartingPlayerRetriever
	{
		IEnumerable<string> RetrieveStartingPlayers { get; }
	}
}
