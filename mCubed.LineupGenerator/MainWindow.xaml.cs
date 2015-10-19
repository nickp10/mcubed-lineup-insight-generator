using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

		private void OnSelectRecommendedClick(object sender, RoutedEventArgs e)
		{
			((LineupViewModel)DataContext).SelectRecommended();
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
					var url = "http://espn.go.com/" + contest.Contest.Sport.ToLower() + "/player/_/id/" + id;
					Process.Start(new ProcessStartInfo(url));
				}
			}
		}

		#endregion

		#region Slider Event Handlers

		private bool isDragging;

		private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			DoWork(sender, ((Slider)sender).Value);
			isDragging = false;
		}

		private void Slider_DragStarted(object sender, DragStartedEventArgs e)
		{
			isDragging = true;
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!isDragging)
			{
				DoWork(sender, e.NewValue);
			}
		}

		private void DoWork(object sender, double value)
		{
			var viewModel = ((Slider)sender).DataContext as LineupViewModel;
			if (viewModel != null)
			{
				viewModel.RefreshRatings(value);
			}
		}

		#endregion
	}
}
