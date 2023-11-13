using UnityEngine;
public class GS_TeamAssignment : MonoBehaviour
{
	[SerializeField]
	[Header("フェ\u30fcド対象スプライト")]
	private SpriteRenderer[] arrayFadingSp;
	private bool isEnable = true;
	public bool IsEnable => isEnable;
	public void SetEnable()
	{
		for (int i = 0; i < arrayFadingSp.Length; i++)
		{
			arrayFadingSp[i].color = Color.white;
		}
		isEnable = true;
		base.gameObject.SetActive(value: true);
	}
	public void SetDisable()
	{
		for (int i = 0; i < arrayFadingSp.Length; i++)
		{
			arrayFadingSp[i].color = Color.gray;
		}
		isEnable = false;
		base.gameObject.SetActive(value: false);
	}
}
