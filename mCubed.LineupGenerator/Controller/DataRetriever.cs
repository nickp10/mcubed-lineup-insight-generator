using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using mCubed.LineupGenerator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mCubed.LineupGenerator.Controller
{
	public class DataRetriever : INotifyPropertyChanged
	{
		#region Data Members

		private const string CONTEST_URL_FORMAT = "https://www.fanduel.com/e/Game/{0}";
		private static readonly IDictionary<string, string> ALTERNATE_NAMES = new Dictionary<string, string>
		{
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
		private static readonly IDictionary<string, StatsInfo> STATS_INFOS = new Dictionary<string, StatsInfo>
		{
			{ "MLB", new StatsInfo
				{
					NameGroupIndex = 1,
					ProjectedGroupIndex = 3,
					RecentGroupIndex = 5,
					SeasonGroupIndex = 7,
					Regex = "<a href=\"/baseball/player.*?>(.*?)</a>(.*?<td){8}.*?>(.*?)</td>(.*?<td){2}.*?>(.*?)</td>(.*?<td){2}.*?>(.*?)</td>",
					NumberFirePlayers = "players",
					NumberFirePlayerID = "mlb_player_id",
					NumberFireProjections = "projections",
					NumberFireProjectedPoints = "fanduel_fp",
					URLRotoWire = "http://www.rotowire.com/daily/mlb/value-report.htm",
					URLNumberFire = new string[]
					{
						"https://www.numberfire.com/mlb/fantasy/fantasy-baseball-projections/batters",
						"https://www.numberfire.com/mlb/fantasy/fantasy-baseball-projections/pitchers"
					},
					InjuryMappings = new Dictionary<string, InjuryData>
					{
						{ "", new InjuryData("DL", InjuryType.Out) },
						{ "day-to-day", new InjuryData("DTD", InjuryType.Possible) },
						{ "out", new InjuryData("O", InjuryType.Out) },
						{ "suspension", new InjuryData("NA", InjuryType.Out) },
						{ "15-day dl", new InjuryData("DL", InjuryType.Out) },
						{ "60-day dl", new InjuryData("DL", InjuryType.Out) },
						{ "7-day dl", new InjuryData("DL", InjuryType.Out) }
					}
				}
			},
			{ "NBA", new StatsInfo
				{
					NameGroupIndex = 1,
					ProjectedGroupIndex = 3,
					RecentGroupIndex = 5,
					SeasonGroupIndex = 7,
					Regex = "<a href=\"/basketball/player.*?>(.*?)</a>(.*?<td){6}.*?>(.*?)</td>(.*?<td){4}.*?>(.*?)</td>(.*?<td){4}.*?>(.*?)</td>",
					NumberFirePlayers = "players",
					NumberFirePlayerID = "nba_player_id",
					NumberFireProjections = "daily_projections",
					NumberFireProjectedPoints = "fanduel_fp",
					URLRotoWire = "http://www.rotowire.com/daily/nba/value-report.htm",
					URLNumberFire = new string[]
					{
						"https://www.numberfire.com/nba/fantasy/fantasy-basketball-projections"
					},
					InjuryMappings = new Dictionary<string, InjuryData>
					{
						{ "", new InjuryData("IR", InjuryType.Out) },
						{ "gtd", new InjuryData("GTD", InjuryType.Possible) },
						{ "out", new InjuryData("O", InjuryType.Out) },
						{ "suspension", new InjuryData("NA", InjuryType.Out) }
					}
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
					URLNumberFire = new string[]
					{
						//"https://www.numberfire.com/nfl/fantasy/fantasy-football-projections"
					},
					InjuryMappings = new Dictionary<string, InjuryData>
					{
						{ "", new InjuryData("IR", InjuryType.Out) },
						{ "doubtful", new InjuryData("D", InjuryType.Possible) },
						{ "out", new InjuryData("O", InjuryType.Out) },
						{ "questionable", new InjuryData("Q", InjuryType.Possible) },
						{ "probable", new InjuryData("P", InjuryType.Probable) },
						{ "suspension", new InjuryData("NA", InjuryType.Out) },
						{ "inactive", new InjuryData("NA", InjuryType.Out) },
						{ "pup-p", new InjuryData("NA", InjuryType.Out) },
						{ "pup-r", new InjuryData("NA", InjuryType.Out) },
						{ "ir", new InjuryData("IR", InjuryType.Out) },
						{ "ir-r", new InjuryData("IR", InjuryType.Out) }
					}
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
					InjuryMappings = new Dictionary<string, InjuryData>
					{
						{ "", new InjuryData("IR", InjuryType.Out) },
						{ "day-to-day", new InjuryData("DTD", InjuryType.Possible) },
						{ "out", new InjuryData("O", InjuryType.Out) },
						{ "dl", new InjuryData("IR", InjuryType.Out) },
						{ "ir", new InjuryData("IR", InjuryType.Out) },
						{ "suspension", new InjuryData("NA", InjuryType.Out) }
					}
				}
			}
		};

		#endregion

		#region Constructors

		public DataRetriever()
		{
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

		#region GameID

		private string _gameID;
		public string GameID
		{
			get { return _gameID; }
			set
			{
				if (_gameID != value)
				{
					_gameID = value;
					RaisePropertyChanged("GameID");
				}
			}
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

		#region StatlessPlayers

		private ObservableCollection<string> _statlessPlayers;
		public ObservableCollection<string> StatlessPlayers
		{
			get
			{
				if (_statlessPlayers == null)
				{
					_statlessPlayers = new ObservableCollection<string>();
				}
				return _statlessPlayers;
			}
		}

		#endregion

		#region ZeroesFromNumberFire

		private ObservableCollection<string> _zeroesFromNumberFire;
		public ObservableCollection<string> ZeroesFromNumberFire
		{
			get
			{
				if (_zeroesFromNumberFire == null)
				{
					_zeroesFromNumberFire = new ObservableCollection<string>();
				}
				return _zeroesFromNumberFire;
			}
		}

		#endregion

		#region ZeroesFromRotoWire

		private ObservableCollection<string> _zeroesFromRotoWire;
		public ObservableCollection<string> ZeroesFromRotoWire
		{
			get
			{
				if (_zeroesFromRotoWire == null)
				{
					_zeroesFromRotoWire = new ObservableCollection<string>();
				}
				return _zeroesFromRotoWire;
			}
		}

		#endregion

		#endregion

		#region Methods

		public void Clear()
		{
			ContestType = null;
			_maxSalary = null;
			Players = null;
			PlayersStats = null;
			Positions = null;
			Application.Current.Dispatcher.Invoke(new Action(() =>
			{
				StatlessPlayers.Clear();
				ZeroesFromNumberFire.Clear();
				ZeroesFromRotoWire.Clear();
			}));
		}

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
				return client.DownloadString(string.Format(CONTEST_URL_FORMAT, GameID));
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

		private InjuryData ParsePlayerInjury(StatsInfo info, int injuryStatus, string injury)
		{
			if ((injuryStatus & 2) == 0)
			{
				return null;
			}
			InjuryData injuryData;
			info.InjuryMappings.TryGetValue(injury.ToLower(), out injuryData);
			return injuryData;
		}

		private bool IsProbablePitcher(StatsInfo info, int injuryStatus)
		{
			return (injuryStatus & 1) != 0;
		}

		private void ParsePlayers(string data)
		{
			StatsInfo info;
			if (STATS_INFOS.TryGetValue(ContestType, out info) && info != null)
			{
				var playersData = ParseJSONString(data, "FD.playerpicker.allPlayersFullData");
				var playersDictionary = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(playersData);
				Players = playersDictionary.Select(p => new Player
				{
					Name = p.Value[1],
					Position = p.Value[0],
					Salary = int.Parse(p.Value[5]),
					SeasonAveragePoints = double.Parse(p.Value[6]),
					PlayerStats = GetPlayerStats(p.Value[1]),
					Injury = ParsePlayerInjury(info, int.Parse(p.Value[9]), p.Value[12]),
					IsProbablePitcher = IsProbablePitcher(info, int.Parse(p.Value[9]))
				}).Where(p => p.Position != "P" || p.IsProbablePitcher).ToArray();
			}
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
				if (!string.IsNullOrEmpty(info.URLRotoWire))
				{
					var data = DownloadStatsData(info.URLRotoWire);
					ParsePlayersStatsFromRotoWire(info, data);
				}
				else
				{
					PlayersStats = new Dictionary<string, PlayerStats>();
				}
				if (info.URLNumberFire != null)
				{
					foreach (var url in info.URLNumberFire)
					{
						var data = DownloadStatsData(url);
						ParsePlayersStatsFromNumberFire(info, data);
					}
				}
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
				if (stats.ProjectedPoints <= 0 && !ZeroesFromRotoWire.Contains(name))
				{
					Application.Current.Dispatcher.Invoke(new Action(() =>
					{
						ZeroesFromRotoWire.Add(name);
					}));
				}
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
				var playerID = (string)projection[info.NumberFirePlayerID];
				var projectedPoints = (double)projection[info.NumberFireProjectedPoints];
				if (projectedPoints >= 0)
				{
					string playerName;
					if (players.TryGetValue(playerID, out playerName))
					{
						var stats = GetPlayerStats(playerName);
						if (stats == null)
						{
							stats = new PlayerStats
							{
								Name = playerName,
								ProjectedPoints = projectedPoints
							};
							PlayersStats[playerName] = stats;
						}
						else if (stats.ProjectedPoints <= 0)
						{
							stats.ProjectedPoints = projectedPoints;
						}
						else if (projectedPoints > 0)
						{
							stats.ProjectedPoints = (stats.ProjectedPoints + projectedPoints) / 2d;
						}
						if (projectedPoints <= 0 && !ZeroesFromNumberFire.Contains(stats.Name))
						{
							Application.Current.Dispatcher.Invoke(new Action(() =>
							{
								ZeroesFromNumberFire.Add(stats.Name);
							}));
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
				var players = ParseNumberFirePlayers((JObject)jsonObj[info.NumberFirePlayers]);
				ParseNumberFireStats(info, players, (JArray)jsonObj[info.NumberFireProjections]);
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
			var lastNameIndex = name.LastIndexOf(' ');
			var lastNameFirst = name.Substring(lastNameIndex + 1) + ", " + name.Substring(0, lastNameIndex);
			if (PlayersStats.TryGetValue(lastNameFirst, out stats))
			{
				return stats;
			}
			if (!StatlessPlayers.Contains(name))
			{
				Application.Current.Dispatcher.Invoke(new Action(() =>
				{
					StatlessPlayers.Add(name);
				}));
			}
			return null;
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(property));
			}
		}

		#endregion
	}
}
