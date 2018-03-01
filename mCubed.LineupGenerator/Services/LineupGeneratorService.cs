using System.Collections.Generic;
using System.Net;
using mCubed.LineupGenerator.Controller;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class LineupGeneratorService
	{
		#region Methods

		public List<Contest> Contests
		{
			get { return Get<List<Contest>>(); }
		}

		public PlayerCard GetPlayerCard(string contestID, string playerID)
		{
			return Get<PlayerCard>(contestID, playerID);
		}

		#endregion

		#region Helpers

		private T Get<T>(params string[] urlParams)
		{
			var url = string.Format("http://{0}:{1}/{2}", Settings.InsightServer, Settings.InsightPort, string.Join("/", urlParams));
			using (var client = new WebClient())
			{
				var responseBody = client.DownloadString(url);
				return JsonConvert.DeserializeObject<T>(responseBody, new JsonSerializerSettings
				{
					DateTimeZoneHandling = DateTimeZoneHandling.Local
				});
			}
		}

		#endregion
	}
}
