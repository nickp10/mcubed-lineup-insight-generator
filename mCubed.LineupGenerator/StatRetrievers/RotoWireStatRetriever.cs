using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.Utilities;

namespace mCubed.LineupGenerator.StatRetrievers
{
	public class RotoWireStatRetriever : IStatRetriever
	{
		#region Data Members

		private readonly string _url;
		private readonly string _regex;
		private readonly int _nameGroupIndex;
		private readonly int _projectedGroupIndex;
		private readonly int _battingOrderGroupIndex;
		private IEnumerable<PlayerStats> _stats;

		#endregion

		#region Constructors

		public RotoWireStatRetriever(string regex, int nameGroupIndex, int projectedGroupIndex, string url)
			: this(regex, nameGroupIndex, projectedGroupIndex, -1, url)
		{
		}

		public RotoWireStatRetriever(string regex, int nameGroupIndex, int projectedGroupIndex, int battingOrderGroupIndex, string url)
		{
			_url = url;
			_regex = regex;
			_nameGroupIndex = nameGroupIndex;
			_projectedGroupIndex = projectedGroupIndex;
			_battingOrderGroupIndex = battingOrderGroupIndex;
		}

		#endregion

		#region IStatRetriever Members

		public IEnumerable<PlayerStats> RetrieveStats
		{
			get
			{
				if (_stats == null)
				{
					_stats = ParsePlayerStats(Utils.DownloadURL(_url)).ToArray();
				}
				return _stats;
			}
		}

		#endregion

		#region Methods

		private string ParsePlayerName(Match match, int index)
		{
			var name = Utils.ParseGroupValue(match, index);
			var parts = name.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
			return parts.Length == 2 ? parts[1] + " " + parts[0] : name;
		}

		private string ParseBattingOrder(Match match, int index)
		{
			var order = Utils.ParseGroupValue(match, index);
			if (order != null)
			{
				var regex = new Regex(".*?>(.*?)</", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				var subMatch = regex.Match(order);
				if (subMatch.Success)
				{
					return Utils.ParseGroupValue(subMatch, 1);
				}
			}
			return null;
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
					ProjectedPoints = Utils.ParseGroupDouble(match, _projectedGroupIndex),
					BattingOrder = ParseBattingOrder(match, _battingOrderGroupIndex)
				};
				match = match.NextMatch();
			}
		}

		#endregion
	}
}
