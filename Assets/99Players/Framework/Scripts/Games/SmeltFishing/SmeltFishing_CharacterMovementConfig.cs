using UnityEngine;
using UnityEngine.Extension;
public class SmeltFishing_CharacterMovementConfig : ScriptableObject
{
	[Header("キャラクタ\u30fc移動設定")]
	[SerializeField]
	[DisplayName("歩行速度")]
	private float moveSpeed = 2f;
	[SerializeField]
	[DisplayName("振り向き速度")]
	private float rotationSpeed;
	public float MoveSpeed => moveSpeed;
	public float RotationSpeed => rotationSpeed;
}
