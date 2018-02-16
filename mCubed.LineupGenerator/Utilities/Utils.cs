using System;
using System.Windows;

namespace mCubed.LineupGenerator.Utilities
{
	public static class Utils
	{
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
