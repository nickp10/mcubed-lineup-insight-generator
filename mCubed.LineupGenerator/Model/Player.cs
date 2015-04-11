using System.ComponentModel;

namespace mCubed.LineupGenerator.Model
{
	public class Player : INotifyPropertyChanged
	{
		#region Constructors

		public Player()
		{
		}

		public Player(string name, string position, int salary)
		{
			Name = name;
			Position = position;
			Salary = salary;
		}

		#endregion

		#region Properties

		#region IncludeInLineups

		private bool _includeInLineups;
		public bool IncludeInLineups
		{
			get { return _includeInLineups; }
			set
			{
				if (_includeInLineups != value)
				{
					_includeInLineups = value;
					RaisePropertyChanged("IncludeInLineups");
				}
			}
		}

		#endregion

		#region Injury

		private InjuryData _injury;
		public InjuryData Injury
		{
			get { return _injury; }
			set
			{
				if (_injury != value)
				{
					_injury = value;
					RaisePropertyChanged("Injury");
				}
			}
		}

		#endregion

		#region IsProbablePitcher

		private bool _isProbablePitcher;
		public bool IsProbablePitcher
		{
			get { return _isProbablePitcher; }
			set
			{
				if (_isProbablePitcher != value)
				{
					_isProbablePitcher = value;
					RaisePropertyChanged("IsProbablePitcher");
				}
			}
		}

		#endregion

		#region Name

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					RaisePropertyChanged("Name");
				}
			}
		}

		#endregion

		#region PlayerStats

		private PlayerStats _playerStats;
		public PlayerStats PlayerStats
		{
			get { return _playerStats; }
			set
			{
				if (_playerStats != value)
				{
					_playerStats = value;
					RaisePropertyChanged("PlayerStats");
				}
			}
		}

		#endregion

		#region Position

		private string _position;
		public string Position
		{
			get { return _position; }
			set
			{
				if (_position != value)
				{
					_position = value;
					RaisePropertyChanged("Position");
				}
			}
		}

		#endregion

		#region Salary

		private int _salary;
		public int Salary
		{
			get { return _salary; }
			set
			{
				if (_salary != value)
				{
					_salary = value;
					RaisePropertyChanged("Salary");
				}
			}
		}

		#endregion

		#region SeasonAveragePoints

		private double _seasonAveragePoints;
		public double SeasonAveragePoints
		{
			get { return _seasonAveragePoints; }
			set
			{
				if (_seasonAveragePoints != value)
				{
					_seasonAveragePoints = value;
					RaisePropertyChanged("SeasonAveragePoints");
				}
			}
		}

		#endregion

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
