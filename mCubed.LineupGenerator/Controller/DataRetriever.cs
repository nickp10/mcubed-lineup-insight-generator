using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.StartingPlayerRetrievers;
using mCubed.LineupGenerator.StatRetrievers;
using mCubed.LineupGenerator.Utilities;
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
					StatRetrievers = new IStatRetriever[]
					{
						new RotoWireStatRetriever("<a.*?baseball/player.*?>(.*?)</(.*?<td){5}.*?>(.*?)</td>", 1, 3,
							"http://www.rotowire.com/daily/mlb/optimizer.htm"),
						new NumberFireStatRetriever("players", "mlb_player_id", "projections", "fanduel_fp",
							"https://www.numberfire.com/mlb/fantasy/fantasy-baseball-projections/batters",
							"https://www.numberfire.com/mlb/fantasy/fantasy-baseball-projections/pitchers")
					},
					StartingPlayerRetriever = new RotoWireStartingPlayerRetriever("http://www.rotowire.com/baseball/daily_lineups.htm",
						@":\s<.*?baseball/player\..*?>(.*?)</",
						@"title=""([\w\s.-]*)"".*?baseball/player\."),
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
					StatRetrievers = new IStatRetriever[]
					{
						new RotoWireStatRetriever("<a.*?basketball/player.*?>(.*?)</(.*?<td){5}.*?>(.*?)</td>", 1, 3,
							"http://www.rotowire.com/daily/nba/optimizer.htm"),
						new NumberFireStatRetriever("players", "nba_player_id", "daily_projections", "fanduel_fp",
							"https://www.numberfire.com/nba/fantasy/fantasy-basketball-projections")
					},
					StartingPlayerRetriever = new RotoWireStartingPlayerRetriever("http://www.rotowire.com/basketball/nba_lineups.htm",
						@"title=""([\w\s.-]*)"".*?basketball/player\."),
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
					StatRetrievers = new IStatRetriever[]
					{
						new RotoWireStatRetriever("<a.*?football/player.*?>(.*?)</(.*?<td){5}.*?>(.*?)</td>", 1, 3,
							"http://www.rotowire.com/daily/nfl/optimizer.htm")
						/*new NumberFireStatRetriever("players", "nfl_player_id", "projections", "fanduel_fp",
							"https://www.numberfire.com/nfl/fantasy/fantasy-football-projections")*/
					},
					StartingPlayerRetriever = new RotoWireStartingPlayerRetriever("http://www.rotowire.com/football/nfl_lineups.htm",
						@"(?<!inactiveblock.*?)football/player\..*?>(.*?)</"),
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
						@"title=""([\w\s.-]*)"".*?hockey/player\."),
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

		#region MaxPlayersPerTeam

		public int MaxPlayersPerTeam
		{
			get { return 4; }
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

		private IDictionary<string, Player> _players;
		public IDictionary<string, Player> Players
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

		#region Teams

		private IDictionary<string, string> _teams;
		public IDictionary<string, string> Teams
		{
			get
			{
				if (_teams == null)
				{
					ReadContestData();
				}
				return _teams;
			}
			private set { _teams = value; }
		}

		#endregion

		#endregion

		#region Methods

		public void Clear()
		{
			ContestType = null;
			_maxSalary = null;
			Players = null;
			Positions = null;
			Teams = null;
		}

		private void ReadContestData()
		{
			var data = DownloadContestData();
			ParseContestType(data);
			if (!string.IsNullOrWhiteSpace(ContestType))
			{
				ParseMaxSalary(data);
				ParseTeams(data);
				ParsePlayers(data);
				ParsePositions(data);
				ParseStats();
				ParseStartingPlayers();
			}
			else
			{
				ContestType = string.Empty;
				MaxSalary = 0;
				Players = new Dictionary<string, Player>();
				Positions = Enumerable.Empty<string>();
				Teams = new Dictionary<string, string>();
			}
		}

		private string DownloadContestData()
		{
			return Utils.DownloadURL(string.Format(CONTEST_URL_FORMAT, GameID));
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
					Stats = new[]
					{
						new PlayerStats
						{
							Name = p.Value[1],
							Source = "FanDuel",
							SeasonAveragePoints = double.Parse(p.Value[6])
						}
					},
					Team = Teams[p.Value[3]],
					Injury = ParsePlayerInjury(info, int.Parse(p.Value[9]), p.Value[12]),
					IsProbablePitcher = IsProbablePitcher(info, int.Parse(p.Value[9]))
				}).Where(p => p.Position != "P" || p.IsProbablePitcher).ToDictionary(k => k.Name, v => v);
			}
		}

		private void ParseTeams(string data)
		{
			var regex = new Regex("<b>(.*)</b>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var teamData = ParseJSONString(data, "FD.playerpicker.teamIdToFixtureCompactString");
			var teams = JObject.Parse(teamData);
			Teams = teams.Properties().ToDictionary(k => k.Name, v =>
			{
				var team = (string)teams[v.Name];
				var match = regex.Match(team);
				return match.Groups[1].Value;
			});
		}

		private void ParsePositions(string data)
		{
			var positionsData = ParseJSONString(data, "FD.playerpicker.positions");
			var positionsList = JsonConvert.DeserializeObject<List<string>>(positionsData);
			Positions = positionsList.ToArray();
		}

		private IEnumerable<PlayerStats> ReadStatsData()
		{
			StatsInfo info;
			if (STATS_INFOS.TryGetValue(ContestType, out info) && info != null)
			{
				return info.StatRetrievers.SelectMany(s => s.RetrieveStats).ToArray();
			}
			return Enumerable.Empty<PlayerStats>();
		}

		private void ParseStats()
		{
			foreach (var stats in ReadStatsData())
			{
				var player = GetPlayer(stats.Name);
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

		private IEnumerable<string> ReadStartingPlayers()
		{
			StatsInfo info;
			if (STATS_INFOS.TryGetValue(ContestType, out info) && info != null)
			{
				return info.StartingPlayerRetriever.RetrieveStartingPlayers;
			}
			return Enumerable.Empty<string>();
		}

		private void ParseStartingPlayers()
		{
			foreach (var player in ReadStartingPlayers())
			{
				var p = GetPlayer(player);
				if (p != null)
				{
					p.IsStarter = true;
				}
			}
		}

		private Player GetPlayer(string name)
		{
			Player player;
			if (Players.TryGetValue(name, out player))
			{
				return player;
			}
			string alternateName;
			if (ALTERNATE_NAMES.TryGetValue(name, out alternateName))
			{
				if (Players.TryGetValue(alternateName, out player))
				{
					return player;
				}
			}
			var lastNameIndex = name.LastIndexOf(' ');
			var lastNameFirst = name.Substring(lastNameIndex + 1) + ", " + name.Substring(0, lastNameIndex);
			if (Players.TryGetValue(lastNameFirst, out player))
			{
				return player;
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
