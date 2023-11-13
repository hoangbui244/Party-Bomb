using UnityEngine;
public class GS_GameSelectTeacher : MonoBehaviour
{
	public enum TeacherType
	{
		DATE,
		HOUJYOU,
		HIME,
		HIMEBUSYOU,
		TOKUGAWA,
		MOURI,
		TOYOTOMI
	}
	[SerializeField]
	[Header("先生画像レンダラ")]
	private SpriteRenderer renderTeacher;
	[SerializeField]
	[Header("先生画像")]
	private Sprite[] arraySpTeacher;
	[SerializeField]
	[Header("定義")]
	private TeacherType[] arrayTeacherType;
	[SerializeField]
	[Header("初期座標")]
	private Vector3[] arrayInitPos;
	[SerializeField]
	[Header("移動後座標")]
	private Vector3[] arrayMoveEndPos;
	public void Show(int _gameIdx)
	{
		renderTeacher.sprite = arraySpTeacher[(int)arrayTeacherType[_gameIdx]];
		renderTeacher.transform.localPosition = arrayInitPos[(int)arrayTeacherType[_gameIdx]];
		LeanTween.cancel(renderTeacher.gameObject);
		LeanTween.moveLocal(renderTeacher.gameObject, arrayMoveEndPos[(int)arrayTeacherType[_gameIdx]], 0.35f).setEaseOutQuart();
	}
	private void OnDestroy()
	{
		LeanTween.cancel(renderTeacher.gameObject);
	}
}
