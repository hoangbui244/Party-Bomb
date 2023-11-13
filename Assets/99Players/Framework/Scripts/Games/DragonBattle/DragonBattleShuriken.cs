using UnityEngine;
public class DragonBattleShuriken : MonoBehaviour
{
	public enum EffectType
	{
		PLAYER,
		BULLET,
		DARK,
		DARK_RED,
		MAX
	}
	private EffectType effectType;
	[SerializeField]
	[Header("ル\u30fcトプレイヤ\u30fc")]
	private DragonBattlePlayer rootPlayer;
	[SerializeField]
	[Header("敵忍者")]
	private DragonBattleEnemyNinja enemyNinja;
	private ParticleSystem effect;
	private ParticleSystem hitEffect;
	private ParticleSystem startEffect;
	public static readonly float MoveSpeed = 12.5f;
	public static readonly float MoveSpeedEnemy = 5f;
	private float moveSpeedMag;
	private Vector3 moveDir;
	private float lifeTime = 7f;
	private DragonBattlePlayer target;
	private DragonBattleGoldBox box;
	private DragonBattleEnemyNinja enemy;
	[SerializeField]
	private SphereCollider col;
	[SerializeField]
	private Rigidbody rigid;
	private float power;
	public EffectType Type
	{
		get
		{
			return effectType;
		}
		set
		{
			effectType = value;
		}
	}
	public DragonBattlePlayer RootPlayer => rootPlayer;
	public float Power => power;
	public void Play(Vector3 _dir, EffectType _type = EffectType.PLAYER, int _no = 0, float _power = 1f)
	{
		base.gameObject.SetActive(value: true);
		moveDir = _dir;
		if (_type == EffectType.BULLET)
		{
			moveSpeedMag = 4f;
		}
		else
		{
			moveSpeedMag = 1f;
		}
		base.transform.forward = moveDir;
		effectType = _type;
		if (effectType == EffectType.PLAYER)
		{
			effect = SingletonCustom<DragonBattleResources>.Instance.EffectList.playerAttack[_no];
			hitEffect = SingletonCustom<DragonBattleResources>.Instance.EffectList.playerAttackHit[_no];
			SingletonCustom<AudioManager>.Instance.SePlay("se_magic_bullet");
		}
		else
		{
			switch (_type)
			{
			case EffectType.BULLET:
				SingletonCustom<AudioManager>.Instance.SePlay("se_magic_bullet");
				SingletonCustom<AudioManager>.Instance.SePlay("se_magic_bullet");
				break;
			case EffectType.DARK:
			case EffectType.DARK_RED:
				SingletonCustom<AudioManager>.Instance.SePlay("se_collision_character_10");
				SingletonCustom<AudioManager>.Instance.SePlay("se_collision_character_10");
				break;
			}
			_no = (int)(_type - 1);
			effect = SingletonCustom<DragonBattleResources>.Instance.EffectList.enemyAttack[_no];
			hitEffect = SingletonCustom<DragonBattleResources>.Instance.EffectList.enemyAttackHit[_no];
			startEffect = SingletonCustom<DragonBattleResources>.Instance.EffectList.enemyAttackStart[_no];
		}
		effect = UnityEngine.Object.Instantiate(effect, base.transform.position, Quaternion.identity, base.transform);
		effect.transform.SetLocalEulerAngles(0f, 180f, 0f);
		effect.gameObject.SetActive(value: true);
		effect.Play(withChildren: true);
		if ((bool)startEffect)
		{
			startEffect = UnityEngine.Object.Instantiate(startEffect, base.transform.position, Quaternion.identity, base.transform.parent);
			startEffect.transform.SetEulerAngles(0f, base.transform.eulerAngles.y, 0f);
			startEffect.gameObject.SetActive(value: true);
			startEffect.Play(withChildren: true);
		}
		if (effectType == EffectType.PLAYER)
		{
			Transform[] componentsInChildren = effect.GetComponentsInChildren<Transform>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetLocalScale(_power, _power, _power);
			}
			col.radius *= _power;
			power = _power;
			rigid.velocity = moveDir * MoveSpeed * moveSpeedMag;
		}
		else
		{
			switch (_type)
			{
			case EffectType.BULLET:
				power = 3f;
				col.radius *= power;
				break;
			case EffectType.DARK:
			case EffectType.DARK_RED:
				power = 1.5f;
				break;
			default:
				power = 1f;
				break;
			}
			rigid.velocity = moveDir * MoveSpeedEnemy * moveSpeedMag;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.1f, ~DragonBattleDefine.ConvertLayerMask("HitCharacterOnly"));
		if (array.Length == 0)
		{
			return;
		}
		for (int j = 0; j < array.Length; j++)
		{
			if (array[j].gameObject.tag == "Box")
			{
				array[j].gameObject.GetComponent<DragonBattleGoldBox>().HitShuriken(this);
			}
			else
			{
				OnTriggerEnter(array[j]);
			}
			if (!effect.gameObject.activeSelf)
			{
				break;
			}
		}
	}
	public void SetHitEffect(Vector3 _pos)
	{
		if (hitEffect != null)
		{
			hitEffect = UnityEngine.Object.Instantiate(hitEffect, base.transform.position, Quaternion.identity, base.transform);
			hitEffect.transform.position = _pos;
			hitEffect.gameObject.SetActive(value: true);
			hitEffect.Play(withChildren: true);
		}
	}
	private void Update()
	{
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		if (!rigid.isKinematic)
		{
			base.transform.forward = moveDir;
		}
	}
	public void OnHit(Vector3 _pos, bool _effect = true, bool _isPlayerAttack = false)
	{
		if (_effect)
		{
			effect.gameObject.SetActive(value: false);
			rigid.isKinematic = true;
			col.enabled = false;
			SetHitEffect(_pos);
			lifeTime = 2.5f;
			if ((bool)rootPlayer)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_magic_hit");
			}
			else if (!_isPlayerAttack)
			{
				switch (effectType)
				{
				case EffectType.BULLET:
					SingletonCustom<AudioManager>.Instance.SePlay("se_magic_hit");
					SingletonCustom<AudioManager>.Instance.SePlay("se_magic_hit");
					break;
				case EffectType.DARK:
				case EffectType.DARK_RED:
					SingletonCustom<AudioManager>.Instance.SePlay("se_damage");
					break;
				}
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
	private void HitPlayer()
	{
		if ((bool)enemyNinja)
		{
			target.KnockBack((target.transform.position - base.transform.position).normalized, target.gsKnockBackPower.attackEnemy[1], _isHitEnemy: false, enemyNinja.IsBoss, Power);
		}
		else
		{
			target.KnockBack((target.transform.position - base.transform.position).normalized, target.gsKnockBackPower.attackPlayer[1], _isHitEnemy: false, _isBoss: false, Power);
		}
		SingletonCustom<AudioManager>.Instance.SePlay("se_chara_break");
		OnHit(target.transform.position);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "CheckPoint" || other.gameObject.tag == "Box")
		{
			return;
		}
		if (other.gameObject.tag == "Ball")
		{
			DragonBattleShuriken component = other.gameObject.GetComponent<DragonBattleShuriken>();
			if (!(component.rootPlayer == rootPlayer) && !(component.enemyNinja == enemyNinja) && !(Power - component.Power >= 0.5f))
			{
				OnHit(base.transform.position, _effect: true, component.rootPlayer != null);
			}
		}
		else
		{
			CheckPlayerAttack(other);
			if ((bool)enemyNinja && !CheckPlayerHit(other.gameObject) && (!(other.gameObject.tag == "Ninja") || !(SingletonCustom<DragonBattleFieldManager>.Instance.CheckEnemy(other.gameObject) == enemyNinja)))
			{
				OnHit(base.transform.position);
			}
		}
	}
	private void CheckPlayerAttack(Collider other)
	{
		if (!rootPlayer || CheckPlayerHit(other.gameObject))
		{
			return;
		}
		if (other.gameObject.tag == "Ninja")
		{
			enemy = SingletonCustom<DragonBattleFieldManager>.Instance.CheckEnemy(other.gameObject);
			if (enemy != null)
			{
				enemy.OnDamage(rootPlayer, Power);
				if (enemy.IsBoss)
				{
					OnHit(base.transform.position);
				}
				else
				{
					OnHit(enemy.transform.position);
				}
				return;
			}
		}
		OnHit(base.transform.position);
	}
	private bool CheckPlayerHit(GameObject _obj)
	{
		if (_obj.tag == "Player")
		{
			target = SingletonCustom<DragonBattlePlayerManager>.Instance.CheckPlayer(_obj);
			if (target == rootPlayer)
			{
				return true;
			}
			if (target != null)
			{
				HitPlayer();
			}
			return true;
		}
		return false;
	}
	private void OnDestroy()
	{
		if ((bool)startEffect)
		{
			UnityEngine.Object.Destroy(startEffect.gameObject);
		}
	}
}
