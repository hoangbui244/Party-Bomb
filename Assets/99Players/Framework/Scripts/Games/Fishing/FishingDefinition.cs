public class FishingDefinition
{
	public enum AiStrength
	{
		Weak,
		Normal,
		Strong
	}
	public enum User
	{
		Player1,
		Player2,
		Player3,
		Player4,
		Cpu1,
		Cpu2,
		Cpu3,
		Cpu4,
		Cpu5
	}
	public enum TeamType
	{
		TeamA,
		TeamB
	}
	public enum BiteDifficulty
	{
		Easy,
		Normal,
		Hard
	}
	public enum FishSizeType
	{
		Small,
		Medium,
		Large,
		Garbage
	}
	public enum FishType
	{
		Ayu,
		Koi,
		Nizimasu,
		Yamame,
		Iwana,
		PetBottle1,
		PetBottle2,
		PetBottle3,
		Sandal,
		Max
	}
	public enum ShakeStrengthType
	{
		Weak,
		Normal,
		Strong
	}
	public static int MedalBronzePoint = 2000;
	public static int MedalSilverPoint = 2500;
	public static int MedalGoldPoint = 3000;
	public static string TagField = "Field";
	public static string TagNoHit = "NoHit";
	public static string TagObject = "Object";
	public static string TagPlayer = "Player";
	public static string LayerField = "Field";
	public static string LayerWall = "Wall";
	public static string LayerCharacter = "Character";
	public static Fishing_MainGame MGM => SingletonCustom<Fishing_MainGame>.Instance;
	public static Fishing_Characters MCM => SingletonCustom<Fishing_Characters>.Instance;
	public static Fishing_Input CM => SingletonCustom<Fishing_Input>.Instance;
	public static Fishing_Fishes FDM => SingletonCustom<Fishing_Fishes>.Instance;
	public static Fishing_CharacterUIManager CUIM => SingletonCustom<Fishing_CharacterUIManager>.Instance;
	public static Fishing_GameUI GUIM => SingletonCustom<Fishing_GameUI>.Instance;
	public static Fishing_FishShadows FSM => SingletonCustom<Fishing_FishShadows>.Instance;
	public static Fishing_Field FM => SingletonCustom<Fishing_Field>.Instance;
}
