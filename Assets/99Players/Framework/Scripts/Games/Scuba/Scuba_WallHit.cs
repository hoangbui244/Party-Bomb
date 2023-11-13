using UnityEngine;
[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class Scuba_WallHit : MonoBehaviour
{
	[SerializeField]
	private float scale = 1f;
	[SerializeField]
	[Range(0f, 1f)]
	private float alpha = 1f;
	private MeshRenderer meshRenderer;
	private MaterialPropertyBlock pb;
	private static int _MainTex_ST = Shader.PropertyToID("_MainTex_ST");
	private static int _Alpha = Shader.PropertyToID("_Alpha");
	private void Update()
	{
		if (meshRenderer == null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
		}
		if (pb == null)
		{
			pb = new MaterialPropertyBlock();
		}
		float num = 1f / Mathf.Max(0.1f, scale);
		float num2 = (1f - num) * 0.5f;
		pb.SetVector(_MainTex_ST, new Vector4(num, num, num2, num2));
		pb.SetFloat(_Alpha, Mathf.Clamp01(alpha));
		meshRenderer.SetPropertyBlock(pb);
	}
}
