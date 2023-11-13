using UnityEngine;
using UnityEngine.Extension;
public class FlyingSquirrelRace_Backgrounds_TextureLayer : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("背景レンダラ\u30fc")]
	[Attach(AttachMode.FindInChildren)]
	private Renderer bgRenderer;
	[SerializeField]
	[DisplayName("スピ\u30fcドに対する補正")]
	private float scaleFactor = 1f;
	private float offset;
	private Material bgMaterial;
	private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
	public void Initialize()
	{
		bgMaterial = bgRenderer.material;
		bgMaterial = Object.Instantiate(bgMaterial);
		bgRenderer.material = bgMaterial;
	}
	public void UpdateMethod(float speed)
	{
		ScrollBg(speed);
	}
	private void ScrollBg(float speed)
	{
		speed *= scaleFactor;
		offset += Time.deltaTime * speed;
		offset = Mathf.Repeat(offset, 1f);
		bgMaterial.SetTextureOffset(MainTexId, new Vector2(offset, 0f));
	}
}
