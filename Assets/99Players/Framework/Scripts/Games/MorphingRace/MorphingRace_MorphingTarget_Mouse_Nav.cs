using UnityEngine;
using UnityEngine.AI;
public class MorphingRace_MorphingTarget_Mouse_Nav : MonoBehaviour
{
	[SerializeField]
	[Header("ナビメッシュビルド後に非表示にする壁などのオブジェクト")]
	private GameObject[] arrayHideObject;
	private void Awake()
	{
		for (int i = 0; i < arrayHideObject.Length; i++)
		{
			arrayHideObject[i].SetActive(value: false);
		}
	}
	public void BuildNavMesh()
	{
		GetComponent<NavMeshSurface>().BuildNavMesh();
	}
	public void SetArrayHideObjectActive(bool _isActive)
	{
		for (int i = 0; i < arrayHideObject.Length; i++)
		{
			arrayHideObject[i].SetActive(_isActive);
		}
	}
}
