using System;
using System.Collections;
using UnityEngine;
public class Shooting_Target : MonoBehaviour, Shooting_IHitCallable
{
	[Serializable]
	public struct TargetData
	{
		[Header("中心点")]
		public GameObject centerPos;
		[Header("獲得ポイント")]
		public int point;
		[Header("的の半径")]
		public float radius;
		[Header("scoreの画像")]
		public Sprite[] scorePointSprite;
	}
	[SerializeField]
	[Header("的の種類")]
	private Shooting_TargetManager.TargetType targetType;
	[SerializeField]
	[Header("※内側か手前側から作ってください")]
	private TargetData[] targetData;
	private int targetNo;
	[SerializeField]
	[Header("スコアのScale")]
	private Vector3 scaleScore;
	private GameObject chilledObj;
	private int lastHitGunNo;
	[SerializeField]
	[Header("ヒットエフェクト")]
	protected ParticleSystem hitEffect;
	private bool isScoreNotification;
	[SerializeField]
	[Header("AIの狙う位置アンカ\u30fc")]
	private Transform aiTargetAnchor;
	private Rigidbody rigidbody;
	private bool isWind;
	private bool isWindSide;
	[SerializeField]
	[Header("空気抵抗係数")]
	private float coefficient;
	private Vector3 windSpeed;
	private float windTime;
	private float fiveKiteLimitTime;
	private bool fiveKiteDown;
	private float targetKiteHeight;
	private float hitTime;
	private float[] fiveKitesMoveTime;
	[SerializeField]
	[Header("５連凧専用")]
	private GameObject[] fiveKites;
	[SerializeField]
	private Shooting_Target fiveKitesTarget;
	private bool[] fiveKitesFlg;
	private bool quickKiteSide;
	private Vector3 quickKiteMovePos;
	[SerializeField]
	[Header("当たった時の吹っ飛ぶ力")]
	private float hitPower;
	[SerializeField]
	private GameObject centerObj;
	[SerializeField]
	private GameObject tragetPos;
	private bool isHit;
	private bool flg;
	private bool targetFlg;
	private bool targetOut;
	[SerializeField]
	[Header("親のリジットボディ")]
	private Rigidbody parentRigidbody;
	[SerializeField]
	private Shooting_TargetManager TManager;
	[SerializeField]
	private GameObject parentObj;
	[SerializeField]
	[Header("紐")]
	public GameObject Himo;
	[SerializeField]
	[Header("速いカイトの速さ")]
	private float targetSpeed;
	private float angle;
	private Vector3 pos;
	private Vector3 latestPos;
	private float fiveKiteMoveTime;
	[SerializeField]
	private Shooting_TargetAvoidance avoidance;
	private float kiteWind;
	private float kiteMove;
	private int strength;
	private bool isIn;
	public Shooting_TargetManager.TargetType TargetType => targetType;
	public TargetData[] TARGETData => targetData;
	public int TargetNo
	{
		get
		{
			return targetNo;
		}
		set
		{
			targetNo = value;
		}
	}
	public int LastHitGunNo
	{
		get
		{
			return lastHitGunNo;
		}
		set
		{
			lastHitGunNo = value;
		}
	}
	public bool IsWindSide
	{
		get
		{
			return isWindSide;
		}
		set
		{
			isWindSide = value;
		}
	}
	public bool QuickKiteSide
	{
		get
		{
			return quickKiteSide;
		}
		set
		{
			quickKiteSide = value;
		}
	}
	public bool TargetOut => targetOut;
	public float KiteWind
	{
		get
		{
			return kiteWind;
		}
		set
		{
			kiteWind = value;
		}
	}
	public float KiteMove
	{
		get
		{
			return kiteMove;
		}
		set
		{
			kiteMove = value;
		}
	}
	private void OnDrawGizmosSelected()
	{
		if (targetData.Length == 0)
		{
			return;
		}
		for (int i = 0; i < targetData.Length; i++)
		{
			if (tragetPos != null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(tragetPos.transform.localPosition, targetData[i].radius);
			}
			else if (targetData[i].centerPos == null)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(base.gameObject.transform.position, targetData[i].radius);
			}
			else
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(targetData[i].centerPos.transform.position, targetData[i].radius);
			}
		}
	}
	public void Init()
	{
		if (targetType == Shooting_TargetManager.TargetType.STRING)
		{
			return;
		}
		if (aiTargetAnchor != null)
		{
			SphereCollider component = aiTargetAnchor.GetComponent<SphereCollider>();
			int aiStrength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
			component.radius *= Shooting_Define.AI_AIM_COLLIDER_SCALE_MAGS[0];
		}
		if (targetType == Shooting_TargetManager.TargetType.TYPE_FIVE_KITES)
		{
			if (strength == 0)
			{
				aiTargetAnchor.transform.parent = fiveKites[0].transform;
			}
			else if (strength == 1)
			{
				aiTargetAnchor.transform.parent = fiveKites[3].transform;
			}
			else if (strength == 2)
			{
				aiTargetAnchor.transform.parent = fiveKites[4].transform;
			}
			aiTargetAnchor.gameObject.transform.localPosition = Vector3.zero;
		}
		strength = SingletonCustom<SaveDataManager>.Instance.SaveData.systemData.aiStrength;
	}
	private void Start()
	{
		rigidbody = GetComponent<Rigidbody>();
		switch (targetType)
		{
		case Shooting_TargetManager.TargetType.TYPE_KITE:
			kiteWind = 2.2f;
			kiteMove = 0f;
			fiveKitesMoveTime = new float[1];
			chilledObj = base.transform.GetChild(UnityEngine.Random.Range(0, 2)).gameObject;
			chilledObj.SetActive(value: true);
			targetKiteHeight = UnityEngine.Random.Range(3f, 8f);
			break;
		case Shooting_TargetManager.TargetType.TYPE_FIVE_KITES:
			fiveKitesFlg = new bool[5];
			fiveKitesMoveTime = new float[5];
			fiveKiteMoveTime = 0f;
			for (int i = 0; i < fiveKitesFlg.Length; i++)
			{
				fiveKitesMoveTime[i] = 0.5f;
			}
			StartCoroutine(TimeMove(fiveKiteMoveTime, delegate
			{
				fiveKitesFlg[4] = true;
			}));
			fiveKiteMoveTime += 0.25f;
			StartCoroutine(TimeMove(fiveKiteMoveTime, delegate
			{
				fiveKitesFlg[3] = true;
			}));
			fiveKiteMoveTime += 0.25f;
			StartCoroutine(TimeMove(fiveKiteMoveTime, delegate
			{
				fiveKitesFlg[2] = true;
			}));
			fiveKiteMoveTime += 0.25f;
			StartCoroutine(TimeMove(fiveKiteMoveTime, delegate
			{
				fiveKitesFlg[1] = true;
			}));
			fiveKiteMoveTime += 0.25f;
			StartCoroutine(TimeMove(fiveKiteMoveTime, delegate
			{
				fiveKitesFlg[0] = true;
			}));
			break;
		case Shooting_TargetManager.TargetType.TYPE_QUICK:
			quickKiteMovePos = base.gameObject.transform.localPosition;
			StartThrow(base.gameObject, 2.5f, quickKiteMovePos, new Vector3(0f, 15f, 200f), 250f);
			SingletonCustom<Shooting_TargetManager>.Instance.QuickKiteCount++;
			chilledObj = base.transform.GetChild(0).gameObject;
			break;
		}
		Init();
		if (!(TManager == null))
		{
			TManager.List.Add(this);
		}
	}
	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		switch (targetType)
		{
		case Shooting_TargetManager.TargetType.TYPE_FALL:
			if (Time.timeScale == 0f)
			{
				return;
			}
			TargetFall();
			break;
		case Shooting_TargetManager.TargetType.TYPE_KITE:
			TargetKite();
			break;
		case Shooting_TargetManager.TargetType.TYPE_FIVE_KITES:
			TargetFiveKites();
			break;
		case Shooting_TargetManager.TargetType.TYPE_QUICK:
			TragetQuick();
			break;
		case Shooting_TargetManager.TargetType.STRING:
			StringMove();
			break;
		case Shooting_TargetManager.TargetType.TRAGET:
			TargetFall();
			break;
		}
		if (isHit)
		{
			if (Time.timeScale == 0f)
			{
				return;
			}
			Hit();
			hitTime += Time.deltaTime;
			if (hitTime >= 0.5f)
			{
				isHit = false;
			}
		}
		else
		{
			hitTime = 0f;
		}
		TargetAimOut();
	}
	private void TargetAimOut()
	{
		switch (targetType)
		{
		case Shooting_TargetManager.TargetType.TYPE_FALL:
			if (base.transform.localPosition.y <= -25f || base.transform.localPosition.z >= 150f)
			{
				targetOut = true;
			}
			break;
		case Shooting_TargetManager.TargetType.TYPE_KITE:
			if (base.transform.localPosition.x >= 100f || base.transform.localPosition.z >= 150f)
			{
				targetOut = true;
			}
			break;
		default:
			if (base.transform.localPosition.y <= -25f || base.transform.localPosition.z >= 150f)
			{
				targetOut = true;
			}
			break;
		case Shooting_TargetManager.TargetType.TYPE_FIVE_KITES:
		case Shooting_TargetManager.TargetType.TYPE_QUICK:
			break;
		}
		if (TManager == null || targetFlg)
		{
			return;
		}
		switch (targetType)
		{
		case Shooting_TargetManager.TargetType.TYPE_FALL:
			if (base.transform.localPosition.y <= 35f)
			{
				TManager.List.Add(this);
			}
			break;
		case Shooting_TargetManager.TargetType.TYPE_KITE:
			if (base.transform.localPosition.y >= -10f)
			{
				TManager.List.Add(this);
			}
			break;
		}
		targetFlg = true;
	}
	public void Hit()
	{
		switch (targetType)
		{
		case Shooting_TargetManager.TargetType.TYPE_FALL:
		case Shooting_TargetManager.TargetType.TYPE_FIVE_KITES:
		case Shooting_TargetManager.TargetType.TYPE_QUICK:
		case Shooting_TargetManager.TargetType.STRING:
			break;
		case Shooting_TargetManager.TargetType.TYPE_KITE:
			TargetKiteHit();
			break;
		case Shooting_TargetManager.TargetType.TRAGET:
			TargetFallHit();
			break;
		}
	}
	public Transform GetAiTargetAnchor()
	{
		return aiTargetAnchor;
	}
	public Vector3 GetAiTargetPos()
	{
		return aiTargetAnchor.position;
	}
	public void CallHit(int _gunNo, Vector3 _vec, Vector3 _pos)
	{
		throw new NotImplementedException();
	}
	private void TargetFall()
	{
		if (avoidance == null || avoidance.OnHit)
		{
			return;
		}
		if (fiveKitesTarget == null)
		{
			isWind = true;
			if (!isWind)
			{
				return;
			}
			Vector3 a;
			if (isWindSide)
			{
				windSpeed = new Vector3(5f, 0f, 0f);
				a = windSpeed - rigidbody.velocity;
				windTime = Mathf.Clamp(windTime + Time.deltaTime / 2f, 0f, 1f);
				if (windTime >= 1f)
				{
					isWindSide = false;
				}
			}
			else
			{
				windSpeed = new Vector3(-5f, 0f, 0f);
				a = windSpeed - rigidbody.velocity;
				windTime = Mathf.Clamp(windTime - Time.deltaTime / 2f, 0f, 1f);
				if (windTime <= 0f)
				{
					isWindSide = true;
				}
			}
			rigidbody.AddForce(coefficient * a);
		}
		else
		{
			Shooting_TargetManager.TargetType targetType2 = fiveKitesTarget.targetType;
		}
	}
	private void TargetFallHit()
	{
		if (fiveKitesTarget == null && Time.timeScale != 0f)
		{
			parentRigidbody.AddForce(Vector3.forward * hitPower);
		}
	}
	private void TargetKite()
	{
		if (base.transform.localPosition.y <= targetKiteHeight)
		{
			rigidbody.velocity = new Vector3(0.25f, 3.5f);
			return;
		}
		rigidbody.velocity = new Vector3(kiteWind, 0.35f, kiteMove);
		fiveKitesMoveTime[0] = Mathf.Clamp(fiveKitesMoveTime[0] + Time.deltaTime / 0.5f, 0f, 1f);
		base.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(350f, 5f, 0f), Quaternion.Euler(345f, 15f, 0f), fiveKitesMoveTime[0]);
	}
	private void TargetKiteHit()
	{
		rigidbody.AddForce(Vector3.forward * hitPower, ForceMode.Impulse);
	}
	private void TargetFiveKites()
	{
		if (!fiveKiteDown)
		{
			if ((!SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && base.transform.localPosition.y <= -13f) || (SingletonCustom<GameSettingManager>.Instance.IsSinglePlay && base.transform.localPosition.y <= -8f))
			{
				rigidbody.velocity = Vector3.up * 6.5f;
			}
			else
			{
				fiveKiteLimitTime += Time.deltaTime;
				rigidbody.velocity = Vector3.zero;
				if (fiveKiteLimitTime >= 10f)
				{
					fiveKiteDown = true;
					SingletonCustom<Shooting_TargetManager>.Instance.IsFiveKite = true;
				}
				rigidbody.velocity = Vector3.zero;
			}
		}
		else
		{
			rigidbody.velocity = Vector3.zero;
		}
		FiveKitesMove(0);
		FiveKitesMove(1);
		FiveKitesMove(2);
		FiveKitesMove(3);
		FiveKitesMove(4);
	}
	private void FiveKitesMove(int _KitesNo)
	{
		fiveKites[_KitesNo].transform.localPosition = Vector3.Lerp(new Vector3(1.5f, fiveKites[_KitesNo].transform.localPosition.y, fiveKites[_KitesNo].transform.localPosition.z), new Vector3(-1.5f, fiveKites[_KitesNo].transform.localPosition.y, fiveKites[_KitesNo].transform.localPosition.z), fiveKitesMoveTime[_KitesNo]);
		if (fiveKitesFlg[_KitesNo])
		{
			fiveKitesMoveTime[_KitesNo] = Mathf.Clamp(fiveKitesMoveTime[_KitesNo] + Time.deltaTime / 3.5f, 0f, 1f);
			if (fiveKitesMoveTime[_KitesNo] >= 1f)
			{
				fiveKitesFlg[_KitesNo] = false;
			}
		}
		else
		{
			fiveKitesMoveTime[_KitesNo] = Mathf.Clamp(fiveKitesMoveTime[_KitesNo] - Time.deltaTime / 3.5f, 0f, 1f);
			if (fiveKitesMoveTime[_KitesNo] <= 0f)
			{
				fiveKitesFlg[_KitesNo] = true;
			}
		}
	}
	public IEnumerator TimeMove(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
	private void StringMove()
	{
		base.transform.position = Himo.transform.position;
	}
	private void TragetQuick()
	{
		rigidbody.velocity = Vector3.zero;
		Vector3 up = base.transform.localPosition - latestPos;
		latestPos = base.transform.localPosition;
		if (up.magnitude > 0.01f)
		{
			base.transform.up = up;
		}
		if (!flg)
		{
			if (base.transform.localPosition.y >= 15f)
			{
				pos = base.transform.localPosition;
				flg = true;
			}
		}
		else
		{
			angle += Time.deltaTime * 0.3f;
			base.transform.localPosition = new Vector3(pos.x + Mathf.Sin(angle) * 100f, pos.y + Mathf.Sin(angle * 2f) * 30f, pos.z);
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (SingletonCustom<Shooting_GameManager>.Instance.Timer.RemainingTime <= 0f || !collision.gameObject.CompareTag("Player"))
		{
			return;
		}
		Shooting_Bullet component = collision.gameObject.GetComponent<Shooting_Bullet>();
		float x = 0f;
		float y = 0f;
		Vector3 zero = Vector3.zero;
		if (centerObj == null)
		{
			collision.gameObject.transform.parent = base.gameObject.transform;
		}
		else if (targetType == Shooting_TargetManager.TargetType.TYPE_QUICK)
		{
			collision.gameObject.transform.parent = centerObj.transform;
		}
		else
		{
			collision.gameObject.transform.parent = centerObj.transform;
		}
		UnityEngine.Object.Destroy(component.Collider);
		if (!component.IsPleryer)
		{
			if (component.transform.localPosition.x <= 0f - targetData[2].radius || component.transform.localPosition.x >= targetData[2].radius)
			{
				x = component.transform.localPosition.x;
				isIn = true;
			}
			if (component.transform.localPosition.y <= 0f - targetData[2].radius || component.transform.localPosition.y >= targetData[2].radius)
			{
				y = component.transform.localPosition.y;
				isIn = true;
			}
			if (!isIn)
			{
				if (component.transform.localPosition.x <= 0f)
				{
					x = Mathf.Clamp(component.transform.localPosition.x + Shooting_Define.AI_SUCK_TARGET[strength], -2.4f, 0f);
				}
				else if (component.transform.localPosition.x >= 0f)
				{
					x = Mathf.Clamp(component.transform.localPosition.x - Shooting_Define.AI_SUCK_TARGET[strength], 0f, 2.4f);
				}
				if (component.transform.localPosition.y <= 0f)
				{
					y = Mathf.Clamp(component.transform.localPosition.y + Shooting_Define.AI_SUCK_TARGET[strength], -2.4f, 0f);
				}
				else if (component.transform.localPosition.y >= 0f)
				{
					x = Mathf.Clamp(component.transform.localPosition.x - Shooting_Define.AI_SUCK_TARGET[strength], 0f, 2.4f);
				}
			}
		}
		else
		{
			x = component.transform.localPosition.x;
			y = component.transform.localPosition.y;
		}
		component.transform.localPosition = new Vector3(x, y, component.transform.localPosition.z);
		zero = collision.gameObject.transform.localPosition;
		for (int i = 0; i < targetData.Length; i++)
		{
			if (targetData[i].centerPos == null)
			{
				if (centerObj == null)
				{
					if (InSphere(Vector3.zero, targetData[i].radius, zero))
					{
						SingletonCustom<Shooting_ScoreManager>.Instance.AddScore(component.GunNo, targetData[i].scorePointSprite[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[component.PlayerNo]], collision.transform.position, targetData[i].point, scaleScore);
						SingletonCustom<AudioManager>.Instance.SePlay("se_blowgun_hit_1");
						component.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
						UnityEngine.Object.Destroy(component.Rigid);
						break;
					}
					if (i >= targetData.Length - 1)
					{
						SingletonCustom<Shooting_BulletManager>.Instance.Add(component.GunNo, component);
						break;
					}
				}
				else
				{
					if (InSphere(tragetPos.transform.localPosition, targetData[i].radius, zero))
					{
						SingletonCustom<Shooting_ScoreManager>.Instance.AddScore(component.GunNo, targetData[i].scorePointSprite[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[component.PlayerNo]], collision.transform.position, targetData[i].point, scaleScore);
						SingletonCustom<AudioManager>.Instance.SePlay("se_blowgun_hit_1");
						UnityEngine.Object.Destroy(component.Rigid);
						component.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
						break;
					}
					if (i >= targetData.Length - 1)
					{
						SingletonCustom<Shooting_BulletManager>.Instance.Add(component.GunNo, component);
						break;
					}
				}
				continue;
			}
			if (InSphere(targetData[i].centerPos.transform.localPosition, targetData[i].radius, zero))
			{
				SingletonCustom<Shooting_ScoreManager>.Instance.AddScore(component.GunNo, targetData[i].scorePointSprite[SingletonCustom<GameSettingManager>.Instance.ArraySelectChracterIdx[component.PlayerNo]], collision.transform.position, targetData[i].point, scaleScore);
				SingletonCustom<AudioManager>.Instance.SePlay("se_blowgun_hit_1");
				component.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				UnityEngine.Object.Destroy(component.Rigid);
				if (targetType == Shooting_TargetManager.TargetType.TYPE_KITE || targetType == Shooting_TargetManager.TargetType.TYPE_QUICK)
				{
					collision.gameObject.transform.parent = base.gameObject.transform;
				}
				else if (parentObj != null)
				{
					collision.gameObject.transform.parent = parentObj.transform;
				}
				else
				{
					collision.gameObject.transform.parent = targetData[i].centerPos.transform;
				}
				break;
			}
			if (i >= targetData.Length - 1)
			{
				SingletonCustom<Shooting_BulletManager>.Instance.Add(component.GunNo, component);
				break;
			}
		}
		isIn = false;
		isHit = true;
	}
	public bool InSphere(Vector3 p, float r, Vector3 c)
	{
		float num = 0f;
		for (int i = 0; i < 3; i++)
		{
			num += Mathf.Pow(p[i] - c[i], 2f);
		}
		return num <= Mathf.Pow(r, 2f);
	}
	public bool RandomBool()
	{
		return UnityEngine.Random.Range(0, 2) == 0;
	}
	public void StartThrow(GameObject target, float height, Vector3 start, Vector3 end, float duration)
	{
		Vector3 half = end - start * 0.5f + start;
		half.y += Vector3.up.y + height;
		half.z -= 100f;
		half.x += Vector3.left.x * height * 25f;
		StartCoroutine(LerpThrow(target, start, half, end, duration));
	}
	private IEnumerator LerpThrow(GameObject target, Vector3 start, Vector3 half, Vector3 end, float duration)
	{
		float startTime = Time.timeSinceLevelLoad;
		float rate = 0f;
		while (!(rate >= 1f))
		{
			float num = Time.timeSinceLevelLoad - startTime;
			rate = num / (duration / 60f);
			target.transform.localPosition = CalcLerpPoint(start, half, end, rate);
			yield return null;
		}
	}
	private Vector3 CalcLerpPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
	{
		Vector3 a = Vector3.Lerp(p0, p1, t);
		Vector3 b = Vector3.Lerp(p1, p2, t);
		return Vector3.Lerp(a, b, t);
	}
}
