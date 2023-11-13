using GamepadInput;
using UnityEngine;
public class GS_SwimmingMode : MonoBehaviour
{
	[SerializeField]
	[Header("選択フレ\u30fcム")]
	private GameObject[] arraySelectFrame;
	[SerializeField]
	[Header("テキスト")]
	private SpriteRenderer[] arrayText;
	public void Set(GS_Define.GameType _type)
	{
		base.gameObject.SetActive(value: true);
		SetFocus(SingletonCustom<GameSettingManager>.Instance.SelectGameMode);
	}
	public void Close()
	{
		base.gameObject.SetActive(value: false);
	}
	private void SetFocus(int _selectIdx)
	{
		for (int i = 0; i < arraySelectFrame.Length; i++)
		{
			arraySelectFrame[i].SetActive(i == _selectIdx);
			arrayText[i].color = ((i == _selectIdx) ? GS_Define.COLOR_SELECT_TEXT_FOCUS : GS_Define.COLOR_SELECT_TEXT_DISABLE);
		}
	}
	private void Update()
	{
		if (SingletonCustom<JoyConManager>.Instance.GetMainPlayerButtonDown(SatGamePad.Button.X))
		{
			SingletonCustom<GameSettingManager>.Instance.SelectGameMode = (SingletonCustom<GameSettingManager>.Instance.SelectGameMode + 1) % arraySelectFrame.Length;
			SetFocus(SingletonCustom<GameSettingManager>.Instance.SelectGameMode);
			SingletonCustom<AudioManager>.Instance.SePlay("se_button_common");
		}
	}
}
