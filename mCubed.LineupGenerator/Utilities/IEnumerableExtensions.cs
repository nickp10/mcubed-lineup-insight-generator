using System.Collections.Generic;
using LineupGenerator.Model;
using System.Linq;
using System;

namespace LineupGenerator.Utilities
{
	public static class IEnumerableExtensions
	{
		#region Methods

		public static void AddRating(this IEnumerable<Lineup> lineups, Func<Lineup, double> lineupRating, int threshold)
		{
			var lineupCount = 0;
			var topThreshold = (int)Math.Floor(threshold * .05d);
			var middleThreshold = (int)Math.Floor(threshold * .25d);
			foreach (var lineup in lineups.OrderByDescending(lineupRating).Where(lineup => lineupRating(lineup) > 0).Take(threshold))
			{
				lineup.Rating += (lineupCount <= topThreshold) ? 3 : ((lineupCount <= middleThreshold) ? 2 : 1);
				lineupCount++;
			}
		}

		#endregion
	}
}
