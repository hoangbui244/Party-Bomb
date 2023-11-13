using UnityEngine;
public class GS_MoveCharacter : MonoBehaviour
{
	[SerializeField]
	[Header("キャラ画像")]
	private SpriteRenderer spCharacter;
	[SerializeField]
	[Header("アニメ\u30fcション文字列")]
	private string[] arrayAnimStr;
	private float ANIM_CHANGE_TIME = 0.5f;
	private float animTime;
	private int animIdx;
	public void Init()
	{
		arrayAnimStr[0] = "character_" + (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0] + 1).ToString() + "_00";
		arrayAnimStr[1] = "character_" + (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0] + 1).ToString() + "_01";
		arrayAnimStr[2] = "character_" + (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0] + 1).ToString() + "_02";
		arrayAnimStr[3] = "character_" + (SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[0] + 1).ToString() + "_01";
		animIdx = 0;
		spCharacter.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, arrayAnimStr[animIdx]);
		animTime = 0f;
	}
	public void SetFlip(bool _isFlip)
	{
		spCharacter.flipX = _isFlip;
	}
	public void SetDirectMove(bool _isEnable)
	{
	}
	private void Update()
	{
		animTime += Time.deltaTime;
		if (animTime >= ANIM_CHANGE_TIME)
		{
			animIdx = (animIdx + 1) % arrayAnimStr.Length;
			spCharacter.sprite = SingletonCustom<SAManager>.Instance.GetSprite(SAType.Main, arrayAnimStr[animIdx]);
			animTime = 0f;
		}
	}
}
