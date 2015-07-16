using System.Collections.Generic;
using System.Linq;
using mCubed.LineupGenerator.ContestRetrievers;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.StartingPlayerRetrievers;
using mCubed.LineupGenerator.StatRetrievers;

namespace mCubed.LineupGenerator.Controller
{
	public static class DataRetriever
	{
		#region Data Members

		/// <summary>
		/// Key - The name from the stat URLs.
		/// Value - The name from the contest URL.
		/// </summary>
		private static readonly IDictionary<string, string> _alternateNames = new Dictionary<string, string>
		{
			/* MLB */
			{ "Alex Guerrero", "Alexander Guerrero" },
			{ "B.J. Upton", "Melvin Upton" },
			{ "Delino DeShields Jr.", "Delino Deshields Jr." },
			{ "Jake Lamb", "Jacob Lamb" },
			{ "J.T. Realmuto", "Jacob Realmuto" },
			{ "JT Realmuto", "Jacob Realmuto" },
			{ "Jung Ho Kang", "Jung-ho Kang" },
			{ "Jung-Ho Kang", "Jung-ho Kang" },
			{ "Melvin Upton Jr.", "Melvin Upton" },
			{ "Mike McKenry", "Michael McKenry" },
			{ "Steven Souza", "Steve Souza" },

			/* NBA */
			{ "Bill Walker", "Henry Walker" },
			{ "Brad Beal", "Bradley Beal" },
			{ "Ishmael Smith", "Ish Smith" },
			{ "JaKarr Sampson", "Jakarr Sampson" },
			{ "James Michael McAdoo", "James McAdoo" },
			{ "Jeffery Taylor", "Jeff Taylor" },
			{ "Jose Juan Barea", "J.J. Barea" },
			{ "Juan Jose Barea", "J.J. Barea" },
			{ "Kentavious Caldwell-Pope", "K. Caldwell-Pope" },
			{ "Luc Mbah a Moute", "L. Mbah a Moute" },
			{ "Luc Richard Mbah a Moute", "L. Mbah a Moute" },
			{ "Michael Carter-Williams", "M. Carter-Williams" },
			{ "Michael Kidd-Gilchrist", "M. Kidd-Gilchrist" },
			{ "Patrick Mills", "Patty Mills" },
			{ "Perry Jones III", "Perry Jones" },
			{ "Phil (Flip) Pressey", "Phil Pressey" },
			{ "Roy Devyn Marble", "Devyn Marble" },
			{ "Tim Hardaway Jr.", "Tim Hardaway" },

			/* NHL */
			{ "Alexander Ovechkin", "Alex Ovechkin" }
		};

		private static readonly IContestRetriever[] _contestRetrievers = new IContestRetriever[]
		{
			new FanDuelContestRetriever()
		};

		private static readonly IDictionary<string, StatsInfo> _statsInfos = new Dictionary<string, StatsInfo>
		{
			{ "MLB", new StatsInfo
				{
					StatRetrievers = new IStatRetriever[]
					{
						new RotoWireStatRetriever("<a.*?baseball/player.*?>(.*?)</(.*?<td){9}.*?>(.*?)</td>", 1, 3,
							"http://www.rotowire.com/daily/mlb/optimizer.htm"),
						new NumberFireStatRetriever("players", "mlb_player_id", "projections", "fanduel_fp",
							"https://www.numberfire.com/mlb/fantasy/fantasy-baseball-projections/batters",
							"https://www.numberfire.com/mlb/fantasy/fantasy-baseball-projections/pitchers")
					},
					StartingPlayerRetriever = new RotoWireStartingPlayerRetriever("http://www.rotowire.com/baseball/daily_lineups.htm",
						@":\s<.*?baseball/player\..*?>(.*?)</",
						@"title=""([\w\s.-]*)"".*?baseball/player\.")
				}
			},
			{ "NBA", new StatsInfo
				{
					StatRetrievers = new IStatRetriever[]
					{
						new RotoWireStatRetriever("<a.*?basketball/player.*?>(.*?)</(.*?<td){5}.*?>(.*?)</td>", 1, 3,
							"http://www.rotowire.com/daily/nba/optimizer.htm"),
						new NumberFireStatRetriever("players", "nba_player_id", "daily_projections", "fanduel_fp",
							"https://www.numberfire.com/nba/fantasy/fantasy-basketball-projections")
					},
					StartingPlayerRetriever = new RotoWireStartingPlayerRetriever("http://www.rotowire.com/basketball/nba_lineups.htm",
						@"title=""([\w\s.-]*)"".*?basketball/player\.")
				}
			},
			{ "NFL", new StatsInfo
				{
					StatRetrievers = new IStatRetriever[]
					{
						new RotoWireStatRetriever("<a.*?football/player.*?>(.*?)</(.*?<td){5}.*?>(.*?)</td>", 1, 3,
							"http://www.rotowire.com/daily/nfl/optimizer.htm")
						/*new NumberFireStatRetriever("players", "nfl_player_id", "projections", "fanduel_fp",
							"https://www.numberfire.com/nfl/fantasy/fantasy-football-projections")*/
					},
					StartingPlayerRetriever = new RotoWireStartingPlayerRetriever("http://www.rotowire.com/football/nfl_lineups.htm",
						@"(?<!inactiveblock.*?)football/player\..*?>(.*?)</")
				}
			},
			{ "NHL", new StatsInfo
				{
					StatRetrievers = new IStatRetriever[]
					{
						new RotoWireStatRetriever("<a.*?hockey/player.*?>(.*?)</(.*?<td){5}.*?>(.*?)</td>", 1, 3,
							"http://www.rotowire.com/daily/nhl/optimizer.htm")
						/*new NumberFireStatRetriever("players", "nhl_player_id", "projections", "fanduel_fp",
							"https://www.numberfire.com/nhl/daily-fantasy-hockey-projections/skaters",
							"https://www.numberfire.com/nhl/daily-fantasy-hockey-projections/goalies")*/
					},
					StartingPlayerRetriever = new RotoWireStartingPlayerRetriever("http://www.rotowire.com/hockey/nhl_lineups.htm",
						@"goalie-tag.*?hockey/player\..*?>(.*?)</",
						@"title=""([\w\s.-]*)"".*?hockey/player\.")
				}
			}
		};

		#endregion

		#region Properties

		#region ContestRetrievers

		public static IEnumerable<IContestRetriever> ContestRetrievers
		{
			get { return _contestRetrievers; }
		}

		#endregion

		#endregion

		#region Methods

		private static IEnumerable<PlayerStats> ReadStatsData(Contest contest)
		{
			StatsInfo info;
			if (_statsInfos.TryGetValue(contest.Sport, out info) && info != null)
			{
				return info.StatRetrievers.SelectMany(s => s.RetrieveStats).ToArray();
			}
			return Enumerable.Empty<PlayerStats>();
		}

		public static void ParseStats(Contest contest)
		{
			var players = contest.PlayersDictionary;
			foreach (var stats in ReadStatsData(contest))
			{
				var player = GetPlayer(players, stats.Name);
				if (player != null)
				{
					var existingStats = player.Stats;
					if (existingStats == null)
					{
						player.Stats = new[] { stats };
					}
					else
					{
						player.Stats = existingStats.Concat(new[] { stats }).ToArray();
					}
				}
			}
		}

		private static IEnumerable<string> ReadStartingPlayers(Contest contest)
		{
			StatsInfo info;
			if (_statsInfos.TryGetValue(contest.Sport, out info) && info != null)
			{
				return info.StartingPlayerRetriever.RetrieveStartingPlayers;
			}
			return Enumerable.Empty<string>();
		}

		public static void ParseStartingPlayers(Contest contest)
		{
			var players = contest.PlayersDictionary;
			foreach (var player in ReadStartingPlayers(contest))
			{
				var p = GetPlayer(players, player);
				if (p != null)
				{
					p.IsStarter = true;
				}
			}
		}

		private static Player GetPlayer(IDictionary<string, Player> players, string name)
		{
			Player player;
			if (players.TryGetValue(name, out player))
			{
				return player;
			}
			string alternateName;
			if (_alternateNames.TryGetValue(name, out alternateName))
			{
				if (players.TryGetValue(alternateName, out player))
				{
					return player;
				}
			}
			var lastNameIndex = name.LastIndexOf(' ');
			var lastNameFirst = name.Substring(lastNameIndex + 1) + ", " + name.Substring(0, lastNameIndex);
			if (players.TryGetValue(lastNameFirst, out player))
			{
				return player;
			}
			return null;
		}

		#endregion
	}
}
