using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using LineupGenerator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LineupGenerator.Controller
{
	public class DataRetriever
	{
		#region Data Members

		private const string CONTEST_URL_FORMAT = "https://www.fanduel.com/e/Game/{0}";
		private static readonly IDictionary<string, string> ALTERNATE_NAMES = new Dictionary<string, string>
		{
			{ "Brad Beal", "Bradley Beal" },
			{ "Jose Juan Barea", "J.J. Barea" },
			{ "Luc Richard Mbah a Moute", "L. Mbah a Moute" },
			{ "Michael Carter-Williams", "M. Carter-Williams" },
			{ "Alexander Ovechkin", "Alex Ovechkin" }
		};
		private static readonly IDictionary<string, StatsInfo> STATS_INFOS = new Dictionary<string, StatsInfo>
		{
			{ "MLB", new StatsInfo
				{
					NameGroupIndex = 1,
					ProjectedGroupIndex = 3,
					RecentGroupIndex = 5,
					SeasonGroupIndex = 7,
					Regex = "<a href=\"/baseball/player.*?>(.*?)</a>(.*?<td){8}.*?>(.*?)</td>(.*?<td){2}.*?>(.*?)</td>(.*?<td){2}.*?>(.*?)</td>",
					URLRotoWire = "http://www.rotowire.com/daily/mlb/value-report.htm",
					URLNumberFire = "https://www.numberfire.com/mlb/fantasy/fantasy-baseball-projections"
				}
			},
			{ "NBA", new StatsInfo
				{
					NameGroupIndex = 1,
					ProjectedGroupIndex = 3,
					RecentGroupIndex = 5,
					SeasonGroupIndex = 7,
					Regex = "<a href=\"/basketball/player.*?>(.*?)</a>(.*?<td){6}.*?>(.*?)</td>(.*?<td){4}.*?>(.*?)</td>(.*?<td){4}.*?>(.*?)</td>",
					URLRotoWire = "http://www.rotowire.com/daily/nba/value-report.htm",
					URLNumberFire = "https://www.numberfire.com/nba/fantasy/fantasy-basketball-projections"
				}
			},
			{ "NFL", new StatsInfo
				{
					NameGroupIndex = 1,
					ProjectedGroupIndex = 3,
					RecentGroupIndex = 5,
					SeasonGroupIndex = 6,
					Regex = "<a href=\"/football/player.*?>(.*?)</a>(.*?<td){5}.*?>(.*?)</td>(.*?<td){2}.*?>(.*?)</td>.*?<td.*?>(.*?)</td>",
					URLRotoWire = "http://www.rotowire.com/daily/nfl/value-report.htm",
					URLNumberFire = "https://www.numberfire.com/nfl/fantasy/fantasy-football-projections"
				}
			},
			{ "NHL", new StatsInfo
				{
					NameGroupIndex = 1,
					ProjectedGroupIndex = 3,
					RecentGroupIndex = -1,
					SeasonGroupIndex = 5,
					Regex = "<a href=\"/hockey/player.*?>(.*?)</a>(.*?<td){5}.*?>(.*?)</td>(.*?<td){2}.*?>(.*?)</td>",
					URLRotoWire = "http://www.rotowire.com/daily/nhl/value-report.htm",
					URLNumberFire = "https://www.numberfire.com/nhl/daily-fantasy-hockey-projections"
				}
			}
		};
		private readonly string _gameID;

		#endregion

		#region Constructors

		public DataRetriever(string gameID)
		{
			_gameID = gameID;
		}

		#endregion

		#region Properties

		#region ContestType

		private string _contestType;
		public string ContestType
		{
			get
			{
				if (_contestType == null)
				{
					ReadContestData();
				}
				return _contestType;
			}
			private set { _contestType = value; }
		}

		#endregion

		#region MaxSalary

		private int? _maxSalary;
		public int MaxSalary
		{
			get
			{
				if (_maxSalary == null)
				{
					ReadContestData();
				}
				return _maxSalary.Value;
			}
			private set { _maxSalary = value; }
		}

		#endregion

		#region Players

		private IEnumerable<Player> _players;
		public IEnumerable<Player> Players
		{
			get
			{
				if (_players == null)
				{
					ReadContestData();
				}
				return _players;
			}
			private set { _players = value; }
		}

		#endregion

		#region PlayersStats

		private IDictionary<string, PlayerStats> _playersStats;
		public IDictionary<string, PlayerStats> PlayersStats
		{
			get
			{
				if (_playersStats == null)
				{
					ReadStatsData();
				}
				return _playersStats;
			}
			private set { _playersStats = value; }
		}

		#endregion

		#region Positions

		private IEnumerable<string> _positions;
		public IEnumerable<string> Positions
		{
			get
			{
				if (_positions == null)
				{
					ReadContestData();
				}
				return _positions;
			}
			private set { _positions = value; }
		}

		#endregion

		#endregion

		#region Contest Data Methods

		private void ReadContestData()
		{
			var data = DownloadContestData();
			ParseContestType(data);
			ParseMaxSalary(data);
			ParsePlayers(data);
			ParsePositions(data);
		}

		private string DownloadContestData()
		{
			using (var client = new WebClient())
			{
				return client.DownloadString(string.Format(CONTEST_URL_FORMAT, _gameID));
			}
		}

		private string ParseJSONString(string data, string key)
		{
			var regex = new Regex(key.Replace(".", @"\.") + @"\s*=\s*(.*);");
			var match = regex.Match(data);
			return match.Groups[1].Value;
		}

		private void ParseContestType(string data)
		{
			ContestType = ParseJSONString(data, "FD.playerpicker.competitionName")
				.Replace("'", "")
				.Replace("\"", "")
				.Split('.')[0];
		}

		private void ParseMaxSalary(string data)
		{
			var salaryData = ParseJSONString(data, "FD.playerpicker.salaryCap");
			MaxSalary = int.Parse(salaryData);
		}

		private void ParsePlayers(string data)
		{
			var playersData = ParseJSONString(data, "FD.playerpicker.allPlayersFullData");
			var playersDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(playersData);
			Players = playersDictionary.Select(p => new Player
			{
				Name = p.Value[1],
				Position = p.Value[0],
				Salary = int.Parse(p.Value[5]),
				SeasonAveragePoints = double.Parse(p.Value[6]),
				PlayerStats = GetPlayerStats(p.Value[1])
			}).ToArray();
		}

		private void ParsePositions(string data)
		{
			var positionsData = ParseJSONString(data, "FD.playerpicker.positions");
			var positionsList = JsonConvert.DeserializeObject<List<string>>(positionsData);
			Positions = positionsList.ToArray();
		}

		#endregion

		#region Stats Data Methods

		private void ReadStatsData()
		{
			StatsInfo info;
			if (STATS_INFOS.TryGetValue(ContestType, out info) && info != null)
			{
				var data = DownloadStatsData(info.URLRotoWire);
				ParsePlayersStatsFromRotoWire(info, data);
				data = DownloadStatsData(info.URLNumberFire);
				ParsePlayersStatsFromNumberFire(info, data);
			}
			else
			{
				PlayersStats = new Dictionary<string, PlayerStats>();
			}
		}

		private string DownloadStatsData(string url)
		{
			using (var client = new WebClient())
			{
				return client.DownloadString(url);
			}
		}

		private string ParseGroupValue(Match match, int index)
		{
			var groups = match == null ? null : match.Groups;
			if (groups != null && index >= 0 && index < groups.Count)
			{
				var group = groups[index];
				if (group != null)
				{
					return group.Value;
				}
			}
			return null;
		}

		private double ParseGroupDouble(Match match, int index)
		{
			var value = ParseGroupValue(match, index);
			double doubleValue;
			if (double.TryParse(value, out doubleValue))
			{
				return doubleValue;
			}
			return 0d;
		}

		private string ParsePlayerName(Match match, int index)
		{
			var name = ParseGroupValue(match, index);
			var parts = name.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
			return parts.Length == 2 ? parts[1] + " " + parts[0] : name;
		}

		private void ParsePlayersStatsFromRotoWire(StatsInfo info, string data)
		{
			var playersStats = new Dictionary<string, PlayerStats>();
			var regex = new Regex(info.Regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var match = regex.Match(data);
			while (match.Success)
			{
				var name = ParsePlayerName(match, info.NameGroupIndex);
				var stats = new PlayerStats
				{
					Name = name,
					RecentAveragePoints = ParseGroupDouble(match, info.RecentGroupIndex),
					ProjectedPoints = ParseGroupDouble(match, info.ProjectedGroupIndex),
					SeasonAveragePoints = ParseGroupDouble(match, info.SeasonGroupIndex)
				};
				playersStats[name] = stats;
				match = match.NextMatch();
			}
			PlayersStats = playersStats;
		}

		private IDictionary<string, string> ParseNumberFirePlayers(IEnumerable<KeyValuePair<string, JToken>> players)
		{
			return players.ToDictionary(pair => pair.Key, pair => (string)(pair.Value["name"]));
		}

		private void ParseNumberFireStats(StatsInfo info, IDictionary<string, string> players, JArray projections)
		{
			foreach (JObject projection in projections)
			{
				var playerID = (string)projection["nba_player_id"];
				var projectedPoints = (double)projection["fanduel_fp"];
				if (projectedPoints >= 0)
				{
					string playerName;
					if (players.TryGetValue(playerID, out playerName))
					{
						var stats = GetPlayerStats(playerName);
						if (stats == null)
						{
							PlayersStats[playerName] = new PlayerStats
							{
								Name = playerName,
								ProjectedPoints = projectedPoints
							};
						}
						else if (stats.ProjectedPoints <= 0)
						{
							stats.ProjectedPoints = projectedPoints;
						}
						else
						{
							stats.ProjectedPoints = (stats.ProjectedPoints + projectedPoints) / 2d;
						}
					}
				}
			}
		}

		private void ParsePlayersStatsFromNumberFire(StatsInfo info, string data)
		{
			var regex = new Regex(@"NF_DATA\s*=\s*(.*?);", RegexOptions.Singleline);
			var match = regex.Match(data);
			if (match.Success)
			{
				var json = ParseGroupValue(match, 1);
				var jsonObj = (JObject)JsonConvert.DeserializeObject(json);
				var players = ParseNumberFirePlayers((JObject)jsonObj["players"]);
				ParseNumberFireStats(info, players, (JArray)jsonObj["daily_projections"]);
			}
		}

		private PlayerStats GetPlayerStats(string name)
		{
			PlayerStats stats;
			if (PlayersStats.TryGetValue(name, out stats))
			{
				return stats;
			}
			string alternateName;
			if (ALTERNATE_NAMES.TryGetValue(name, out alternateName))
			{
				if (PlayersStats.TryGetValue(alternateName, out stats))
				{
					return stats;
				}
			}
			return null;
		}

		#endregion
	}
}
