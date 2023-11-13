using UnityEngine;
public class GS_CharacterSelectLayoutBase : MonoBehaviourExtension
{
	[SerializeField]
	[Header("モ\u30fcドセレクトクラス")]
	protected GS_ModeSelect modeSelect;
	public void StartPlayKing()
	{
		SingletonCustom<GameSettingManager>.Instance.InitSportsDay();
		SingletonCustom<GameSettingManager>.Instance.SelectGameProgressType = GameSettingManager.GameProgressType.ALL_SPORTS;
		SingletonCustom<SceneManager>.Instance.NextScene(SceneManager.SceneType.PLAY_KING_OP);
	}
}
