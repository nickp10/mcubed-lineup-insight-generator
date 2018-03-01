using System;
using System.Diagnostics;
using System.Web;
using System.Windows;
using System.Windows.Interop;
using mCubed.LineupGenerator.Controller;

namespace mCubed.LineupGenerator
{
	public partial class MainWindow : Window, System.Windows.Forms.IWin32Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new LineupViewModel();
		}

		#region Event Handlers

		private void OnSelectRecommendedClick(object sender, RoutedEventArgs e)
		{
			((LineupViewModel)DataContext).SelectRecommended();
		}

		private void OnGenerateLineupsClick(object sender, RoutedEventArgs e)
		{
			((LineupViewModel)DataContext).GenerateLineups();
		}

		private void OnPlayerViewClick(object sender, RoutedEventArgs e)
		{
			var viewModel = DataContext as LineupViewModel;
			var contest = viewModel == null ? null : viewModel.SelectedContest;
			if (contest != null)
			{
				var element = sender as FrameworkContentElement;
				var playerViewModel = element == null ? null : element.DataContext as PlayerViewModel;
				if (playerViewModel != null)
				{
					var url = "http://espn.go.com/" + contest.Contest.Sport.ToString().ToLower() + "/players?search=" + HttpUtility.UrlEncode(GetLastName(playerViewModel.Player.Name));
					Process.Start(new ProcessStartInfo(url));
				}
			}
		}

		private string GetLastName(string name)
		{
			var index = name.LastIndexOf(' ');
			if (index >= 0)
			{
				var lastName = name.Substring(index + 1);
				if (lastName.EndsWith(".")) // Jr., Sr., etc.
				{
					return GetLastName(name.Substring(0, index));
				}
				return lastName;
			}
			return name;
		}

		#endregion

		#region IWin32Window Members

		public IntPtr Handle
		{
			get { return new WindowInteropHelper(this).Handle; }
		}

		#endregion
	}
}
