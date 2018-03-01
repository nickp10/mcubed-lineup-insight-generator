using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using mCubed.LineupGenerator.Model;
using mCubed.LineupGenerator.Services;
using mCubed.LineupGenerator.Utilities;

namespace mCubed.LineupGenerator.Controller
{
	public class ContestViewModel : INotifyPropertyChanged, ISortable
	{
		#region Properties

		#region Contest

		public Contest Contest { get; private set; }

		#endregion

		#region PlayersGrouped

		private List<PositionPlayers> _playersGrouped;
		public List<PositionPlayers> PlayersGrouped
		{
			get
			{
				if (_playersGrouped == null)
				{
					var games = Contest.Games;
					var positions = Contest.Positions;
					if (games == null || !games.Any() || positions == null)
					{
						_playersGrouped = new List<PositionPlayers>();
					}
					else
					{
						var players = Enumerable.Empty<PlayerViewModel>();
						foreach (var game in games)
						{
							players = players.
								Concat(game.AwayTeam.Players.Select(p => new PlayerViewModel(Contest, game, false, p))).
								Concat(game.HomeTeam.Players.Select(p => new PlayerViewModel(Contest, game, true, p)));
						}
						_playersGrouped = players.
							GroupBy(p => p.Player.Position).
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
		}

		#endregion

		#region Title

		public string Title
		{
			get
			{
				var builder = new StringBuilder().Append(Contest.Sport).Append(" ").Append(Contest.ContestType);
				var label = Contest.Label;
				if (label != null)
				{
					builder.Append(" (").Append(label).Append(")");
				}
				return builder.ToString();
			}
		}

		#endregion

		#endregion

		#region Constructors

		public ContestViewModel(Contest contest)
		{
			Contest = contest;
		}

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
