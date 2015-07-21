using System.Collections.Generic;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.StatRetrievers
{
	public interface IStatRetriever
	{
		IEnumerable<Player> RetrieveStats { get; }
	}
}
