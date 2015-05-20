using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using mCubed.LineupGenerator.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mCubed.LineupGenerator.StatRetrievers
{
	public class NumberFireStatRetriever : IStatRetriever
	{
		#region Data Members

		private readonly string _players;
		private readonly string _playerID;
		private readonly string _projections;
		private readonly string _projectedPoints;
		private readonly IEnumerable<string> _urls;
		private IEnumerable<PlayerStats> _stats;

		#endregion

		#region Constructors

		public NumberFireStatRetriever(string players, string playerID, string projections, string projectedPoints, params string[] urls)
			: this(players, playerID, projections, projectedPoints, (IEnumerable<string>)urls)
		{
		}

		public NumberFireStatRetriever(string players, string playerID, string projections, string projectedPoints, IEnumerable<string> urls)
		{
			_players = players;
			_playerID = playerID;
			_projections = projections;
			_projectedPoints = projectedPoints;
			_urls = urls ?? Enumerable.Empty<string>();
		}

		#endregion

		#region IStatRetriever Members

		public IEnumerable<PlayerStats> RetrieveStats
		{
			get
			{
				if (_stats == null)
				{
					_stats = _urls.Select(DownloadStatsData).Select(ParsePlayerStats).SelectMany(p => p).ToArray();
				}
				return _stats;
			}
		}

		#endregion

		#region Methods

		private string DownloadStatsData(string url)
		{
			using (var client = new WebClient())
			{
				return client.DownloadString(url);
			}
		}

		private IDictionary<string, string> ParsePlayers(IEnumerable<KeyValuePair<string, JToken>> players)
		{
			return players.ToDictionary(pair => pair.Key, pair => (string)(pair.Value["name"]));
		}

		private IEnumerable<PlayerStats> ParseStats(IDictionary<string, string> players, JArray projections)
		{
			foreach (JObject projection in projections)
			{
				var playerID = (string)projection[_playerID];
				var projectedPoints = (double?)projection[_projectedPoints];
				if (projectedPoints != null)
				{
					string playerName;
					if (players.TryGetValue(playerID, out playerName))
					{
						yield return new PlayerStats
						{
							Name = playerName,
							Source = "NumberFire",
							ProjectedPoints = projectedPoints.Value
						};
					}
				}
			}
		}

		private IEnumerable<PlayerStats> ParsePlayerStats(string data)
		{
			var regex = new Regex(@"NF_DATA\s*=\s*(.*?);", RegexOptions.Singleline);
			var match = regex.Match(data);
			if (match.Success)
			{
				var json = ParseGroupValue(match, 1);
				var jsonObj = (JObject)JsonConvert.DeserializeObject(json);
				var players = ParsePlayers((JObject)jsonObj[_players]);
				return ParseStats(players, (JArray)jsonObj[_projections]);
			}
			return Enumerable.Empty<PlayerStats>();
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

		#endregion
	}
}
