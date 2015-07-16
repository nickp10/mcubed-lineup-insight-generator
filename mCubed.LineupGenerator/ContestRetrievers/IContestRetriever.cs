using System;
using System.Collections.Generic;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.ContestRetrievers
{
	public interface IContestRetriever
	{
		Type ContestType { get; }
		IEnumerable<Contest> Contests { get; }

		/// <summary>
		/// Called to fill in additional contest information. By default, a contest should contain
		/// as minimal information as possible to prevent loading unneeded data. Once a contest is
		/// selected, the rest of the information should be retrieved as the data will be needed.
		/// This method will be called when the contest is selected to fill in the additional data.
		/// </summary>
		/// <param name="contest">The contest that was selected.</param>
		void FillAdditionalContestData(Contest contest);
	}
}
