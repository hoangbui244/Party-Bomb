using TMPro;
using UnityEngine;
namespace BeachSoccer
{
	public class CharacterNameManager : SingletonCustom<CharacterNameManager>
	{
		[SerializeField]
		[Header("キャラ名リスト")]
		private TextMeshPro[] charaNameList;
		[SerializeField]
		[Header("キャラ名背景")]
		private SpriteRenderer[] charaNameBack;
		[SerializeField]
		[Header("縦向きカメラのときのプレイヤ\u30fcキャラ名")]
		private TextMeshPro singleCharaNameList;
		[SerializeField]
		[Header("スタミナゲ\u30fcジ")]
		private SpriteRenderer[] staminaGauge;
		[SerializeField]
		[Header("縦向きカメラのときのプレイヤ\u30fcスタミナゲ\u30fcジ")]
		private SpriteRenderer singleStaminaGauge;
		private float[] showPosOffset = new float[4];
		public void Init()
		{
			for (int i = 0; i < showPosOffset.Length; i++)
			{
				showPosOffset[i] = 140f;
			}
			if ((GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.SINGLE) && GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL)) || (GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.TOURNAMENT) && GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL)))
			{
				charaNameList[0] = singleCharaNameList;
				staminaGauge[0] = singleStaminaGauge;
				showPosOffset[0] = 95f;
				showPosOffset[1] = 160f;
			}
		}
		public void UpdateNameObj()
		{
			int num = 1;
			GameSaveData.MainGameMode selectMainGameMode = GameSaveData.GetSelectMainGameMode();
			if (selectMainGameMode == GameSaveData.MainGameMode.SINGLE || selectMainGameMode == GameSaveData.MainGameMode.TOURNAMENT)
			{
				num = 1;
				if (MainCharacterManager.IsGameWatchingMode() && GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.HORIZONTAL))
				{
					num = 2;
				}
			}
			else
			{
				num = GameSaveData.GetSelectMultiPlayerNum();
			}
			CharacterScript characterScript;
			for (int i = 0; i < num; i++)
			{
				charaNameList[i].gameObject.SetActive(value: false);
				characterScript = SingletonCustom<MainCharacterManager>.Instance.GetControlChara(i);
				if (MainCharacterManager.IsGameWatchingMode())
				{
					CharacterScript haveBallChara = SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara();
					if (haveBallChara != null)
					{
						if (GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL))
						{
							if (haveBallChara.TeamNo == 0)
							{
								characterScript = haveBallChara;
							}
						}
						else if (i == haveBallChara.TeamNo)
						{
							characterScript = haveBallChara;
						}
					}
				}
				charaNameList[i].gameObject.SetActive(characterScript != null && !characterScript.CheckCharaHide() && !SingletonCustom<MainGameManager>.Instance.CheckGameState(MainGameManager.GameState.HALF_TIME));
				if (charaNameList[i].gameObject.activeSelf)
				{
					charaNameList[i].text = characterScript.GetUniformNumber().ToString() + ". " + characterScript.GetName();
					staminaGauge[i].transform.SetLocalScaleX(characterScript.GetStaminaPer());
					CalcManager.mCalcVector3 = SingletonCustom<FieldManager>.Instance.Get3dCamera().WorldToScreenPoint(characterScript.GetPos());
					CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
					if ((GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.SINGLE) && GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL)) || (GameSaveData.CheckSelectMainGameMode(GameSaveData.MainGameMode.TOURNAMENT) && GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL)))
					{
						CalcManager.mCalcVector3.y -= showPosOffset[i];
					}
					else
					{
						CalcManager.mCalcVector3.y += showPosOffset[i];
					}
					charaNameList[i].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, charaNameList[i].transform.position.z);
				}
			}
			if (!GameSaveData.CheckSelectCameraMode(GameSaveData.CameraMode.VERTICAL))
			{
				return;
			}
			charaNameList[1].gameObject.SetActive(value: false);
			characterScript = null;
			if (SingletonCustom<MainCharacterManager>.Instance.CheckHaveBallTeam(1))
			{
				characterScript = SingletonCustom<MainCharacterManager>.Instance.GetHaveBallChara();
			}
			charaNameList[1].gameObject.SetActive(characterScript != null && !characterScript.CheckCharaHide() && !SingletonCustom<MainGameManager>.Instance.CheckGameState(MainGameManager.GameState.HALF_TIME));
			if (charaNameList[1].gameObject.activeSelf)
			{
				charaNameList[1].gameObject.SetActive(characterScript != null);
				if (characterScript != null)
				{
					charaNameList[1].text = characterScript.GetUniformNumber().ToString() + ". " + characterScript.GetName();
					staminaGauge[1].transform.SetLocalScaleX(characterScript.GetStaminaPer());
					CalcManager.mCalcVector3 = SingletonCustom<FieldManager>.Instance.Get3dCamera().WorldToScreenPoint(characterScript.GetPos());
					CalcManager.mCalcVector3 = SingletonCustom<GlobalCameraManager>.Instance.GetMainCamera<Camera>().ScreenToWorldPoint(CalcManager.mCalcVector3);
					CalcManager.mCalcVector3.y += showPosOffset[1];
					charaNameList[1].transform.SetPosition(CalcManager.mCalcVector3.x, CalcManager.mCalcVector3.y, charaNameList[1].transform.position.z);
				}
			}
		}
	}
}
