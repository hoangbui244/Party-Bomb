using UnityEngine;
public class RoadRaceStageScript : MonoBehaviour
{
	[SerializeField]
	[Header("夜設定")]
	private bool IS_NIGHT;
	[SerializeField]
	[Header("夜用フェ\u30fcド")]
	private Transform nightFade;
	[SerializeField]
	[Header("生成アンカ\u30fc")]
	private Transform[] createAnchor;
	[SerializeField]
	[Header("AIポイント")]
	private RoadRaceAipoint[] aiPoints;
	[SerializeField]
	[Header("レ\u30fcスのチェックポイント")]
	private Collider[] raceCheckPoints;
	[SerializeField]
	[Header("アクションポイント")]
	private RoadRaceActionPoint[] actionPoints;
	[SerializeField]
	[Header("スタ\u30fcト板")]
	private Transform startStopper;
	[SerializeField]
	[Header("コ\u30fcト上オブジェクト配置パタ\u30fcン")]
	private GameObject[] OBJECT_PATTERN_LIST;
	[SerializeField]
	[Header("オブジェクトアンカ\u30fc")]
	private Transform OBJECT_ANCHOR;
	private GameObject patternObj;
	private int patternNo = -1;
	[SerializeField]
	[Header("サイズアンカ\u30fc(右上)")]
	private Transform sizeAnchorRightTop;
	[SerializeField]
	[Header("サイズアンカ\u30fc(左下)")]
	private Transform sizeAnchorLeftBottom;
	public Transform[] CreateAnchor => createAnchor;
	public RoadRaceAipoint[] AiPoints => aiPoints;
	public Collider[] RaceCheckPoints => raceCheckPoints;
	public RoadRaceActionPoint[] ActionPoints => actionPoints;
	public Transform StartStopper => startStopper;
	public Transform SizeAnchorRightTop => sizeAnchorRightTop;
	public Transform SizeAnchorLeftBottom => sizeAnchorLeftBottom;
	public void Init()
	{
		bool iS_NIGHT = IS_NIGHT;
		RandomObjectPattern();
	}
	private void OnDestroy()
	{
		if (IS_NIGHT)
		{
			UnityEngine.Object.Destroy(nightFade.gameObject);
		}
	}
	public void RandomObjectPattern()
	{
		if (OBJECT_PATTERN_LIST.Length != 0 && !(OBJECT_ANCHOR == null))
		{
			if (patternObj != null)
			{
				UnityEngine.Object.Destroy(patternObj);
			}
			int num = patternNo;
			do
			{
				patternNo = UnityEngine.Random.Range(0, OBJECT_PATTERN_LIST.Length);
			}
			while (patternNo == num);
			patternObj = UnityEngine.Object.Instantiate(OBJECT_PATTERN_LIST[patternNo], OBJECT_ANCHOR);
			patternObj.transform.localPosition = Vector3.zero;
			patternObj.transform.localRotation = Quaternion.identity;
			patternObj.transform.localScale = Vector3.one;
		}
	}
	public RoadRaceActionPoint GetPoint(GameObject _obj)
	{
		for (int i = 0; i < actionPoints.Length; i++)
		{
			if (actionPoints[i].gameObject == _obj)
			{
				return actionPoints[i];
			}
		}
		if (actionPoints.Length == 0)
		{
			return null;
		}
		return actionPoints[0];
	}
}
