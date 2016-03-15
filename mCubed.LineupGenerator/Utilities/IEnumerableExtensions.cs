using System;
using System.Collections.Generic;
using System.Linq;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.Utilities
{
	public static class IEnumerableExtensions
	{
		#region Data Members

		public const double PROJECTED_POINTS_PERCENT = 0.5d;
		public const double RECENT_POINTS_PERCENT = 0.35d;
		public const double SEASON_POINTS_PERCENT = 0.15d;

		#endregion

		#region Methods

		public static void UpdateRating(this IEnumerable<Lineup> lineups, int count)
		{
			foreach (var lineup in lineups)
			{
				lineup.Rating = 100;
			}
			lineups.UpdateRating(l => l.TotalProjectedPoints, count, PROJECTED_POINTS_PERCENT);
			lineups.UpdateRating(l => l.TotalRecentAveragePoints, count, RECENT_POINTS_PERCENT);
			lineups.UpdateRating(l => l.TotalSeasonAveragePoints, count, SEASON_POINTS_PERCENT);
		}

		public static void UpdateRating(this IEnumerable<Lineup> lineups, Func<Lineup, double> lineupRating, int count, double percentage)
		{
			var lineupCount = 0;
			foreach (var lineup in lineups.OrderByDescending(lineupRating).Where(lineup => lineupRating(lineup) > 0))
			{
				var percentile = (double)lineupCount / (double)count;
				var proratedPercentile = percentile * percentage * 100;
				lineup.Rating -= proratedPercentile;
				lineupCount++;
			}
		}

		#endregion
	}
}
