using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using LineupGenerator.Model;
using LineupGenerator.Utilities;

namespace LineupGenerator.Controller
{
	public class LineupViewModel : INotifyPropertyChanged
	{
		#region Data Members

		private DataRetriever _dataRetriever;
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
				if (_dataRetriever == null)
				{
					return Enumerable.Empty<IGrouping<string, Player>>();
				}
				else
				{
					return AllPlayers.GroupBy(p => p.Position).OrderBy(g => g.Key, new PositionComparer(_dataRetriever.Positions));
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

		#region GameID

		private string _gameID;
		public string GameID
		{
			get { return _gameID; }
			set
			{
				if (_gameID != value)
				{
					_gameID = value;
					RaisePropertyChanged("GameID");
				}
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

		#endregion

		#region Methods

		public void RetrievePlayerList()
		{
			CurrentProcess = "Retrieving players...";
			ThreadPool.QueueUserWorkItem(q =>
			{
				_dataRetriever = new DataRetriever(GameID);
				_lineupGenerator.AllPlayers = _dataRetriever.Players;
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
				var lineups = _lineupGenerator.GenerateLineups(_dataRetriever.Positions, _dataRetriever.MaxSalary).ToArray();
				var threshold = (int)Math.Ceiling(lineups.Length * 0.05d);
				lineups.AddRating(l => l.TotalProjectedPoints, threshold);
				lineups.AddRating(l => l.TotalRecentAveragePoints, threshold);
				lineups.AddRating(l => l.TotalSeasonAveragePoints, threshold);
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
