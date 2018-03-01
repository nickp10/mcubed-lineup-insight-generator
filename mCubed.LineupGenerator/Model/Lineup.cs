using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using mCubed.LineupGenerator.Controller;

namespace mCubed.LineupGenerator.Model
{
	public class Lineup : INotifyPropertyChanged
	{
		#region Constructors

		public Lineup(params PlayerViewModel[] players)
		{
			_players = new ObservableCollection<PlayerViewModel>(players);
		}

		#endregion

		#region Properties

		#region IsSelected

		private bool _isSelected;
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected != value)
				{
					_isSelected = value;
					RaisePropertyChanged("IsSelected");
				}
			}
		}

		#endregion

		#region Players

		private ObservableCollection<PlayerViewModel> _players;
		public ObservableCollection<PlayerViewModel> Players
		{
			get
			{
				if (_players == null)
				{
					_players = new ObservableCollection<PlayerViewModel>();
					_players.CollectionChanged += new NotifyCollectionChangedEventHandler(OnPlayersCollectionChanged);
				}
				return _players;
			}
		}



		#endregion

		#region PlayersString

		private string _playersString;
		public string PlayersString
		{
			get
			{
				if (_playersString == null)
				{
					_playersString = Players.Any() ? Players.Select(p => p.Player.Name).Aggregate((p1, p2) => p1 + ", " + p2) : string.Empty;
				}
				return _playersString;
			}
			private set
			{
				if (_playersString != value)
				{
					_playersString = value;
					RaisePropertyChanged("PlayersString");
				}
			}
		}

		#endregion

		#region Rating

		private double _rating;
		public double Rating
		{
			get { return _rating; }
			set
			{
				if (_rating != value)
				{
					_rating = value;
					RaisePropertyChanged("Rating");
				}
			}
		}

		#endregion

		#region TotalSalary

		private bool _recalculateTotalSalary = true;
		private double? _totalSalary;
		public double TotalSalary
		{
			get
			{
				if (_recalculateTotalSalary)
				{
					_totalSalary = Sum(p => p.Player.Salary);
					_recalculateTotalSalary = false;
				}
				return _totalSalary == null ? 0d : _totalSalary.Value;
			}
		}

		#endregion

		#region TotalProjectedCeiling

		private bool _recalculateProjectedCeiling = true;
		private double? _totalProjectedCeiling;
		public double? TotalProjectedCeiling
		{
			get
			{
				if (_recalculateProjectedCeiling)
				{
					_totalProjectedCeiling = Sum(p => p.Player.ProjectedCeiling);
					_recalculateProjectedCeiling = false;
				}
				return _totalProjectedCeiling;
			}
		}

		#endregion

		#region TotalProjectedFloor

		private bool _recalculateProjectedFloor = true;
		private double? _totalProjectedFloor;
		public double? TotalProjectedFloor
		{
			get
			{
				if (_recalculateProjectedFloor)
				{
					_totalProjectedFloor = Sum(p => p.Player.ProjectedFloor);
					_recalculateProjectedFloor = false;
				}
				return _totalProjectedFloor;
			}
		}

		#endregion

		#region TotalProjectedPoints

		private bool _recalculateTotalProjectedPoints = true;
		private double? _totalProjectedPoints;
		public double TotalProjectedPoints
		{
			get
			{
				if (_recalculateTotalProjectedPoints)
				{
					_totalProjectedPoints = Sum(p => p.Player.ProjectedPoints);
					_recalculateTotalProjectedPoints = false;
				}
				return _totalProjectedPoints == null ? 0d : _totalProjectedPoints.Value;
			}
		}

		#endregion

		#region TotalRecentAveragePoints

		private bool _recalculateTotalRecentAveragePoints = true;
		private double? _totalRecentAveragePoints;
		public double TotalRecentAveragePoints
		{
			get
			{
				if (_recalculateTotalRecentAveragePoints)
				{
					_totalRecentAveragePoints = Sum(p => p.Player.RecentAveragePoints);
					_recalculateTotalRecentAveragePoints = false;
				}
				return _totalRecentAveragePoints == null ? 0d : _totalRecentAveragePoints.Value;
			}
		}

		#endregion

		#region TotalSeasonAveragePoints

		private bool _recalculateTotalSeasonAveragePoints = true;
		private double? _totalSeasonAveragePoints;
		public double TotalSeasonAveragePoints
		{
			get
			{
				if (_recalculateTotalSeasonAveragePoints)
				{
					_totalSeasonAveragePoints = Sum(p => p.Player.SeasonAveragePoints);
					_recalculateTotalSeasonAveragePoints = false;
				}
				return _totalSeasonAveragePoints == null ? 0d : _totalSeasonAveragePoints.Value;
			}
		}

		#endregion

		#endregion

		#region Methods

		private void OnPlayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			PlayersString = null;
			_recalculateTotalSalary = true;
			RaisePropertyChanged("TotalSalary");
			_recalculateProjectedCeiling = true;
			RaisePropertyChanged("TotalProjectedCeiling");
			_recalculateProjectedFloor = true;
			RaisePropertyChanged("TotalProjectedFloor");
			_recalculateTotalProjectedPoints = true;
			RaisePropertyChanged("TotalProjectedPoints");
			_recalculateTotalRecentAveragePoints = true;
			RaisePropertyChanged("TotalRecentAveragePoints");
			_recalculateTotalSeasonAveragePoints = true;
			RaisePropertyChanged("TotalSalary");
		}

		private double? Sum(Func<PlayerViewModel, double?> valueFunc)
		{
			double? sum = null;
			foreach (var player in Players)
			{
				var value = valueFunc(player);
				if (value != null)
				{
					if (sum == null)
					{
						sum = value.Value;
					}
					else
					{
						sum = sum.Value + value.Value;
					}
				}
			}
			return sum;
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
