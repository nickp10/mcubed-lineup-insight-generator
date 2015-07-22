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
				var contests = jsonData.Value<JArray>("fixture_lists");
				if (contests != null)
				{
					foreach (var contest in contests)
					{
						yield return new FanDuelContest
						{
							ContestURL = contest.Value<string>("_url"),
							Label = contest.Value<string>("label"),
							MaxSalary = contest.Value<int>("salary_cap"),
							PlayersURL = contest.Value<JObject>("players").Value<string>("_url"),
							Sport = contest.Value<string>("sport")
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
			var contests = jsonData.Value<JArray>("fixture_lists");
			if (contests != null)
			{
				var contest = contests.FirstOrDefault();
				if (contest != null)
				{
					var positions = contest.Value<JArray>("roster_positions");
					if (positions != null)
					{
						foreach (var position in positions)
						{
							yield return position.Value<string>("abbr");
						}
					}
				}
			}
		}

		private void ParseTeams(JObject jsonData)
		{
			var teams = jsonData.Value<JArray>("teams");
			if (teams != null)
			{
				foreach (var team in teams)
				{
					_teams[team.Value<string>("id")] = team.Value<string>("code").ToUpper();
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

		private string ParseBattingOrder(string startingOrder)
		{
			int order = 0;
			if (int.TryParse(startingOrder, out order))
			{
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
			}
			return "NA";
		}

		private IEnumerable<Player> ParsePlayers(JObject jsonData)
		{
			var players = jsonData.Value<JArray>("players");
			if (players != null)
			{
				foreach (var player in players)
				{
					var name = player.Value<string>("first_name") + " " + player.Value<string>("last_name");
					var team = (string)player.Value<JObject>("team").Value<JArray>("_members")[0];
					yield return new Player(name)
					{
						Position = player.Value<string>("position"),
						Salary = player.Value<int>("salary"),
						Stats = new[]
						{
							new PlayerStats
							{
								Source = "FanDuel",
								SeasonAveragePoints = player.Value<double?>("fppg")
							}
						},
						Team = _teams[team],
						BattingOrder = ParseBattingOrder(player.Value<string>("starting_order")),
						Injury = ParseInjury(player.Value<bool>("injured"), player.Value<string>("injury_status")),
						IsProbablePitcher = player.Value<bool>("probable_pitcher")
					};
				}
			}
		}

		#endregion
	}
}
