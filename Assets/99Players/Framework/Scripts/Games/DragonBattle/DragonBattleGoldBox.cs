using System;
using UnityEngine;
public class DragonBattleGoldBox : MonoBehaviour
{
	public enum Type
	{
		BOX,
		BARREL,
		POT
	}
	private GameObject model;
	private int modelNo;
	[SerializeField]
	[Header("破壊時の破片")]
	private GameObject parts;
	[SerializeField]
	[Header("ボックスコライダ\u30fc")]
	private BoxCollider boxCol;
	[SerializeField]
	[Header("種類")]
	private Type type;
	private IsRendered isRendered;
	private bool isRender;
	private ParticleSystemRenderer[] psr = new ParticleSystemRenderer[0];
	public bool IsRender => isRendered;
	private void Start()
	{
		modelNo = UnityEngine.Random.Range(0, SingletonCustom<DragonBattleResources>.Instance.ObjectList.rocks.Length);
		GameObject gameObject = SingletonCustom<DragonBattleResources>.Instance.ObjectList.rocks[modelNo];
		model = UnityEngine.Object.Instantiate(gameObject, base.transform.position + gameObject.transform.localPosition, Quaternion.identity, base.transform);
		model.transform.localScale = gameObject.transform.localScale;
		isRendered = model.GetComponent<IsRendered>();
		model.SetActive(value: true);
		model.transform.SetLocalEulerAngles(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
		model.transform.localScale *= UnityEngine.Random.Range(0.9f, 1f);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (IsRender)
		{
			CheckHitShuriken(other);
		}
	}
	private void CheckHitSword(Collider other)
	{
		DragonBattleSword component = other.gameObject.GetComponent<DragonBattleSword>();
		if (component != null)
		{
			switch (type)
			{
			case Type.BOX:
			case Type.BARREL:
				SingletonCustom<AudioManager>.Instance.SePlay("se_breakbox");
				break;
			case Type.POT:
				SingletonCustom<AudioManager>.Instance.SePlay("se_stage_item_pot_break");
				break;
			}
			SingletonCustom<AudioManager>.Instance.SePlay("se_goeraser_item_0", _loop: false, 0f, 1f, 1f, 0.1f);
			component.SetHitEffet(base.transform.position);
			component.RootPlayer.AddScore(50);
			model.SetActive(value: false);
			if ((bool)boxCol)
			{
				boxCol.enabled = false;
			}
			parts.SetActive(value: true);
			psr = GetComponentsInChildren<ParticleSystemRenderer>();
			base.gameObject.name = "DestroyWait";
			LeanTween.delayedCall(2f, (Action)delegate
			{
				if (base.gameObject != null)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			});
		}
	}
	private void CheckHitShuriken(Collider other)
	{
		if (other.tag == "Ball")
		{
			DragonBattleShuriken component = other.gameObject.GetComponent<DragonBattleShuriken>();
			if (component != null)
			{
				HitShuriken(component);
			}
		}
	}
	public void HitShuriken(DragonBattleShuriken _shuriken)
	{
		SingletonCustom<AudioManager>.Instance.SePlay("se_breakbox");
		SingletonCustom<AudioManager>.Instance.SePlay("se_goeraser_item_0", _loop: false, 0f, 1f, 1f, 0.1f);
		if ((bool)_shuriken.RootPlayer)
		{
			_shuriken.RootPlayer.AddScore(50, _isShowValue: true, _isVibration: false);
		}
		_shuriken.OnHit(_shuriken.transform.position);
		model.SetActive(value: false);
		if ((bool)boxCol)
		{
			boxCol.enabled = false;
		}
		parts.SetActive(value: true);
		psr = GetComponentsInChildren<ParticleSystemRenderer>();
		base.gameObject.name = "DestroyWait";
		LeanTween.delayedCall(2f, (Action)delegate
		{
			if (base.gameObject != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		});
	}
	private void Update()
	{
		isRender = IsRender;
		if (base.transform.localPosition.y <= -50f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
	private void OnDestroy()
	{
		LeanTween.cancel(base.gameObject);
	}
}
