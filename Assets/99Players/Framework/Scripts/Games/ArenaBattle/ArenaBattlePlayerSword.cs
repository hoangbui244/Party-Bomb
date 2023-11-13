using UnityEngine;
public class ArenaBattlePlayerSword : MonoBehaviour
{
	[SerializeField]
	[Header("ル\u30fcトプレイヤ\u30fc")]
	private ArenaBattlePlayer rootPlayer;
	[SerializeField]
	[Header("パ\u30fcティクルトレイル")]
	private ParticleSystem psTrail;
	[SerializeField]
	[Header("剣レンダラ\u30fc")]
	private MeshRenderer rendererSword;
	[SerializeField]
	[Header("剣マテリアル")]
	private Material[] arraySwordMat;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private BoxCollider boxCol;
	[SerializeField]
	[Header("必殺技コライダ\u30fc")]
	private BoxCollider boxColSp;
	[SerializeField]
	[Header("爆発パ\u30fcティクル")]
	private ParticleSystem psHit;
	private ArenaBattlePlayer target;
	private bool isSp;
	public ArenaBattlePlayer RootPlayer => rootPlayer;
	public void Init()
	{
		if ((bool)rootPlayer)
		{
			rendererSword.material = arraySwordMat[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[rootPlayer.IsCpu ? (4 + (rootPlayer.PlayerIdx - SingletonCustom<GameSettingManager>.Instance.PlayerNum)) : rootPlayer.PlayerIdx]];
		}
		SetCol(_isEnable: false);
		psTrail.gameObject.SetActive(value: false);
	}
	public MeshRenderer GetMesh()
	{
		return rendererSword;
	}
	public void SetCol(bool _isEnable)
	{
		boxCol.enabled = _isEnable;
	}
	public void AttackStart(bool _isSp = false)
	{
		if (!_isSp)
		{
			SetCol(_isEnable: true);
		}
		else
		{
			boxColSp.enabled = true;
		}
		isSp = _isSp;
		psTrail.gameObject.SetActive(value: true);
	}
	public void AttackEnd(bool _isSp = false)
	{
		if (!_isSp)
		{
			SetCol(_isEnable: false);
		}
		else
		{
			boxColSp.enabled = false;
		}
		psTrail.gameObject.SetActive(value: false);
		isSp = false;
	}
	public void SetHitEffet(Vector3 _pos)
	{
	}
	private void OnTriggerEnter(Collider other)
	{
		if (rootPlayer == null)
		{
			target = other.gameObject.GetComponent<ArenaBattlePlayer>();
			bool flag = target != null;
		}
		else
		{
			if (!(other.gameObject != rootPlayer.gameObject))
			{
				return;
			}
			target = other.gameObject.GetComponent<ArenaBattlePlayer>();
			if (target != null && !target.IsDodge && target.CurrentState != ArenaBattlePlayer.State.KNOCK_BACK && target.CurrentState != ArenaBattlePlayer.State.SWORD_ATTACK_SP)
			{
				target.KnockBack((target.transform.position - rootPlayer.transform.position).normalized, 1.5f, rootPlayer.GetAttackDamage());
				RootPlayer.HitStop();
				RootPlayer.SetVibration();
				if (isSp)
				{
					ParticleSystem particleSystem = UnityEngine.Object.Instantiate(psHit, SingletonCustom<ArenaBattleFieldManager>.Instance.transform);
					particleSystem.transform.position = target.transform.position;
					particleSystem.gameObject.SetActive(value: true);
					SingletonCustom<AudioManager>.Instance.SePlay("se_magic_hit", _loop: false, 0f, 1f, 1f, 0f, _overlap: true);
				}
				else
				{
					RootPlayer.AddSp();
				}
			}
		}
	}
}
