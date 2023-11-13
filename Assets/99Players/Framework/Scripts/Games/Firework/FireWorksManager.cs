using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FireWorksManager : MonoBehaviour
{
	[Serializable]
	public struct FireWorksList
	{
		public ParticleSystem[] fireWorksParts;
	}
	[Serializable]
	public struct LimitValue
	{
		public float min;
		public float max;
		public LimitValue(float _min, float _max)
		{
			min = _min;
			max = _max;
		}
	}
	[SerializeField]
	[Header("花火の大きさ")]
	public float EFFECT_SCALE = 1f;
	[SerializeField]
	[Header("打ち上げ速度")]
	public float SHOOT_UP_SPEED = 1f;
	[SerializeField]
	[Header("打ち上げ時間")]
	private float SHOOT_UP_TIME;
	private float shootUpTime;
	[SerializeField]
	[Header("エフェクトリスト")]
	private FireWorksList[] fireWorksList;
	private Vector3 createPos;
	private Vector2 createRange;
	[SerializeField]
	[Header("花火の生成インダ\u30fcバル")]
	private LimitValue CREATE_INTERVAL = new LimitValue(0.25f, 1f);
	private float createInterval;
	private List<ParticleSystem> createEffectList = new List<ParticleSystem>();
	private Quaternion rotate;
	[SerializeField]
	[Header("花火が爆発するまでの時間")]
	private LimitValue EXPLOSION_TIME = new LimitValue(0.8f, 1.2f);
	[SerializeField]
	[Header("赤色の範囲")]
	[Range(0f, 1f)]
	public float redColorMin = 0.5f;
	[SerializeField]
	[Range(0f, 1f)]
	public float redColorMax = 1f;
	[SerializeField]
	[Header("緑色の範囲")]
	[Range(0f, 1f)]
	public float greenColorMin = 0.5f;
	[SerializeField]
	[Range(0f, 1f)]
	public float greenColorMax = 1f;
	[SerializeField]
	[Header("青色の範囲")]
	[Range(0f, 1f)]
	public float blueColorMin = 0.5f;
	[SerializeField]
	[Range(0f, 1f)]
	public float blueColorMax = 1f;
	[SerializeField]
	[Header("SEの再生")]
	private bool isPlaySe = true;
	public readonly string hint = "BoxColliderで花火の出現範囲を指定できます";
	private Vector4 startColor;
	public void Awake()
	{
		for (int i = 0; i < fireWorksList.Length; i++)
		{
			fireWorksList[i].fireWorksParts[0].startSpeed *= SHOOT_UP_SPEED;
			for (int j = 0; j < fireWorksList[i].fireWorksParts.Length; j++)
			{
				fireWorksList[i].fireWorksParts[j].startSize *= EFFECT_SCALE;
				fireWorksList[i].fireWorksParts[j].startSpeed *= EFFECT_SCALE;
				fireWorksList[i].fireWorksParts[j].startRotation *= EFFECT_SCALE;
				fireWorksList[i].fireWorksParts[j].gravityModifier *= EFFECT_SCALE;
			}
		}
	}
	private void Start()
	{
		rotate.eulerAngles = new Vector3(270f, 0f, 0f);
		BoxCollider2D component = GetComponent<BoxCollider2D>();
		createPos.x = component.offset.x;
		createPos.y = component.offset.y;
		createPos.z = 0f;
		createRange = component.size;
	}
	private void OnEnable()
	{
		for (int num = createEffectList.Count - 1; num >= 0; num--)
		{
			if (createEffectList[num] != null)
			{
				UnityEngine.Object.Destroy(createEffectList[num].gameObject);
				createEffectList.RemoveAt(num);
			}
			else
			{
				createEffectList.RemoveAt(num);
			}
		}
	}
	private void Update()
	{
		shootUpTime += Time.deltaTime;
		if (SHOOT_UP_TIME > 0f && shootUpTime >= SHOOT_UP_TIME)
		{
			return;
		}
		if (createInterval <= 0f)
		{
			startColor = new Vector4(UnityEngine.Random.Range(redColorMin, redColorMax), UnityEngine.Random.Range(greenColorMin, greenColorMax), UnityEngine.Random.Range(blueColorMin, blueColorMax), 1f);
			createEffectList.Add(UnityEngine.Object.Instantiate(fireWorksList[UnityEngine.Random.Range(0, fireWorksList.Length)].fireWorksParts[0], base.transform.position + createPos + new Vector3(UnityEngine.Random.Range((0f - createRange.x) / 2f, createRange.x / 2f), UnityEngine.Random.Range((0f - createRange.y) / 2f, createRange.y / 2f), 0f), rotate));
			createEffectList[createEffectList.Count - 1].transform.parent = base.transform;
			createEffectList[createEffectList.Count - 1].startLifetime *= UnityEngine.Random.Range(EXPLOSION_TIME.min, EXPLOSION_TIME.max);
			createEffectList[createEffectList.Count - 1].gameObject.SetActive(value: true);
			for (int i = 0; i < fireWorksList.Length; i++)
			{
				for (int j = 0; j < fireWorksList[i].fireWorksParts.Length; j++)
				{
					fireWorksList[i].fireWorksParts[j].startColor = startColor;
				}
			}
			createEffectList[createEffectList.Count - 1].Play();
			if (UnityEngine.Random.Range(0, 3) == 0 && isPlaySe)
			{
				SingletonCustom<AudioManager>.Instance.SePlay("se_fireworks_shoot_up", _loop: false, 0f, 0.25f);
			}
			StartCoroutine(_PlaySeExplo(createEffectList[createEffectList.Count - 1].startLifetime));
			createInterval = UnityEngine.Random.Range(CREATE_INTERVAL.min, CREATE_INTERVAL.max);
		}
		createInterval -= Time.deltaTime;
		for (int num = createEffectList.Count - 1; num >= 0; num--)
		{
			if (createEffectList[num] == null)
			{
				createEffectList.RemoveAt(num);
			}
		}
	}
	private IEnumerator _PlaySeExplo(float _delayTime)
	{
		yield return new WaitForSeconds(_delayTime);
		if (isPlaySe)
		{
			SingletonCustom<AudioManager>.Instance.SePlay("se_fireworks_explo");
		}
	}
	public void SetShootUpTime(float _shootUpTime)
	{
		SHOOT_UP_TIME = _shootUpTime;
	}
}
