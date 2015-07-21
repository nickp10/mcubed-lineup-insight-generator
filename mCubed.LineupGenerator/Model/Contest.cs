using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using mCubed.LineupGenerator.Controller;
using mCubed.LineupGenerator.Utilities;

namespace mCubed.LineupGenerator.Model
{
	public abstract class Contest : INotifyPropertyChanged, ISortable
	{
		#region Properties

		#region IsDataRetrieved

		public bool IsDataRetrieved { get; set; }

		#endregion

		#region Label

		private string _label;
		public string Label
		{
			get { return _label; }
			set
			{
				if (_label != value)
				{
					_label = value;
					RaisePropertyChanged("Label");
					RaisePropertyChanged("Title");
				}
			}
		}

		#endregion

		#region MaxPlayersPerTeam

		public int MaxPlayersPerTeam
		{
			get { return 4; }
		}

		#endregion

		#region MaxSalary

		private int _maxSalary;
		public int MaxSalary
		{
			get { return _maxSalary; }
			set
			{
				if (_maxSalary != value)
				{
					_maxSalary = value;
					RaisePropertyChanged("MaxSalary");
				}
			}
		}

		#endregion

		#region Players

		private IEnumerable<Player> _players;
		public IEnumerable<Player> Players
		{
			get { return _players; }
			set
			{
				if (_players != value)
				{
					_players = value;
					RaisePropertyChanged("Players");
					PlayersDictionary = null;
					PlayersGrouped = null;
				}
			}
		}

		#endregion

		#region PlayersDictionary

		private IDictionary<string, Player> _playersDictionary;
		public IDictionary<string, Player> PlayersDictionary
		{
			get
			{
				if (_playersDictionary == null)
				{
					var players = Players;
					if (players == null)
					{
						_playersDictionary = new Dictionary<string, Player>(StringComparer.OrdinalIgnoreCase);
					}
					else
					{
						_playersDictionary = players.Distinct(new PlayerNameComparer()).ToDictionary(k => k.Name, v => v, StringComparer.OrdinalIgnoreCase);
					}
				}
				return _playersDictionary;
			}
			private set
			{
				if (_playersDictionary != value)
				{
					_playersDictionary = value;
					RaisePropertyChanged("PlayersDictionary");
				}
			}
		}

		#endregion

		#region PlayersGrouped

		private List<PositionPlayers> _playersGrouped;
		public List<PositionPlayers> PlayersGrouped
		{
			get
			{
				if (_playersGrouped == null)
				{
					var players = Players;
					var positions = Positions;
					if (players == null || !players.Any() || positions == null)
					{
						_playersGrouped = new List<PositionPlayers>();
					}
					else
					{
						_playersGrouped = players.
							GroupBy(p => p.Position).
							Select(g => new PositionPlayers
							{
								Position = g.Key,
								Players = g.ToList()
							}).
							OrderBy(g => g.Position, new PositionComparer(positions)).
							ToList();
					}
				}
				return _playersGrouped;
			}
			private set
			{
				if (_playersGrouped != value)
				{
					_playersGrouped = value;
					RaisePropertyChanged("PlayersGrouped");
					PlayersGroupedView = null;
				}
			}
		}

		#endregion

		#region PlayersGroupedView

		private ListCollectionView _playersGroupedView;
		public ListCollectionView PlayersGroupedView
		{
			get
			{
				if (_playersGroupedView == null)
				{
					Utils.DispatcherInvoke(() =>
					{
						_playersGroupedView = new ListCollectionView(PlayersGrouped);
					});
				}
				return _playersGroupedView;
			}
			private set
			{
				if (_playersGroupedView != value)
				{
					_playersGroupedView = value;
					RaisePropertyChanged("PlayersGroupedView");
				}
			}
		}

		#endregion

		#region Positions

		private IEnumerable<string> _positions;
		public IEnumerable<string> Positions
		{
			get { return _positions; }
			set
			{
				if (_positions != value)
				{
					_positions = value;
					RaisePropertyChanged("Positions");
					PlayersGrouped = null;
				}
			}
		}

		#endregion

		#region Source

		protected abstract string Source { get; }

		#endregion

		#region Sport

		private string _sport;
		public string Sport
		{
			get { return _sport; }
			set
			{
				if (_sport != value)
				{
					_sport = value;
					RaisePropertyChanged("Sport");
					RaisePropertyChanged("Title");
				}
			}
		}

		#endregion

		#region Title

		public string Title
		{
			get
			{
				var builder = new StringBuilder(Sport).Append(" ").Append(Source);
				if (Label != null)
				{
					builder.Append(" (").Append(Label).Append(")");
				}
				return builder.ToString();
			}
		}

		#endregion

		#endregion

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string property)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(property));
			}
		}

		#endregion

		#region ISortable Members

		public void Sort(string property, ListSortDirection initialSortDirection = ListSortDirection.Ascending)
		{
			foreach (var p in PlayersGrouped)
			{
				p.Sort(property, initialSortDirection);
			}
			RaisePropertyChanged("SortDirection");
			RaisePropertyChanged("SortProperty");
		}

		public ListSortDirection? SortDirection
		{
			get
			{
				var p = PlayersGrouped.FirstOrDefault();
				return p == null ? null : p.SortDirection;
			}
		}

		public string SortProperty
		{
			get
			{
				var p = PlayersGrouped.FirstOrDefault();
				return p == null ? null : p.SortProperty;
			}
		}

		#endregion
	}
}
