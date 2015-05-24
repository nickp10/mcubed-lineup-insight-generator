using System.Collections.Generic;
using System.Linq;
using mCubed.Combinatorics;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.Controller
{
	public class LineupGenerator
	{
		#region Constructors

		public LineupGenerator(params Player[] players)
		{
			AllPlayers = players ?? Enumerable.Empty<Player>();
		}

		#endregion

		#region Properties

		#region AllPlayers

		public IEnumerable<Player> AllPlayers { get; set; }

		#endregion

		#endregion

		#region Methods

		public IEnumerable<Lineup> GenerateLineups(IEnumerable<string> positions, int maxSalary, int maxPlayersPerTeam)
		{
			return GenerateLineups(positions).Where(l => l.TotalSalary <= maxSalary).Where(l => l.Players.GroupBy(p => p.Team).All(g => g.Count() <= maxPlayersPerTeam)).OrderByDescending(l => l.TotalSalary);
		}

		private IEnumerable<Lineup> GenerateLineups(IEnumerable<string> positions)
		{
			var combinations = new List<Combinations<Player>>();
			foreach (var positionGroup in positions.GroupBy(p => p))
			{
				var position = positionGroup.Key;
				var playersNeededForPosition = positionGroup.Count();
				var possiblePlayersForPosition = AllPlayers.Where(p => p.IncludeInLineups && p.Position == position).ToList();
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
				yield return lineup;
			}
		}

		#endregion
	}
}
