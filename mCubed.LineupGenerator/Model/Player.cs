using System;
using System.Collections.Generic;
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
					RaisePropertyChanged("IsPlaying");
				}
			}
		}

		#endregion

		#region IsStarter

		private bool _isStarter;
		public bool IsStarter
		{
			get { return _isStarter; }
			set
			{
				if (_isStarter != value)
				{
					_isStarter = value;
					RaisePropertyChanged("IsStarter");
					RaisePropertyChanged("IsPlaying");
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

		#region Stats

		private IEnumerable<PlayerStats> _stats;
		public IEnumerable<PlayerStats> Stats
		{
			get { return _stats; }
			set
			{
				if (_stats != value)
				{
					_stats = value;
					RaisePropertyChanged("Stats");
					RaisePropertyChanged("ProjectedPoints");
					RaisePropertyChanged("RecentAveragePoints");
					RaisePropertyChanged("SeasonAveragePoints");
				}
			}
		}

		#endregion

		#region Team

		private string _team;
		public string Team
		{
			get { return _team; }
			set
			{
				if (_team != value)
				{
					_team = value;
					RaisePropertyChanged("Team");
				}
			}
		}

		#endregion

		#endregion

		#region Calculated Properties

		#region IsPlaying

		public bool IsPlaying
		{
			get { return IsStarter || IsProbablePitcher; }
		}

		#endregion

		#region ProjectedPoints

		public double ProjectedPoints
		{
			get { return Calculate(s => s.ProjectedPoints); }
		}

		#endregion

		#region RecentAveragePoints

		public double RecentAveragePoints
		{
			get { return Calculate(s => s.RecentAveragePoints); }
		}

		#endregion

		#region SeasonAveragePoints

		public double SeasonAveragePoints
		{
			get { return Calculate(s => s.SeasonAveragePoints); }
		}

		#endregion

		#endregion

		#region Methods

		private double Calculate(Func<PlayerStats, double?> property)
		{
			double total = 0d;
			int count = 0;
			var stats = Stats;
			if (stats != null)
			{
				foreach (var stat in stats)
				{
					if (stat != null)
					{
						var value = property(stat);
						if (value != null)
						{
							total += value.Value;
							count++;
						}
					}
				}
			}
			return count > 0 ? total / (double)count : 0d;
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
