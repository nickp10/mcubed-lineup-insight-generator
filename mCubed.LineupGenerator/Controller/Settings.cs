using System;
using System.Collections.Generic;
using System.IO;

namespace mCubed.LineupGenerator.Controller
{
	public static class Settings
	{
		#region Data Members

		private static readonly IDictionary<string, string> _settings = new Dictionary<string, string>();

		#endregion

		#region Helpers

		private static void ReadSettings()
		{
			if (_settings.Count == 0)
			{
				var lines = File.ReadAllLines(AppDomain.CurrentDomain.FriendlyName + ".settings");
				foreach (var line in lines)
				{
					var index = line.IndexOf('=');
					if (index > -1)
					{
						string key = line.Substring(0, index);
						string value = line.Substring(index + 1);
						_settings[key] = value;
					}
				}
			}
		}

		#endregion

		#region Properties

		public static string[] InsightServers
		{
			get
			{
				ReadSettings();
				return _settings["InsightServers"].Split(',');
			}
		}

		public static string InsightPort
		{
			get
			{
				ReadSettings();
				return _settings["InsightPort"];
			}
		}

		#endregion
	}
}
