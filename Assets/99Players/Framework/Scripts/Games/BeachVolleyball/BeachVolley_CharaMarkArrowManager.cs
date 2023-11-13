using UnityEngine;
public class BeachVolley_CharaMarkArrowManager : MonoBehaviour
{
	[SerializeField]
	[Header("マ\u30fcク")]
	private Transform[] mark;
	[SerializeField]
	[Header("矢印アンカ\u30fc")]
	private Transform[] arrowAnchor;
	[SerializeField]
	[Header("表示範囲")]
	private Vector3 showRange;
	[SerializeField]
	[Header("表示範囲表示フラグ")]
	private bool isShowRange;
	private Vector3 markPos;
	private float COMPLIANCE_SPEED = 0.1f;
	private float dirOfChara;
	public void Init()
	{
		for (int i = 0; i < BeachVolley_Define.MCM.TeamList.Length; i++)
		{
			for (int j = 0; j < BeachVolley_Define.MCM.TeamList[i].charas.Length; j++)
			{
				if (!BeachVolley_Define.MCM.TeamList[i].charas[j].IsPlayer)
				{
					mark[i].gameObject.SetActive(value: false);
					arrowAnchor[i].gameObject.SetActive(value: false);
				}
			}
		}
	}
	public void UpdateMethod()
	{
		for (int i = 0; i < BeachVolley_Define.MCM.TeamList.Length; i++)
		{
			for (int j = 0; j < BeachVolley_Define.MCM.TeamList[i].charas.Length; j++)
			{
				bool isPlayer = BeachVolley_Define.MCM.TeamList[i].charas[j].IsPlayer;
			}
		}
	}
	private void SettingMarkAndArrow(int _teamNo, Transform _mark, Transform _arrow)
	{
		Vector3 vector = BeachVolley_Define.FM.Get3dCamera().WorldToScreenPoint(BeachVolley_Define.MCM.GetControlChara(_teamNo).GetPos());
		if (CheckScreenOut(vector) && BeachVolley_Define.MGM.CheckGameState(BeachVolley_MainGameManager.GameState.IN_PLAY))
		{
			markPos = Camera.main.ScreenToWorldPoint(vector);
			Vector3 vector2 = markPos;
			markPos.x = Mathf.Min(Mathf.Max(markPos.x, base.transform.position.x - showRange.x * 0.5f), base.transform.position.x + showRange.x * 0.5f);
			markPos.y = Mathf.Min(Mathf.Max(markPos.y, base.transform.position.y - showRange.y * 0.5f), base.transform.position.y + showRange.y * 0.5f);
			markPos.z = base.transform.position.z;
			if (mark[_teamNo].gameObject.activeSelf)
			{
				_mark.Translate((markPos - _mark.position) * COMPLIANCE_SPEED);
			}
			else
			{
				_mark.transform.position = markPos;
			}
			dirOfChara = Mathf.Atan2(vector2.y - markPos.y, vector2.x - markPos.x);
			CalcManager.mCalcVector3.x = (CalcManager.mCalcVector3.y = 0f);
			CalcManager.mCalcVector3.z = dirOfChara * 57.29578f + 90f;
			_arrow.transform.localRotation = Quaternion.Euler(CalcManager.mCalcVector3);
			mark[_teamNo].gameObject.SetActive(value: true);
			arrowAnchor[_teamNo].gameObject.SetActive(value: true);
		}
		else
		{
			mark[_teamNo].gameObject.SetActive(value: false);
			arrowAnchor[_teamNo].gameObject.SetActive(value: false);
		}
	}
	private bool CheckScreenOut(Vector3 _pos)
	{
		CalcManager.mCalcVector3 = Camera.main.ScreenToViewportPoint(_pos);
		if (CalcManager.mCalcVector3.x < -0f || CalcManager.mCalcVector3.x > 1f || CalcManager.mCalcVector3.y < -0f || CalcManager.mCalcVector3.y > 1f)
		{
			return true;
		}
		return false;
	}
	private void OnDrawGizmosSelected()
	{
		if (isShowRange)
		{
			Gizmos.color = ColorPalet.lightblue;
			Gizmos.DrawWireCube(base.transform.position, showRange);
		}
	}
}
