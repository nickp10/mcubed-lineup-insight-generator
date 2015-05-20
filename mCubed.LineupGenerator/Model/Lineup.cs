using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace mCubed.LineupGenerator.Model
{
	public class Lineup : INotifyPropertyChanged
	{
		#region Constructors

		public Lineup(params Player[] players)
		{
			_players = new ObservableCollection<Player>(players);
		}

		#endregion

		#region Properties

		#region Players

		private ObservableCollection<Player> _players;
		public ObservableCollection<Player> Players
		{
			get
			{
				if (_players == null)
				{
					_players = new ObservableCollection<Player>();
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
					_playersString = Players.Any() ? Players.Select(p => p.Name).Aggregate((p1, p2) => p1 + ", " + p2) : string.Empty;
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

		private int _rating = 1;
		public int Rating
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
					_totalSalary = Players.Sum(p => p.Salary);
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

		#region TotalProjectedPoints

		private double? _totalProjectedPoints;
		public double TotalProjectedPoints
		{
			get
			{
				if (_totalProjectedPoints == null)
				{
					_totalProjectedPoints = Players.Sum(p => p.ProjectedPoints);
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
					_totalRecentAveragePoints = Players.Sum(p => p.RecentAveragePoints);
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
					_totalSeasonAveragePoints = Players.Sum(p => p.SeasonAveragePoints);
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
			TotalProjectedPointsSetter = null;
			TotalRecentAveragePointsSetter = null;
			TotalSeasonAveragePointsSetter = null;
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
