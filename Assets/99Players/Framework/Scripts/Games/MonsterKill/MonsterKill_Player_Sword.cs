using UnityEngine;
public class MonsterKill_Player_Sword : MonoBehaviour
{
	private MonsterKill_Player player;
	[SerializeField]
	[Header("メッシュ")]
	private MeshRenderer mesh;
	[SerializeField]
	[Header("トレイル")]
	private ParticleSystem trail;
	[SerializeField]
	[Header("攻撃用コライダ\u30fc")]
	private MonsterKill_AttackCollider attackCollider;
	public void Init(MonsterKill_Player _player)
	{
		player = _player;
		attackCollider.Init(player);
		attackCollider.gameObject.SetActive(value: false);
		trail.gameObject.SetActive(value: false);
	}
	public void SetSwordMaterial(Material _mat)
	{
		if (!(mesh == null))
		{
			mesh.material = _mat;
		}
	}
	public MeshRenderer GetSwordMesh()
	{
		return mesh;
	}
	public void AttackStart()
	{
		attackCollider.gameObject.SetActive(value: true);
		trail.gameObject.SetActive(value: true);
	}
	public void AttackEnd()
	{
		attackCollider.gameObject.SetActive(value: false);
		trail.gameObject.SetActive(value: false);
	}
}
