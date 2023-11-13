using UnityEngine;
public class FireWorksDestroyScript : MonoBehaviour
{
	public GameObject FireWork;
	public float DestroyTime;
	public float nowTime;
	private void Start()
	{
	}
	private void Update()
	{
		if (nowTime > DestroyTime)
		{
			UnityEngine.Object.Destroy(FireWork);
		}
		nowTime += Time.deltaTime;
	}
}
