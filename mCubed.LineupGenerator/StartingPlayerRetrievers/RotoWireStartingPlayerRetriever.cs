using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using mCubed.LineupGenerator.Utilities;

namespace mCubed.LineupGenerator.StartingPlayerRetrievers
{
	public class RotoWireStartingPlayerRetriever : IStartingPlayerRetriever
	{
		#region Data Members

		private readonly string _url;
		private readonly IEnumerable<string> _regexs;
		private IEnumerable<string> _startingPlayers;

		#endregion

		#region Constructors

		public RotoWireStartingPlayerRetriever(string url, params string[] regexs)
			: this(url, (IEnumerable<string>)regexs)
		{
		}

		public RotoWireStartingPlayerRetriever(string url, IEnumerable<string> regexs)
		{
			_url = url;
			_regexs = regexs;
		}

		#endregion

		#region IStartingPlayerRetriever Members

		public IEnumerable<string> RetrieveStartingPlayers
		{
			get
			{
				if (_startingPlayers == null)
				{
					_startingPlayers = ParseStartingPlayers(Utils.DownloadURL(_url)).ToArray();
				}
				return _startingPlayers;
			}
		}

		#endregion

		#region Methods

		private IEnumerable<string> ParseStartingPlayers(string data)
		{
			foreach (var regexExp in _regexs)
			{
				var regex = new Regex(regexExp, RegexOptions.IgnoreCase | RegexOptions.Singleline);
				var match = regex.Match(data);
				while (match.Success)
				{
					yield return Utils.ParseGroupValue(match, 1);
					match = match.NextMatch();
				}
			}
		}

		#endregion
	}
}
