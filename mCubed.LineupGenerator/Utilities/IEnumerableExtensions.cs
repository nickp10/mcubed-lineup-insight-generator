using System;
using System.Collections.Generic;
using System.Linq;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.Utilities
{
	public static class IEnumerableExtensions
	{
		#region Data Members

		public const double DEFAULT_RATING_TOLERANCE = 0.025d;

		#endregion

		#region Methods

		public static void AddRating(this IEnumerable<Lineup> lineups, Func<Lineup, double> lineupRating, double ratingTolerance = DEFAULT_RATING_TOLERANCE)
		{
			lineups.AddRating(lineupRating, lineups.Count(), ratingTolerance);
		}

		public static void AddRating(this IEnumerable<Lineup> lineups, Func<Lineup, double> lineupRating, int count, double ratingTolerance = DEFAULT_RATING_TOLERANCE)
		{
			var lineupCount = 0;
			var topLineupEndCount = Math.Max(1, (int)Math.Floor(count * ratingTolerance));
			var bottomLineupStartCount = topLineupEndCount * 3;
			var bottomLineupEndCount = bottomLineupStartCount * 2;
			foreach (var lineup in lineups.OrderByDescending(lineupRating).Where(lineup => lineupRating(lineup) > 0))
			{
				lineup.Rating += (lineupCount < topLineupEndCount) ? 3 : ((lineupCount < bottomLineupStartCount) ? 2 : 1);
				lineupCount++;
				if (lineupCount >= bottomLineupEndCount)
				{
					break;
				}
			}
		}

		#endregion
	}
}
