using UnityEngine;
public class MorphingRace_GenerateObstacle : MonoBehaviour
{
	[SerializeField]
	[Header("デバッグ用：障害物を固定にするかどうかのフラグ")]
	private bool isOnlyObstacle;
	[SerializeField]
	[Header("デバッグ用：固定する障害物のIdx（生成する障害物の配列数 - 1 以内の数値を設定）")]
	private int onlyObstackeIdx;
	[SerializeField]
	[Header("生成する位置アンカ\u30fc")]
	private Transform[] arrayGenerateAnchor;
	[SerializeField]
	[Header("生成する障害物プレハブ")]
	private GameObject[] arrayGenerateObstaclePref;
	[SerializeField]
	[Header("スタ\u30fcト用のオブジェクトを生成するかどうかのフラグ")]
	private bool isGenerateStart;
	[SerializeField]
	[Header("生成するスタ\u30fcト用のアンカ\u30fc")]
	private Transform generateStartAnchor;
	[SerializeField]
	[Header("生成するスタ\u30fcトプレハブ")]
	private GameObject[] arrayGenerateStartGatePref;
	[SerializeField]
	[Header("ゴ\u30fcル用のオブジェクトを生成するかどうかのフラグ")]
	private bool isGenerateGoal;
	[SerializeField]
	[Header("生成するゴ\u30fcル用のアンカ\u30fc")]
	private Transform generateGoalAnchor;
	[SerializeField]
	[Header("生成するゴ\u30fcルプレハブ")]
	private GameObject[] arrayGenerateGoalGatePref;
	public void GenerateObstacle(MorphingRace_FieldManager.TargetPrefType _morphingTargetType)
	{
		isOnlyObstacle = false;
		if (isOnlyObstacle)
		{
			for (int i = 0; i < arrayGenerateAnchor.Length; i++)
			{
				Object.Instantiate(arrayGenerateObstaclePref[onlyObstackeIdx], arrayGenerateAnchor[i]);
			}
		}
		else
		{
			arrayGenerateObstaclePref.Shuffle();
			for (int j = 0; j < arrayGenerateAnchor.Length; j++)
			{
				if (j >= arrayGenerateObstaclePref.Length)
				{
					Object.Instantiate(arrayGenerateObstaclePref[Random.Range(0, arrayGenerateObstaclePref.Length)], arrayGenerateAnchor[j]);
				}
				else
				{
					Object.Instantiate(arrayGenerateObstaclePref[j], arrayGenerateAnchor[j]);
				}
			}
		}
		if (isGenerateStart && generateStartAnchor != null && arrayGenerateGoalGatePref.Length != 0)
		{
			arrayGenerateStartGatePref.Shuffle();
			Object.Instantiate(arrayGenerateStartGatePref[0], generateStartAnchor);
		}
		if (isGenerateGoal && generateGoalAnchor != null && arrayGenerateGoalGatePref.Length != 0)
		{
			arrayGenerateGoalGatePref.Shuffle();
			Object.Instantiate(arrayGenerateGoalGatePref[0], generateGoalAnchor);
		}
	}
	public Transform[] GetArrayGenerateAnchor()
	{
		return arrayGenerateAnchor;
	}
	public Transform GetGenerateStartAnchor()
	{
		return generateStartAnchor;
	}
	public Transform GetGenerateGoalAnchor()
	{
		return generateGoalAnchor;
	}
	public MeshRenderer[] GetArrayStartGateMesh()
	{
		return generateStartAnchor.transform.GetComponentsInChildren<MeshRenderer>();
	}
	public MeshRenderer[] GetArrayGoalGateMesh()
	{
		return generateGoalAnchor.transform.GetComponentsInChildren<MeshRenderer>();
	}
}
