using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using mCubed.LineupGenerator.Controller;

namespace mCubed.LineupGenerator.Model
{
	public abstract class Contest : INotifyPropertyChanged
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
					RaisePropertyChanged("PlayersGrouped");
				}
			}
		}

		#endregion

		#region PlayersDictionary

		public IDictionary<string, Player> PlayersDictionary
		{
			get
			{
				var players = Players;
				return players == null ? new Dictionary<string, Player>() : players.Distinct(new PlayerNameComparer()).ToDictionary(k => k.Name, v => v);
			}
		}

		#endregion

		#region PlayersGrouped

		public IEnumerable<IGrouping<string, Player>> PlayersGrouped
		{
			get
			{
				var players = Players;
				var positions = Positions;
				if (players == null || !players.Any() || positions == null)
				{
					return Enumerable.Empty<IGrouping<string, Player>>();
				}
				else
				{
					return players.GroupBy(p => p.Position).OrderBy(g => g.Key, new PositionComparer(positions));
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
					RaisePropertyChanged("PlayersGrouped");
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
	}
}
