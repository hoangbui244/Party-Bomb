using UnityEngine;
using UnityEngine.Extension;
public class Shuriken_ReticleUI : DecoratedMonoBehaviour
{
	[SerializeField]
	[DisplayName("レティクル本体")]
	private Transform reticleTransform;
	[SerializeField]
	[DisplayName("レティクルアイコン")]
	private SpriteRenderer reticleIcon;
	[SerializeField]
	[DisplayName("コンフィグ")]
	private Shuriken_ReticleConfig config;
	public void Initialize(int no)
	{
		reticleIcon.color = config.GetColor(no);
	}
	public void Disable()
	{
		base.gameObject.SetActive(value: false);
	}
	public void UpdatePosition(Vector2 position)
	{
		Vector2 minReticlePosition = config.MinReticlePosition;
		Vector2 maxReticlePosition = config.MaxReticlePosition;
		position.x = Mathf.Clamp(position.x, minReticlePosition.x, maxReticlePosition.x);
		position.y = Mathf.Clamp(position.y, minReticlePosition.y, maxReticlePosition.y);
		reticleTransform.localPosition = position;
	}
}
