using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using mCubed.LineupGenerator.Model;

namespace mCubed.LineupGenerator.StatRetrievers
{
	public class RotoWireStatRetriever : IStatRetriever
	{
		#region Data Members

		private readonly string _url;
		private readonly string _regex;
		private readonly int _nameGroupIndex;
		private readonly int _projectedGroupIndex;
		private IEnumerable<PlayerStats> _stats;

		#endregion

		#region Constructors

		public RotoWireStatRetriever(string regex, int nameGroupIndex, int projectedGroupIndex, string url)
		{
			_url = url;
			_regex = regex;
			_nameGroupIndex = nameGroupIndex;
			_projectedGroupIndex = projectedGroupIndex;
		}

		#endregion

		#region IStatRetriever Members

		public IEnumerable<PlayerStats> RetrieveStats
		{
			get
			{
				if (_stats == null)
				{
					_stats = ParsePlayerStats(DownloadStatsData(_url)).ToArray();
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

		private IEnumerable<PlayerStats> ParsePlayerStats(string data)
		{
			var regex = new Regex(_regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var match = regex.Match(data);
			while (match.Success)
			{
				var name = ParsePlayerName(match, _nameGroupIndex);
				yield return new PlayerStats
				{
					Name = name,
					Source = "RotoWire",
					ProjectedPoints = ParseGroupDouble(match, _projectedGroupIndex)
				};
				match = match.NextMatch();
			}
		}

		#endregion
	}
}
