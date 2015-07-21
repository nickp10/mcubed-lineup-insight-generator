﻿using System;
using System.Collections.Generic;
using System.Linq;
using mCubed.LineupGenerator.ContestRetrievers;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.StartingPlayerRetrievers;
using mCubed.LineupGenerator.StatRetrievers;
using mCubed.LineupGenerator.Utilities;

namespace mCubed.LineupGenerator.Controller
{
	public static class DataRetriever
	{
		#region Data Members

		/// <summary>
		/// Key - The name from the stat URLs.
		/// Value - The name from the contest URL.
		/// </summary>
		private static readonly IDictionary<string, string> _alternateNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			/* MLB */
			{ "Alejandro Aza", "Alejandro De Aza" },
			{ "Alex Guerrero", "Alexander Guerrero" },
			{ "Alex Torres", "Alexander Torres" },
			{ "B.J. Upton", "Melvin Upton" },
			{ "Brad Boxberger", "Bradley Boxberger" },
			{ "Dan Murphy", "Daniel Murphy" },
			{ "Danny Santana", "Daniel Santana" },
			{ "Delino Jr.", "Delino Deshields Jr." },
			{ "D.J. LeMahieu", "DJ LeMahieu" },
			{ "Drew Hutchison", "Andrew Hutchison" },
			{ "Ivan DeJesus", "Ivan De Jesus" },
			{ "Jake Elmore", "Jacob Elmore" },
			{ "Jake Lamb", "Jacob Lamb" },
			{ "Jake Marisnick", "Jacob Marisnick" },
			{ "Jake Petricka", "Jacob Petricka" },
			{ "James Happ", "J.A. Happ" },
			{ "John Ryan Murphy", "John Murphy" },
			{ "Jon Herrera", "Jonathan Herrera" },
			{ "J.R. Murphy", "John Murphy" },
			{ "J.T. Realmuto", "Jacob Realmuto" },
			{ "JT Realmuto", "Jacob Realmuto" },
			{ "Jumbo Diaz", "Jose Diaz" },
			{ "Jung Ho Kang", "Jung-ho Kang" },
			{ "Jung-Ho Kang", "Jung-ho Kang" },
			{ "Kike Hernandez", "Enrique Hernandez" },
			{ "Matt Dekker", "Matt den Dekker" },
			{ "Matt Stites", "Matthew Stites" },
			{ "Matthew Duffy", "Matt Duffy" },
			{ "Matthew Joyce", "Matt Joyce" },
			{ "Melvin Upton Jr.", "Melvin Upton" },
			{ "Michael A. Taylor", "Michael Taylor" },
			{ "Michael Zunino", "Mike Zunino" },
			{ "Mike Bolsinger", "Michael Bolsinger" },
			{ "Mike Dunn", "Michael Dunn" },
			{ "Mike McKenry", "Michael McKenry" },
			{ "Mike Morin", "Michael Morin" },
			{ "Nate Karns", "Nathan Karns" },
			{ "Nick Martinez", "Nicholas Martinez" },
			{ "Patrick Neshek", "Pat Neshek" },
			{ "Rubby Rosa", "Rubby de la Rosa" },
			{ "Ryan Tepera", "Dennis Tepera" },
			{ "Sam Tuivailala", "Samuel Tuivailala" },
			{ "Steven Pearce", "Steve Pearce" },
			{ "Steven Souza", "Steve Souza" },
			{ "Tommy Milone", "Tom Milone" },
			{ "Will Smith", "William Smith" },

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
						new RotoWireStatRetriever("<a.*?baseball/player.*?>(.*?)</(.*?<td){3}.*?>(.*?)</td>(.*?<td){6}.*?>(.*?)</td>", 1, 5, 3,
							"http://www.rotowire.com/daily/mlb/optimizer.htm"),
						new NumberFireStatRetriever("players", "mlb_player_id", "projections", "fanduel_fp",
							"https://www.numberfire.com/mlb/fantasy/fantasy-baseball-projections/batters",
							"https://www.numberfire.com/mlb/fantasy/fantasy-baseball-projections/pitchers"),
						// Batters - Last 7 days
						new RotoGuruStatRetriever("http://rotoguru1.com/cgi-bin/stats.cgi?pos=8&sort=4&game=d&colA=0&daypt=3&denom=3&xavg=4&inact=0&maxprc=99999&sched=0&starters=0&hithand=0&numlist=c"),
						// Pitchers - Last 30 days
						new RotoGuruStatRetriever("http://rotoguru1.com/cgi-bin/stats.cgi?pos=1&sort=4&game=d&colA=0&daypt=1&denom=3&xavg=4&inact=0&maxprc=99999&sched=0&starters=0&hithand=0&numlist=c")
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

		private static void MergePlayer(IDictionary<string, Player> players, Player player)
		{
			var targetPlayer = GetPlayer(players, player.Name);
			if (targetPlayer != null)
			{
				var sourceStats = player.Stats;
				if (sourceStats != null)
				{
					var targetStats = targetPlayer.Stats;
					if (targetStats == null)
					{
						targetPlayer.Stats = sourceStats;
					}
					else
					{
						targetPlayer.Stats = targetStats.Concat(sourceStats).ToArray();
					}
				}
				if (Utils.IsBattingOrder(player.BattingOrder) && !Utils.IsBattingOrder(targetPlayer.BattingOrder))
				{
					targetPlayer.BattingOrder = player.BattingOrder;
				}
				if (!string.IsNullOrWhiteSpace(player.ESPNID) && string.IsNullOrWhiteSpace(targetPlayer.ESPNID))
				{
					targetPlayer.ESPNID = player.ESPNID;
				}
			}
		}

		private static IEnumerable<Player> ReadStatsData(Contest contest)
		{
			StatsInfo info;
			if (_statsInfos.TryGetValue(contest.Sport, out info) && info != null)
			{
				return info.StatRetrievers.SelectMany(s => s.RetrieveStats).ToArray();
			}
			return Enumerable.Empty<Player>();
		}

		public static void ParseStats(Contest contest)
		{
			var players = contest.PlayersDictionary;
			foreach (var player in ReadStatsData(contest))
			{
				MergePlayer(players, player);
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
				var targetPlayer = GetPlayer(players, player);
				if (targetPlayer != null)
				{
					targetPlayer.IsStarter = true;
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
			return null;
		}

		#endregion
	}
}
