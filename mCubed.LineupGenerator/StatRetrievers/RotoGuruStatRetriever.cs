using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.Utilities;

namespace mCubed.LineupGenerator.StatRetrievers
{
	public class RotoGuruStatRetriever : IStatRetriever
	{
		#region Data Members

		private readonly string _url;
		private IEnumerable<Player> _stats;

		#endregion

		#region Constructors

		public RotoGuruStatRetriever(string url)
		{
			_url = url;
		}

		#endregion

		#region IStatRetriever Members

		public IEnumerable<Player> RetrieveStats
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

		private double? ParsePoints(string[] parts, int index)
		{
			var str = parts[index];
			double value;
			if (double.TryParse(str, out value))
			{
				return value;
			}
			return null;
		}

		private IEnumerable<Player> ParsePlayerStats(string data)
		{
			var regex = new Regex("<P>(GID.*?)<", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			var match = regex.Match(data);
			if (match.Success)
			{
				var stats = Utils.ParseGroupValue(match, 1);
				if (stats != null)
				{
					using (var reader = new StringReader(stats))
					{
						string line;
						var firstLine = true;
						while ((line = reader.ReadLine()) != null)
						{
							if (firstLine)
							{
								firstLine = false;
							}
							else
							{
								var lineParts = line.Split(';');
								yield return new Player(lineParts[2])
								{
									ESPNID = lineParts[16],
									Stats = new[]
									{
										new PlayerStats
										{
											Source = "RotoGuru",
											RecentAveragePoints = ParsePoints(lineParts, 10)
										}
									}
								};
							}
						}
					}
				}
			}
		}

		#endregion
	}
}
