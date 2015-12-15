using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.Utilities;
using mCubed.LineupGenerator.View;
using mCubed.Services.Core;

namespace mCubed.LineupGenerator.Controller
{
	public class LineupViewModel : INotifyPropertyChanged
	{
		#region Properties

		#region Contests

		private IEnumerable<ContestViewModel> _contests = Enumerable.Empty<ContestViewModel>();
		public IEnumerable<ContestViewModel> Contests
		{
			get { return _contests; }
			private set
			{
				if (_contests != value)
				{
					_contests = value;
					RaisePropertyChanged("Contests");
				}
			}
		}

		#endregion

		#region CurrentFullScreenProcess

		private string _currentFullScreenProcess;
		public string CurrentFullScreenProcess
		{
			get { return _currentFullScreenProcess; }
			private set
			{
				if (_currentFullScreenProcess != value)
				{
					_currentFullScreenProcess = value;
					RaisePropertyChanged("CurrentFullScreenProcess");
				}
			}
		}

		#endregion

		#region CurrentLineupProcess

		private string _currentLineupProcess;
		public string CurrentLineupProcess
		{
			get { return _currentLineupProcess; }
			private set
			{
				if (_currentLineupProcess != value)
				{
					_currentLineupProcess = value;
					RaisePropertyChanged("CurrentLineupProcess");
				}
			}
		}

		#endregion

		#region Lineups

		private List<Lineup> _lineups = new List<Lineup>();
		public List<Lineup> Lineups
		{
			get { return _lineups; }
			private set
			{
				if (_lineups != value)
				{
					_lineups = value;
					RaisePropertyChanged("Lineups");
					LineupsView = null;
				}
			}
		}

		#endregion

		#region LineupsView

		private SortableListCollectionView _lineupsView;
		public SortableListCollectionView LineupsView
		{
			get
			{
				if (_lineupsView == null)
				{
					Utils.DispatcherInvoke(() =>
					{
						_lineupsView = new SortableListCollectionView(Lineups);
						_lineupsView.Sort("Rating", ListSortDirection.Descending);
					});
				}
				return _lineupsView;
			}
			private set
			{
				if (_lineupsView != value)
				{
					_lineupsView = value;
					RaisePropertyChanged("LineupsView");
				}
			}
		}

		#endregion

		#region SelectedContest

		private ContestViewModel _selectedContest;
		public ContestViewModel SelectedContest
		{
			get { return _selectedContest; }
			set
			{
				if (_selectedContest != value)
				{
					_selectedContest = value;
					RaisePropertyChanged("SelectedContest");
				}
			}
		}

		#endregion

		#endregion

		#region Constructors

		public LineupViewModel()
		{
			RetrieveContests();
		}

		#endregion

		#region Methods

		public void RetrieveContests()
		{
			CurrentFullScreenProcess = "Retrieving contests...";
			ThreadPool.QueueUserWorkItem(q =>
			{
				var service = new LineupAggregatorService();
				Contests = service.Contests.Select(c => new ContestViewModel(c)).ToArray();
				CurrentFullScreenProcess = null;
			});
		}

		private void LineupError(string error)
		{
			MessageBoxEx.Show((MainWindow)Application.Current.MainWindow, error);
			Lineups = new List<Lineup>();
		}

		public void GenerateLineups()
		{
			var contest = SelectedContest;
			if (contest == null)
			{
				LineupError("Select a contest to choose players from.");
			}
			else
			{
				var includePlayers = contest.PlayersGrouped.SelectMany(p => p.Players).Where(p => p.IncludeInLineups).Select(p => p.Player).ToArray();
				if (includePlayers.Length < contest.Contest.Positions.Count)
				{
					LineupError(string.Format("Select at least {0} players to generate lineups from.", contest.Contest.Positions.Count));
				}
				else if (includePlayers.Length > LineupGenerator.MAX_PLAYERS_TO_INCLUDE)
				{
					LineupError(string.Format("Select at most {0} players to generate lineups from for performance purposes.", LineupGenerator.MAX_PLAYERS_TO_INCLUDE));
				}
				else
				{
					CurrentLineupProcess = "Generating lineups...";
					ThreadPool.QueueUserWorkItem(q =>
					{
						var lineups = LineupGenerator.GenerateLineups(contest, includePlayers).ToList();
						lineups.UpdateRating(lineups.Count);
						Lineups = lineups;
						CurrentLineupProcess = null;
					});
				}
			}
		}

		public void SelectRecommended()
		{
			var contest = SelectedContest;
			if (contest != null)
			{
				ThreadPool.QueueUserWorkItem(q =>
				{
					foreach (var playerGroup in contest.PlayersGrouped)
					{
						var playersNeededForPosition = contest.Contest.Positions.Count(p => p == playerGroup.Position);
						var recommendedPlayers = playersNeededForPosition * 4;
						foreach (var player in playerGroup.Players.OrderByDescending(p => p.Player.ProjectedPointsPerDollar))
						{
							player.IncludeInLineups = player.Player.IsPlaying && recommendedPlayers-- > 0;
						}
						var topPlayer = playerGroup.Players.OrderByDescending(p => p.Player.ProjectedPoints).FirstOrDefault(p => p.Player.IsPlaying);
						if (topPlayer != null)
						{
							topPlayer.IncludeInLineups = true;
						}
					}
				});
			}
		}

		public void SelectStarters()
		{
			var contest = SelectedContest;
			if (contest != null)
			{
				ThreadPool.QueueUserWorkItem(q =>
				{
					foreach (var player in contest.PlayersGrouped.SelectMany(p => p.Players))
					{
						player.IncludeInLineups = player.Player.IsProbablePitcher || player.Player.IsStarter;
					}
				});
			}
		}

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(property));
			}
		}

		#endregion
	}
}
