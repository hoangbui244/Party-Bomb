using UnityEngine;
public class MorphingRace_MorphingTarget_Fish_SwimArea : MonoBehaviour
{
	[SerializeField]
	[Header("泳ぐル\u30fcト")]
	private BezierPosition[] arrraySwimRoot;
	private BezierPosition swimRoot;
	public void Init()
	{
		for (int i = 0; i < arrraySwimRoot.Length; i++)
		{
			arrraySwimRoot[i].gameObject.SetActive(value: false);
		}
		swimRoot = arrraySwimRoot[Random.Range(0, arrraySwimRoot.Length)];
		swimRoot.Init();
		swimRoot.gameObject.SetActive(value: true);
	}
	public BezierPosition GetSwimRoot()
	{
		return swimRoot;
	}
	public bool CheckPassSwimArea(int _idx, Vector3 _pos)
	{
		return swimRoot.GetPosList()[_idx].z <= _pos.z;
	}
	public Vector3[] GetSwimAreaPosList()
	{
		return swimRoot.GetPosList().ToArray();
	}
	public Vector3 GetSwimAreaPos(int _idx)
	{
		return swimRoot.GetPosList()[_idx];
	}
	public int GetSwimAreaLength()
	{
		return swimRoot.GetPosList().Count;
	}
}
