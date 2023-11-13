using UnityEngine;
public class SwordFight_Sword : MonoBehaviour
{
	[SerializeField]
	[Header("キャラクタ\u30fc")]
	private SwordFight_CharacterScript character;
	[SerializeField]
	[Header("トレイルPS")]
	private ParticleSystem psTrail;
	[SerializeField]
	[Header("コライダ\u30fc")]
	private BoxCollider boxCol;
	[SerializeField]
	[Header("ドロップ用モデル")]
	private GameObject objDropModel;
	private const float BASE_KNOCKBACK_POWER = 300f;
	private const float KNOCKBACK_POWER_2ND_ATTACK = 100f;
	private const float KNOCKBACK_POWER_LAST_ATTACK = 200f;
	private const float TIME_ADD_KNOCKBACK_POWER = 1f;
	private float KnockBackPower = 300f;
	private bool isSwingVertical;
	private bool isSwingLeftHorizontal;
	private bool isSwingRightHorizontal;
	private int haveCharaNo = -1;
	private int haveCharaTeamNo = -1;
	private bool isTeamMode;
	private static readonly float DAMAGE_VERTICAL_NORMAL = 3.5f;
	private static readonly float DAMAGE_VERTICAL_LAST = 6f;
	private static readonly float DAMAGE_HORIZONTAL_NORMAL = 3f;
	private static readonly float DAMAGE_HORIZONTAL_LAST = 6.5f;
	private float damage;
	private GameObject instanceDrop;
	private void Start()
	{
		haveCharaNo = character.CharaNo;
		haveCharaTeamNo = character.TeamNo;
		isTeamMode = SwordFight_Define.IS_TEAM_MODE;
	}
	public void Init()
	{
		haveCharaNo = character.CharaNo;
		haveCharaTeamNo = character.TeamNo;
		isTeamMode = SwordFight_Define.IS_TEAM_MODE;
		psTrail.Stop();
		psTrail.Clear();
		boxCol.enabled = false;
		boxCol.isTrigger = true;
		base.gameObject.SetActive(value: true);
		if (instanceDrop != null)
		{
			UnityEngine.Object.Destroy(instanceDrop);
		}
	}
	public void Drop()
	{
		base.gameObject.SetActive(value: false);
		instanceDrop = UnityEngine.Object.Instantiate(objDropModel, character.transform.parent);
		instanceDrop.transform.position = base.transform.position;
		instanceDrop.transform.rotation = base.transform.rotation;
		instanceDrop.transform.localScale = new Vector3(2.673f, 2.673f, 2.673f);
		Rigidbody component = instanceDrop.GetComponent<Rigidbody>();
		component.AddForce(new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f)), ForceMode.Impulse);
		component.AddTorque(new Vector3(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, 5f)), ForceMode.Impulse);
		instanceDrop.gameObject.SetActive(value: true);
	}
	private void Update()
	{
		KnockBackPower = 300f + (float)(Mathf.FloorToInt(SwordFight_Define.GAME_TIME_TOTAL / 10f) - Mathf.FloorToInt(SingletonCustom<SwordFight_GameUiManager>.Instance.GetGameTime() / 10f)) * 1f;
	}
	private void OnTriggerEnter(Collider other)
	{
		if ((!isSwingVertical && !isSwingLeftHorizontal && !isSwingRightHorizontal) || !(other.gameObject.GetComponentInParent<SwordFight_CharacterScript>() != null) || other.gameObject.GetComponentInParent<SwordFight_CharacterScript>().CharaNo == haveCharaNo)
		{
			return;
		}
		SwordFight_CharacterScript componentInParent = other.gameObject.GetComponentInParent<SwordFight_CharacterScript>();
		if (componentInParent.GetActionState() == SwordFight_CharacterScript.ActionState.DEFENCE)
		{
			if (componentInParent.CheckTargetAngleCharacter(-45f, 45f))
			{
				character.RepelAnimation();
			}
			else
			{
				if (!character.CheckTargetAngleCharacter(-45f, 45f))
				{
					return;
				}
				character.ResetDamageCount();
				if (character.IsSecondAttack)
				{
					KnockBackPower += 100f;
				}
				if (character.IsLastAttack)
				{
					KnockBackPower += 200f;
				}
				if (isSwingVertical)
				{
					if (character.IsLastAttack)
					{
						damage = DAMAGE_VERTICAL_NORMAL;
					}
					else
					{
						damage = DAMAGE_VERTICAL_LAST;
					}
				}
				if (isSwingLeftHorizontal)
				{
					if (character.IsLastAttack)
					{
						damage = DAMAGE_HORIZONTAL_NORMAL;
					}
					else
					{
						damage = DAMAGE_HORIZONTAL_LAST;
					}
				}
				if (isSwingRightHorizontal)
				{
					if (character.IsLastAttack)
					{
						damage = DAMAGE_HORIZONTAL_NORMAL;
					}
					else
					{
						damage = DAMAGE_HORIZONTAL_LAST;
					}
				}
				componentInParent.KnockBackAnimation(character, KnockBackPower, isSwingVertical, isSwingLeftHorizontal, isSwingRightHorizontal, character.IsFirstAttack, character.IsSecondAttack, character.IsLastAttack, damage);
				character.SetVibration();
				if (componentInParent.HpScale <= 0f)
				{
					character.StopRunEffect();
				}
			}
			return;
		}
		character.ResetDamageCount();
		if (character.IsSecondAttack)
		{
			KnockBackPower += 100f;
		}
		if (character.IsLastAttack)
		{
			KnockBackPower += 200f;
		}
		if (isSwingVertical)
		{
			if (character.IsLastAttack)
			{
				damage = DAMAGE_VERTICAL_NORMAL;
			}
			else
			{
				damage = DAMAGE_VERTICAL_LAST;
			}
		}
		if (isSwingLeftHorizontal)
		{
			if (character.IsLastAttack)
			{
				damage = DAMAGE_HORIZONTAL_NORMAL;
			}
			else
			{
				damage = DAMAGE_HORIZONTAL_LAST;
			}
		}
		if (isSwingRightHorizontal)
		{
			if (character.IsLastAttack)
			{
				damage = DAMAGE_HORIZONTAL_NORMAL;
			}
			else
			{
				damage = DAMAGE_HORIZONTAL_LAST;
			}
		}
		componentInParent.KnockBackAnimation(character, KnockBackPower, isSwingVertical, isSwingLeftHorizontal, isSwingRightHorizontal, character.IsFirstAttack, character.IsSecondAttack, character.IsLastAttack, damage);
		character.SetVibration();
		if (componentInParent.HpScale <= 0f)
		{
			character.StopRunEffect();
		}
	}
	public void SetDeath()
	{
		boxCol.enabled = true;
		boxCol.isTrigger = false;
	}
	public void SetVerticalSwingFlg(bool _swingFlg)
	{
		if (_swingFlg)
		{
			psTrail.Clear();
			psTrail.Play();
		}
		else
		{
			psTrail.Stop();
		}
		isSwingVertical = _swingFlg;
		boxCol.enabled = _swingFlg;
	}
	public void SetHorizontalLeftSwingFlg(bool _swingFlg)
	{
		if (_swingFlg)
		{
			psTrail.Clear();
			psTrail.Play();
		}
		else
		{
			psTrail.Stop();
		}
		isSwingLeftHorizontal = _swingFlg;
		boxCol.enabled = _swingFlg;
	}
	public void SetHorizontalRightSwingFlg(bool _swingFlg)
	{
		if (_swingFlg)
		{
			psTrail.Clear();
			psTrail.Play();
		}
		else
		{
			psTrail.Stop();
		}
		isSwingRightHorizontal = _swingFlg;
		boxCol.enabled = _swingFlg;
	}
	public void DisableTrail()
	{
		psTrail.Clear();
		psTrail.Stop();
		boxCol.enabled = false;
	}
}
