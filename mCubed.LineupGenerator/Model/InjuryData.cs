
namespace mCubed.LineupGenerator.Model
{
	public class InjuryData
	{
		public string Display { get; set; }
		public InjuryType InjuryType { get; set; }

		public InjuryData()
		{
		}

		public InjuryData(string display, InjuryType injuryType)
		{
			Display = display;
			InjuryType = injuryType;
		}
	}
}
