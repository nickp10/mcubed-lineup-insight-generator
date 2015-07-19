using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.Utilities;

namespace mCubed.LineupGenerator.Controller
{
	public class LineupViewModel : INotifyPropertyChanged
	{
		#region Properties

		#region Contests

		private IEnumerable<Contest> _contests = Enumerable.Empty<Contest>();
		public IEnumerable<Contest> Contests
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

		#region CurrentProcess

		private string _currentProcess;
		public string CurrentProcess
		{
			get { return _currentProcess; }
			private set
			{
				if (_currentProcess != value)
				{
					_currentProcess = value;
					RaisePropertyChanged("CurrentProcess");
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
						_lineupsView.Sort("TotalProjectedPoints", ListSortDirection.Descending);
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

		#region RatingTolerance

		private double _ratingTolerance = IEnumerableExtensions.DEFAULT_RATING_TOLERANCE;
		public double RatingTolerance
		{
			get { return _ratingTolerance; }
			set
			{
				if (_ratingTolerance != value)
				{
					_ratingTolerance = value;
					RaisePropertyChanged("RatingTolerance");
				}
			}
		}

		#endregion

		#region SelectedContest

		private Contest _selectedContest;
		public Contest SelectedContest
		{
			get { return _selectedContest; }
			set
			{
				if (_selectedContest != value)
				{
					_selectedContest = value;
					RetrieveContestData(_selectedContest);
					RaisePropertyChanged("SelectedContest");
				}
			}
		}

		#endregion

		#endregion

		#region Constructors

		public LineupViewModel()
		{
			RetrieveContestList();
		}

		#endregion

		#region Methods

		public void RetrieveContestList()
		{
			CurrentProcess = "Retrieving contests...";
			ThreadPool.QueueUserWorkItem(q =>
			{
				Contests = DataRetriever.ContestRetrievers.SelectMany(c => c.Contests).ToArray();
				CurrentProcess = null;
			});
		}

		public void RetrieveContestData(Contest contest)
		{
			if (contest != null && !contest.IsDataRetrieved)
			{
				CurrentProcess = "Retrieving contest data...";
				ThreadPool.QueueUserWorkItem(q =>
				{
					var retriever = DataRetriever.ContestRetrievers.FirstOrDefault(c => c.ContestType == contest.GetType());
					if (retriever != null)
					{
						retriever.FillAdditionalContestData(contest);
						contest.IsDataRetrieved = true;
					}
					DataRetriever.ParseStartingPlayers(contest);
					DataRetriever.ParseStats(contest);
					CurrentProcess = null;
				});
			}
		}

		public void GenerateLineups()
		{
			var contest = SelectedContest;
			if (contest != null)
			{
				CurrentProcess = "Generating lineups...";
				ThreadPool.QueueUserWorkItem(q =>
				{
					var lineups = LineupGenerator.GenerateLineups(contest).ToList();
					lineups.AddRating(l => l.TotalProjectedPoints, lineups.Count, RatingTolerance);
					lineups.AddRating(l => l.TotalRecentAveragePoints, lineups.Count, RatingTolerance);
					lineups.AddRating(l => l.TotalSeasonAveragePoints, lineups.Count, RatingTolerance);
					Lineups = lineups;
					CurrentProcess = null;
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
					foreach (var player in contest.Players)
					{
						player.IncludeInLineups = player.IsProbablePitcher || player.IsStarter;
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
