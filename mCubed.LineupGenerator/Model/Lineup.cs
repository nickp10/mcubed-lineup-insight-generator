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

		private int? _totalSalary;
		public int TotalSalary
		{
			get
			{
				if (_totalSalary == null)
				{
					_totalSalary = Players.Sum(p => p.Player.Salary);
				}
				return _totalSalary.Value;
			}
		}
		private int? TotalSalarySetter
		{
			set
			{
				if (_totalSalary != value)
				{
					_totalSalary = value;
					RaisePropertyChanged("TotalSalary");
				}
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

		private double? _totalProjectedPoints;
		public double TotalProjectedPoints
		{
			get
			{
				if (_totalProjectedPoints == null)
				{
					_totalProjectedPoints = Players.Sum(p => p.Player.ProjectedPoints ?? 0d);
				}
				return _totalProjectedPoints.Value;
			}
		}
		private double? TotalProjectedPointsSetter
		{
			set
			{
				if (_totalProjectedPoints != value)
				{
					_totalProjectedPoints = value;
					RaisePropertyChanged("TotalProjectedPoints");
				}
			}
		}

		#endregion

		#region TotalRecentAveragePoints

		private double? _totalRecentAveragePoints;
		public double TotalRecentAveragePoints
		{
			get
			{
				if (_totalRecentAveragePoints == null)
				{
					_totalRecentAveragePoints = Players.Sum(p => p.Player.RecentAveragePoints ?? 0d);
				}
				return _totalRecentAveragePoints.Value;
			}
		}
		private double? TotalRecentAveragePointsSetter
		{
			set
			{
				if (_totalRecentAveragePoints != value)
				{
					_totalRecentAveragePoints = value;
					RaisePropertyChanged("TotalRecentAveragePoints");
				}
			}
		}

		#endregion

		#region TotalSeasonAveragePoints

		private double? _totalSeasonAveragePoints;
		public double TotalSeasonAveragePoints
		{
			get
			{
				if (_totalSeasonAveragePoints == null)
				{
					_totalSeasonAveragePoints = Players.Sum(p => p.Player.SeasonAveragePoints ?? 0d);
				}
				return _totalSeasonAveragePoints.Value;
			}
		}
		private double? TotalSeasonAveragePointsSetter
		{
			set
			{
				if (_totalSeasonAveragePoints != value)
				{
					_totalSeasonAveragePoints = value;
					RaisePropertyChanged("TotalSeasonAveragePoints");
				}
			}
		}

		#endregion

		#endregion

		#region Methods

		private void OnPlayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			PlayersString = null;
			TotalSalarySetter = null;
			_recalculateProjectedCeiling = true;
			RaisePropertyChanged("TotalProjectedCeiling");
			_recalculateProjectedFloor = true;
			RaisePropertyChanged("TotalProjectedFloor");
			TotalProjectedPointsSetter = null;
			TotalRecentAveragePointsSetter = null;
			TotalSeasonAveragePointsSetter = null;
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
