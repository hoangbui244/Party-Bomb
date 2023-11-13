using UnityEngine;
public class SnowBoard_RailRidePointEffect : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer renderer;
	[SerializeField]
	private float speed = 1f;
	private Material mat;
	private Vector4 mainTex;
	private static int _MainTex_ST = Shader.PropertyToID("_MainTex_ST");
	private void Start()
	{
		if (renderer == null)
		{
			base.enabled = false;
		}
		else if (!(renderer.sharedMaterial == null))
		{
			mat = UnityEngine.Object.Instantiate(renderer.sharedMaterial);
			renderer.sharedMaterial = mat;
			mainTex = mat.GetVector(_MainTex_ST);
			renderer.transform.up = Vector3.up;
		}
	}
	private void Update()
	{
		mainTex.w -= Time.deltaTime * speed;
		mat.SetVector(_MainTex_ST, mainTex);
	}
}
