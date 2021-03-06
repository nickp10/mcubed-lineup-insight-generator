﻿using System.Collections.Generic;
using System.ComponentModel;
using mCubed.LineupGenerator.Controller;
using mCubed.LineupGenerator.Utilities;

namespace mCubed.LineupGenerator.Model
{
	public class PositionPlayers : INotifyPropertyChanged, ISortable
	{
		#region Properties

		#region Players

		private List<PlayerViewModel> _players;
		public List<PlayerViewModel> Players
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
						_playersView.Sort("Player.Salary", ListSortDirection.Descending);
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

		#region PositionLabel

		private string _positionLabel;
		public string PositionLabel
		{
			get { return _positionLabel; }
			set
			{
				if (_positionLabel != value)
				{
					_positionLabel = value;
					RaisePropertyChanged("PositionLabel");
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

		#region ISortable Members

		public void Sort(string property, ListSortDirection initialSortDirection)
		{
			PlayersView.Sort(property, initialSortDirection);
		}

		public ListSortDirection? SortDirection
		{
			get { return PlayersView.SortDirection; }
		}

		public string SortProperty
		{
			get { return PlayersView.SortProperty; }
		}

		#endregion
	}
}
