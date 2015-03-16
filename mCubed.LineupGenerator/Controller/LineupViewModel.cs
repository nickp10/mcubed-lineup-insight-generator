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
		#region Data Members

		private readonly LineupGenerator _lineupGenerator = new LineupGenerator();

		#endregion

		#region Properties

		#region AllPlayers

		public IEnumerable<Player> AllPlayers
		{
			get { return _lineupGenerator.AllPlayers; }
		}

		#endregion

		#region AllPlayersGrouped

		public IEnumerable<IGrouping<string, Player>> AllPlayersGrouped
		{
			get
			{
				if (string.IsNullOrEmpty(DataRetriever.GameID))
				{
					return Enumerable.Empty<IGrouping<string, Player>>();
				}
				else
				{
					return AllPlayers.GroupBy(p => p.Position).OrderBy(g => g.Key, new PositionComparer(DataRetriever.Positions));
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

		#region DataRetriever

		private DataRetriever _dataRetriever;
		public DataRetriever DataRetriever
		{
			get
			{
				if (_dataRetriever == null)
				{
					_dataRetriever = new DataRetriever();
				}
				return _dataRetriever;
			}
		}

		#endregion

		#region Lineups

		private IEnumerable<Lineup> _lineups = Enumerable.Empty<Lineup>();
		public IEnumerable<Lineup> Lineups
		{
			get { return _lineups; }
			private set
			{
				if (_lineups != value)
				{
					_lineups = value;
					RaisePropertyChanged("Lineups");
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

		#endregion

		#region Methods

		public void RetrievePlayerList()
		{
			CurrentProcess = "Retrieving players...";
			ThreadPool.QueueUserWorkItem(q =>
			{
				DataRetriever.Clear();
				_lineupGenerator.AllPlayers = DataRetriever.Players;
				RaisePropertyChanged("AllPlayers");
				RaisePropertyChanged("AllPlayersGrouped");
				CurrentProcess = null;
			});
		}

		public void GenerateLineups()
		{
			CurrentProcess = "Generating lineups...";
			ThreadPool.QueueUserWorkItem(q =>
			{
				var lineups = _lineupGenerator.GenerateLineups(DataRetriever.Positions, DataRetriever.MaxSalary).ToArray();
				lineups.AddRating(l => l.TotalProjectedPoints, lineups.Length, RatingTolerance);
				lineups.AddRating(l => l.TotalRecentAveragePoints, lineups.Length, RatingTolerance);
				lineups.AddRating(l => l.TotalSeasonAveragePoints, lineups.Length, RatingTolerance);
				Lineups = lineups;
				CurrentProcess = null;
			});
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
