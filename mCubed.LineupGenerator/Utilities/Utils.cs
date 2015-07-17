using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;

namespace mCubed.LineupGenerator.Utilities
{
	public static class Utils
	{
		#region URL Methods

		public static string DownloadURL(string url)
		{
			using (var client = new WebClient())
			{
				return client.DownloadString(url);
			}
		}

		public static string DownloadURL(string url, string authorization)
		{
			using (var client = new WebClient())
			{
				client.Headers.Add(HttpRequestHeader.Authorization, authorization);
				return client.DownloadString(url);
			}
		}

		#endregion

		#region Regex Methods

		public static string ParseGroupValue(Match match, int index)
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

		public static double ParseGroupDouble(Match match, int index)
		{
			var value = ParseGroupValue(match, index);
			double doubleValue;
			if (double.TryParse(value, out doubleValue))
			{
				return doubleValue;
			}
			return 0d;
		}

		#endregion

		#region Dispatcher Methods

		public static void DispatcherInvoke(Action action)
		{
			var dispatcher = Application.Current.Dispatcher;
			if (dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				dispatcher.Invoke(action);
			}
		}

		public static void DispatcherBeginInvoke(Action action)
		{
			var dispatcher = Application.Current.Dispatcher;
			if (dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				dispatcher.BeginInvoke(action);
			}
		}

		#endregion
	}
}
