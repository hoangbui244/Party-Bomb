using System.Runtime.InteropServices;
public class Shooting_StageManager : SingletonCustom<Shooting_StageManager>
{
	public enum StageSeasonType
	{
		STAGE_SPRING,
		STAGE_SUMMER,
		STAGE_AUTUMN,
		STAGE_WINTER,
		NONE
	}
	public enum StageWeatherType
	{
		SUNNY,
		EVENING,
		NIGHT,
		RAIN,
		CLOUDY,
		SNOW
	}
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct staegeData
	{
	}
	private StageSeasonType stageSeasonType;
	private StageWeatherType stageWeatherType;
	public void Init()
	{
	}
}
