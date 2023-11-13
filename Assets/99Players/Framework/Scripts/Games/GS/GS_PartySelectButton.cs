using UnityEngine;
public class GS_PartySelectButton : MonoBehaviour
{
	[SerializeField]
	[Header("カ\u30fcソルマネ\u30fcジャ\u30fc")]
	private CursorManager cursor;
	[SerializeField]
	[Header("選択状態フェ\u30fcド")]
	private GameObject[] arraySelectFade;
	[SerializeField]
	[Header("選択番号")]
	private SpriteRenderer[] arrayRenderSelectNumber;
	[SerializeField]
	[Header("一段目：右端オブジェクト")]
	private CursorButtonObject firstStepRightObj;
	[SerializeField]
	[Header("二段目：右端オブジェクト")]
	private CursorButtonObject secondStepRightObj;
	[SerializeField]
	[Header("三段目：右端オブジェクト")]
	private CursorButtonObject thirdStepRightObj;
	public CursorManager GetCursorManager()
	{
		return cursor;
	}
	public int GetSelectFadeLen()
	{
		return arraySelectFade.Length;
	}
	public GameObject GetSelectFade(int _idx)
	{
		return arraySelectFade[_idx];
	}
	public int GetRenderSelectNumberLen()
	{
		return arrayRenderSelectNumber.Length;
	}
	public SpriteRenderer GetRenderSelectNumber(int _idx)
	{
		return arrayRenderSelectNumber[_idx];
	}
	public CursorButtonObject GetFirstStepRightObj()
	{
		return firstStepRightObj;
	}
	public CursorButtonObject GetSecondStepRightObj()
	{
		return secondStepRightObj;
	}
	public CursorButtonObject GetThirdStepRightObj()
	{
		return thirdStepRightObj;
	}
}
