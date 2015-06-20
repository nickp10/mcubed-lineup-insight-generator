using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.Utilities;
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
					_stats = _urls.Select(Utils.DownloadURL).Select(ParsePlayerStats).SelectMany(p => p).ToArray();
				}
				return _stats;
			}
		}

		#endregion

		#region Methods

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
			var regex = new Regex(@"NF_DATA\s*=\s*(.*?}|]);", RegexOptions.Singleline);
			var match = regex.Match(data);
			if (match.Success)
			{
				var json = Utils.ParseGroupValue(match, 1);
				var jsonObj = JsonConvert.DeserializeObject(json) as JObject;
				if (jsonObj != null)
				{
					var jsonPlayers = jsonObj[_players] as JObject;
					var jsonProjections = jsonObj[_projections] as JArray;
					if (jsonPlayers != null && jsonProjections != null)
					{
						var players = ParsePlayers(jsonPlayers);
						return ParseStats(players, jsonProjections);
					}
				}
			}
			return Enumerable.Empty<PlayerStats>();
		}

		#endregion
	}
}
