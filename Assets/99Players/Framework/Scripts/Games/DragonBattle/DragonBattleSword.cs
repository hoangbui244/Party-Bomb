using UnityEngine;
public class DragonBattleSword : MonoBehaviour
{
	[SerializeField]
	[Header("ル\u30fcトプレイヤ\u30fc")]
	private DragonBattlePlayer rootPlayer;
	[SerializeField]
	[Header("ル\u30fcトエネミ\u30fc")]
	private DragonBattleEnemyNinja rootBoss;
	[SerializeField]
	[Header("ヒットエフェクト")]
	private ParticleSystem psHitEffect;
	[SerializeField]
	[Header("トレイル")]
	private TrailRenderer trail;
	[SerializeField]
	[Header("剣レンダラ\u30fc")]
	private MeshRenderer rendererSword;
	[SerializeField]
	[Header("剣マテリアル")]
	private Material[] arraySwordMat;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private BoxCollider boxCol;
	private DragonBattlePlayer target;
	private DragonBattleEnemyNinja enemy;
	public DragonBattlePlayer RootPlayer => rootPlayer;
	public void Init()
	{
		if ((bool)rootPlayer)
		{
			rendererSword.material = arraySwordMat[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[rootPlayer.IsCpu ? (3 + rootPlayer.PlayerIdx) : rootPlayer.PlayerIdx]];
		}
	}
	public void SetCol(bool _isEnable)
	{
		boxCol.enabled = _isEnable;
	}
	public void AttackStart()
	{
		base.gameObject.SetActive(value: true);
	}
	public void AttackEnd()
	{
		base.gameObject.SetActive(value: false);
	}
	public bool IsAttack()
	{
		return base.gameObject.activeSelf;
	}
	public void SetHitEffet(Vector3 _pos)
	{
		psHitEffect.transform.position = _pos;
		psHitEffect.Play();
		SingletonCustom<AudioManager>.Instance.SePlay("se_attack_hit_3");
	}
	private void OnTriggerEnter(Collider other)
	{
		if (rootPlayer == null)
		{
			target = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(other.gameObject);
			if (target != null)
			{
				target.KnockBack((target.transform.position - rootBoss.transform.position).normalized, target.gsKnockBackPower.attackEnemy[0]);
				SetHitEffet(target.transform.position);
			}
		}
		else if (SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(other.gameObject) != rootPlayer)
		{
			target = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(other.gameObject);
			enemy = SingletonCustom<DragonBattleFieldManager>.Instance.CheckEnemy(other.gameObject);
			if (target != null)
			{
				target.KnockBack((target.transform.position - rootPlayer.transform.position).normalized, target.gsKnockBackPower.attackPlayer[0]);
				RootPlayer.SetVibration();
				SetHitEffet(target.transform.position);
			}
			else if (enemy != null)
			{
				enemy.OnDamage(RootPlayer);
				RootPlayer.SetVibration();
				SetHitEffet(enemy.transform.position);
			}
		}
	}
}
