using UnityEngine;
using UnityEngine.Extension;
using UnityEngine.U2D;
public class Shuriken_JoyConButton : DecoratedMonoBehaviour
{
	private enum Button
	{
		A,
		B,
		X,
		Y,
		L,
		R,
		LStick,
		RStick
	}
	private static readonly string[] FullKeySpriteNames = new string[8]
	{
		"_b_operation_button_A",
		"_b_operation_button_B",
		"_b_operation_button_X",
		"_b_operation_button_Y",
		"_b_operation_button_L",
		"_b_operation_button_R",
		"_b_operation_button_L_stick",
		"_b_operation_button_R_stick"
	};
	private static readonly string[] JoyLeftOrRightSpriteNames = new string[8]
	{
		"_operation_button_cross",
		"_operation_button_cross",
		"_operation_button_cross",
		"_operation_button_cross",
		"_b_operation_button_SL",
		"_b_operation_button_SR",
		"_b_operation_button_stick",
		"_b_operation_button_stick"
	};
	[SerializeField]
	[DisplayName("対象ボタン")]
	private Button button;
	[SerializeField]
	[DisplayName("レンダラ\u30fc")]
	private SpriteRenderer buttonIcon;
	[SerializeField]
	[DisplayName("スプライトアトラス")]
	private SpriteAtlas spriteAtlas;
	private void OnEnable()
	{
		bool isSinglePlay = SingletonCustom<GameSettingManager>.Instance.IsSinglePlay;
		int num = (int)button;
		if (isSinglePlay)
		{
			buttonIcon.sprite = spriteAtlas.GetSprite(FullKeySpriteNames[num]);
			buttonIcon.transform.localRotation = Quaternion.identity;
			return;
		}
		buttonIcon.sprite = spriteAtlas.GetSprite(JoyLeftOrRightSpriteNames[(int)button]);
		if (num <= 3)
		{
			int num2 = 270 - 90 * num;
			buttonIcon.transform.localEulerAngles = new Vector3(0f, 0f, num2);
		}
	}
}
