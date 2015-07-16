using System.Collections.Generic;
using System.Linq;
using mCubed.Combinatorics;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.Controller
{
	public static class LineupGenerator
	{
		#region Methods

		public static IEnumerable<Lineup> GenerateLineups(Contest contest)
		{
			var maxSalary = contest.MaxSalary;
			var maxPlayersPerTeam = contest.MaxPlayersPerTeam;
			return GenerateLineupsForContest(contest).
				Where(l => l.TotalSalary <= maxSalary).
				Where(l => l.Players.GroupBy(p => p.Team).All(g => g.Count() <= maxPlayersPerTeam)).
				OrderByDescending(l => l.TotalSalary);
		}

		private static IEnumerable<Lineup> GenerateLineupsForContest(Contest contest)
		{
			var players = contest.Players;
			var positions = contest.Positions;
			if (players != null && positions != null)
			{
				var combinations = new List<Combinations<Player>>();
				foreach (var positionGroup in positions.GroupBy(p => p))
				{
					var position = positionGroup.Key;
					var playersNeededForPosition = positionGroup.Count();
					var possiblePlayersForPosition = players.Where(p => p.IncludeInLineups && p.Position == position).ToList();
					combinations.Add(new Combinations<Player>(possiblePlayersForPosition, playersNeededForPosition));
				}
				var totalLineups = combinations.Select(c => c.Count).Aggregate((c1, c2) => c1 * c2);
				for (long i = 0; i < totalLineups; i++)
				{
					var index = i;
					var lineup = new Lineup();
					foreach (var combination in combinations)
					{
						var positionPlayers = combination.ElementAt((int)(index % combination.Count));
						foreach (var positionPlayer in positionPlayers)
						{
							lineup.Players.Add(positionPlayer);
						}
						index = index / combination.Count;
					}
					if (lineup.Players.Count == positions.Count())
					{
						yield return lineup;
					}
				}
			}
		}

		#endregion
	}
}
