using System.Collections.Generic;
using System.Linq;
using mCubed.Combinatorics;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.Controller
{
	public static class LineupGenerator
	{
		#region Constants

		public const int MAX_PLAYERS_TO_INCLUDE = 60;

		#endregion

		#region Methods

		public static IEnumerable<Lineup> GenerateLineups(ContestViewModel contest, IEnumerable<PlayerViewModel> includePlayers)
		{
			var maxSalary = contest.Contest.MaxSalary;
			var maxPlayersPerTeam = contest.Contest.MaxPlayersPerTeam;
			return GenerateLineupsForContest(contest, includePlayers).
				Where(l => l.TotalSalary <= maxSalary).
				Where(l => l.Players.GroupBy(p => p.Player.Team).All(g => g.Count() <= maxPlayersPerTeam)).
				Where(l => l.Players.Distinct().Count() == l.Players.Count);
		}

		private static IEnumerable<Lineup> GenerateLineupsForContest(ContestViewModel contest, IEnumerable<PlayerViewModel> includePlayers)
		{
			var positions = contest.Contest.Positions;
			if (includePlayers != null && positions != null)
			{
				var combinations = new List<Combinations<PlayerViewModel>>();
				foreach (var position in positions)
				{
					var playersNeededForPosition = 1;
					var possiblePlayersForPosition = includePlayers.Where(p => position.EligiblePlayerPositions.Contains(p.Player.Position)).ToList();
					combinations.Add(new Combinations<PlayerViewModel>(possiblePlayersForPosition, playersNeededForPosition));
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
