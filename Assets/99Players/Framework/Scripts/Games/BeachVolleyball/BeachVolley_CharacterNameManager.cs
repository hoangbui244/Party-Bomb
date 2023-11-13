using UnityEngine;
public class BeachVolley_CharacterNameManager : SingletonCustom<BeachVolley_CharacterNameManager>
{
	[SerializeField]
	[Header("キャラ名")]
	private BeachVolley_CharacterName[] characterName;
	[SerializeField]
	[Header("ゲ\u30fcム中プレイヤ\u30fcアイコン")]
	private GameObject[] playerIcons;
	[SerializeField]
	[Header("時間オブジェクト")]
	private RectTransform[] timeObject;
	private float[] showPosOffset = new float[4];
	public void Init()
	{
		if (!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay)
		{
			for (int i = 0; i < showPosOffset.Length; i++)
			{
				showPosOffset[i] = 85f;
			}
		}
		else
		{
			showPosOffset[0] = 60f;
			showPosOffset[1] = 90f;
		}
		for (int j = 0; j < 5; j++)
		{
			timeObject[j].SetLocalPositionY(50f);
		}
	}
	public void UpdateNameObj()
	{
		int pLAYER_NUM = BeachVolley_Define.PLAYER_NUM;
		for (int i = 0; i < 5; i++)
		{
			if (i >= pLAYER_NUM)
			{
				continue;
			}
			if (pLAYER_NUM == 1)
			{
				playerIcons[0].gameObject.SetActive(value: false);
			}
			BeachVolley_Character controlChara = SingletonCustom<BeachVolley_MainCharacterManager>.Instance.GetControlChara(i);
			if (!(controlChara == null))
			{
				characterName[i].gameObject.SetActive(!controlChara.CheckCharaHide() && !SingletonCustom<BeachVolley_MainGameManager>.Instance.CheckGameState(BeachVolley_MainGameManager.GameState.SET_INTERVAL));
				if (characterName[i].gameObject.activeSelf)
				{
					characterName[i].SetName(controlChara.GetUniformNumber().ToString() + ". " + controlChara.GetName());
					CalcManager.mCalcVector3 = SingletonCustom<BeachVolley_FieldManager>.Instance.Get3dCamera().WorldToScreenPoint(controlChara.GetPos());
					CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
					CalcManager.mCalcVector3.y -= showPosOffset[i];
					characterName[i].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, characterName[i].transform.position.z);
				}
			}
		}
	}
	public void SetCnt(int _teamNo, int _num, int _playerNo = -1)
	{
	}
	public void HideCnt(int _teamNo)
	{
		characterName[_teamNo].HideCnt();
	}
	public BeachVolley_CharacterName GetCharacterName(int _teamNo)
	{
		return characterName[_teamNo];
	}
}
