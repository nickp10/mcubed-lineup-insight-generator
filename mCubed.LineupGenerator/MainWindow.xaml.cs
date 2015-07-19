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

		#endregion
	}
}
