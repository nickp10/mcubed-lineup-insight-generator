using System.Collections.Generic;
using System.ComponentModel;
using mCubed.LineupGenerator.Controller;
using mCubed.LineupGenerator.Utilities;

namespace mCubed.LineupGenerator.Model
{
	public class PositionPlayers : INotifyPropertyChanged
	{
		#region Properties

		#region Players

		private List<Player> _players;
		public List<Player> Players
		{
			get { return _players; }
			set
			{
				if (_players != value)
				{
					_players = value;
					RaisePropertyChanged("Players");
					PlayersView = null;
				}
			}
		}

		#endregion

		#region PlayersView

		private SortableListCollectionView _playersView;
		public SortableListCollectionView PlayersView
		{
			get
			{
				if (_playersView == null)
				{
					Utils.DispatcherInvoke(() =>
					{
						_playersView = new SortableListCollectionView(Players);
						_playersView.Sort("Salary", ListSortDirection.Descending);
					});
				}
				return _playersView;
			}
			private set
			{
				if (_playersView != value)
				{
					_playersView = value;
					RaisePropertyChanged("PlayersView");
				}
			}
		}

		#endregion

		#region Position

		private string _positions;
		public string Position
		{
			get { return _positions; }
			set
			{
				if (_positions != value)
				{
					_positions = value;
					RaisePropertyChanged("Position");
				}
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
