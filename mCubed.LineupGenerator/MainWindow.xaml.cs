using System.Diagnostics;
using System.Windows;
using mCubed.LineupGenerator.Controller;

namespace mCubed.LineupGenerator
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new LineupViewModel();
		}

		#region Event Handlers

		private void OnSelectStartersClick(object sender, RoutedEventArgs e)
		{
			((LineupViewModel)DataContext).SelectStarters();
		}

		private void OnGenerateLineupsClick(object sender, RoutedEventArgs e)
		{
			((LineupViewModel)DataContext).GenerateLineups();
		}

		private void OnESPNViewClick(object sender, RoutedEventArgs e)
		{
			var viewModel = DataContext as LineupViewModel;
			var contest = viewModel == null ? null : viewModel.SelectedContest;
			if (contest != null)
			{
				var element = sender as FrameworkContentElement;
				var id = element == null ? null : element.Tag as string;
				if (id != null)
				{
					var url = "http://espn.go.com/" + contest.Sport.ToLower() + "/player/_/id/" + id;
					Process.Start(new ProcessStartInfo(url));
				}
			}
		}

		#endregion
	}
}
