using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using mCubed.LineupGenerator.Controller;
using Newtonsoft.Json;

namespace mCubed.LineupGenerator.Services
{
	public class LineupGeneratorService
	{
		#region Properties

		private string _server;
		public string Server
		{
			get
			{
				if (_server == null)
				{
					foreach (var server in Settings.InsightServers)
					{
						try
						{
							using (var ping = new Ping())
							{
								var reply = ping.Send(server, 5000);
								if (reply.Status == IPStatus.Success)
								{
									_server = server;
									break;
								}
							}
						}
						catch { }
					}
				}
				return _server;
			}
		}

		#endregion

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
			try
			{
				var url = string.Format("http://{0}:{1}/{2}", Server, Settings.InsightPort, string.Join("/", urlParams));
				using (var client = new WebClient())
				{
					var responseBody = client.DownloadString(url);
					return JsonConvert.DeserializeObject<T>(responseBody, new JsonSerializerSettings
					{
						DateTimeZoneHandling = DateTimeZoneHandling.Local
					});
				}
			}
			catch { }
			return default(T);
		}

		#endregion
	}
}
