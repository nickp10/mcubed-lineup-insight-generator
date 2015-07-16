
namespace mCubed.LineupGenerator.Model
{
	public class FanDuelContest : Contest
	{
		#region Properties

		#region ContestURL

		public string ContestURL { get; set; }

		#endregion

		#region PlayersURL

		public string PlayersURL { get; set; }

		#endregion

		#region Source

		protected override string Source { get { return "FanDuel"; } }

		#endregion

		#endregion
	}
}
