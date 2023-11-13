using UnityEngine;
public class RoadRaceActionPoint : MonoBehaviour
{
	[SerializeField]
	[Header("CPUがアクションしないフラグ")]
	private bool isCpuNotAction;
	public bool IsCpuNotAction => isCpuNotAction;
}
