using System;
using System.Collections.Generic;
using System.Linq;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mCubed.LineupGenerator.ContestRetrievers
{
	public class FanDuelContestRetriever : IContestRetriever
	{
		#region Data Members

		private const string API_CLIENT_ID = "N2U3ODNmMTE4OTIzYzE2NzVjNWZhYWFmZTYwYTc5ZmM6";
		private const string CONTEST_URL = "https://api.fanduel.com/fixture-lists";
		private readonly IDictionary<string, string> _teams = new Dictionary<string, string>();

		#endregion

		#region IContestRetriever Members

		public Type ContestType
		{
			get { return typeof(FanDuelContest); }
		}

		private IEnumerable<Contest> _contests;
		public IEnumerable<Contest> Contests
		{
			get
			{
				if (_contests == null)
				{
					_contests = ReadContests().ToArray();
				}
				return _contests;
			}
		}

		public void FillAdditionalContestData(Contest contest)
		{
			var fanDuelContest = contest as FanDuelContest;
			if (fanDuelContest != null)
			{
				ReadContestData(fanDuelContest);
				ReadContestPlayers(fanDuelContest);
			}
		}

		#endregion

		#region Methods

		private IEnumerable<FanDuelContest> ReadContests()
		{
			var data = Utils.DownloadURL(CONTEST_URL, "Basic " + API_CLIENT_ID);
			return ParseContests(data);
		}

		private IEnumerable<FanDuelContest> ParseContests(string data)
		{
			var jsonData = JsonConvert.DeserializeObject(data) as JObject;
			if (jsonData != null)
			{
				var contests = jsonData["fixture_lists"] as JArray;
				if (contests != null)
				{
					foreach (var contest in contests)
					{
						yield return new FanDuelContest
						{
							ContestURL = (string)contest["_url"],
							Label = (string)contest["label"],
							MaxSalary = (int)contest["salary_cap"],
							PlayersURL = (string)((JObject)contest["players"])["_url"],
							Sport = (string)contest["sport"]
						};
					}
				}
			}
		}

		private void ReadContestData(FanDuelContest contest)
		{
			var data = Utils.DownloadURL(contest.ContestURL, "Basic " + API_CLIENT_ID);
			var jsonData = JsonConvert.DeserializeObject(data) as JObject;
			if (jsonData != null)
			{
				contest.Positions = ParsePositions(jsonData).ToArray();
				ParseTeams(jsonData);
			}
		}

		private void ReadContestPlayers(FanDuelContest contest)
		{
			var data = Utils.DownloadURL(contest.PlayersURL, "Basic " + API_CLIENT_ID);
			var jsonData = JsonConvert.DeserializeObject(data) as JObject;
			if (jsonData != null)
			{
				contest.Players = ParsePlayers(jsonData).Where(p => p.Position != "P" || p.IsProbablePitcher).ToArray();
			}
		}

		private IEnumerable<string> ParsePositions(JObject jsonData)
		{
			var contests = jsonData["fixture_lists"] as JArray;
			if (contests != null)
			{
				var contest = contests.FirstOrDefault();
				if (contest != null)
				{
					var positions = contest["roster_positions"] as JArray;
					if (positions != null)
					{
						foreach (var position in positions)
						{
							yield return (string)position["abbr"];
						}
					}
				}
			}
		}

		private void ParseTeams(JObject jsonData)
		{
			var teams = jsonData["teams"] as JArray;
			if (teams != null)
			{
				foreach (var team in teams)
				{
					_teams[(string)team["id"]] = ((string)team["code"]).ToUpper();
				}
			}
		}

		private InjuryData ParseInjury(bool isInjured, string injuryStatus)
		{
			if (isInjured)
			{
				injuryStatus = injuryStatus == null ? null : injuryStatus.ToUpper();
				if (injuryStatus == "DL" || injuryStatus == "IR" || injuryStatus == "NA" || injuryStatus == "O")
				{
					return new InjuryData(injuryStatus, InjuryType.Out);
				}
				else if (injuryStatus == "D" || injuryStatus == "DTD" || injuryStatus == "GTD" || injuryStatus == "Q")
				{
					return new InjuryData(injuryStatus, InjuryType.Possible);
				}
				else if (injuryStatus == "P")
				{
					return new InjuryData(injuryStatus, InjuryType.Probable);
				}
			}
			return null;
		}

		private string ParseBattingOrder(JToken startingOrder)
		{
			int order = 0;
			var orderString = (string)startingOrder;
			if (orderString == null || !int.TryParse(orderString, out order))
			{
				var orderInt = (int?)startingOrder;
				if (orderInt != null)
				{
					order = orderInt.Value;
				}
			}
			if (order == 1)
			{
				return "1st";
			}
			else if (order == 2)
			{
				return "2nd";
			}
			else if (order == 3)
			{
				return "3rd";
			}
			else if (order >= 4 && order <= 9)
			{
				return order + "th";
			}
			return "NA";
		}

		private IEnumerable<Player> ParsePlayers(JObject jsonData)
		{
			var players = jsonData["players"] as JArray;
			if (players != null)
			{
				foreach (var player in players)
				{
					var name = (string)player["first_name"] + " " + (string)player["last_name"];
					var team = (string)((JArray)((JObject)player["team"])["_members"])[0];
					yield return new Player
					{
						Name = name,
						Position = (string)player["position"],
						Salary = (int)player["salary"],
						Stats = new[]
						{
							new PlayerStats
							{
								Name = name,
								Source = "FanDuel",
								SeasonAveragePoints = (double?)player["fppg"]
							}
						},
						Team = _teams[team],
						BattingOrder = ParseBattingOrder(player["starting_order"]),
						Injury = ParseInjury((bool)player["injured"], (string)player["injury_status"]),
						IsProbablePitcher = (bool)player["probable_pitcher"]
					};
				}
			}
		}

		#endregion
	}
}
